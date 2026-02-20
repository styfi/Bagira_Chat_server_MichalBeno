using Protocol;
using System.Net.Sockets;

namespace ClientLib
{
    public sealed class ChatClient
    {
        private readonly string _host;
        private readonly int _port;
        private readonly CancellationTokenSource _cts = new();

        private ITransport Transport { get; set; }
        private string UserName { get; set; }

        public ChatClient(string host, int port)
        {
            _host = host;
            _port = port;
        }

        public void RequestStop() => _cts.Cancel();

        public async Task RunAsync()
        {
            while (string.IsNullOrWhiteSpace(UserName))
            {
                Console.WriteLine("Please enter Name:");
                UserName = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(UserName))
                {
                    Console.WriteLine("Name cannot be empty. Please try again.");
                }
            }

            using var tcp = new TcpClient();
            try
            {
                await tcp.ConnectAsync(_host, _port, _cts.Token);
                tcp.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);
                Transport = new TextTransport(tcp.GetStream());
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Client Failed to connect to the server.");
                _cts.Cancel();
            }

            var readTask = Task.Run(async () =>
            {
                try
                {
                    while (!_cts.IsCancellationRequested)
                    {
                        var line = await Transport.ReceiveAsync(_cts.Token);
                        if (line is null)
                        {
                            break;
                        }

                        Console.WriteLine();
                        Console.WriteLine(line);
                        Console.Write(MessageFormatter.Format($"{UserName.ToUpper()} - "));
                    }
                }
                catch (OperationCanceledException) { }
                catch (Exception ex)
                {
                    Console.WriteLine($"Read error: {ex.Message}");
                }
                finally
                {
                    _cts.Cancel();
                }
            });     

            await Transport.SendAsync($"{CommandNames.Login} {UserName}", _cts.Token);
            Console.WriteLine($"Connected to {_host}:{_port}");

            try
            {
                while (!_cts.IsCancellationRequested)
                {
                    var input = Console.ReadLine();
                    if (input is null)
                    {
                        break;
                    }

                    input = input.Trim();
                    if (input.Length == 0)
                    {
                        continue;
                    }

                    var message = string.Empty;
                    if (input.ToLowerInvariant().StartsWith("to:"))
                    {
                        input = input.Substring(3).Trim();
                        var parts = input.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);
                        if (parts.Length < 2)
                        {
                            Console.WriteLine("Invalid private message format. Use: to:recipient message");
                            continue;
                        }
                        var recipient = parts[0];
                        var content = parts[1];
                        message = $"{CommandNames.Private} {recipient} {content}";
                    }
                    else
                    {
                        message = $"{CommandNames.Public} {input}";
                    }

                    Console.Write(MessageFormatter.Format($"{UserName.ToUpper()} - "));
                    await Transport.SendAsync(message, _cts.Token);
                }
            }
            catch (OperationCanceledException) { }
            catch (Exception ex)
            {
                Console.WriteLine($"Write error: {ex.Message}");
            }
            finally
            {
                _cts.Cancel();

                try
                {
                    tcp.Close();
                }
                catch { }

                await readTask;
                Console.WriteLine("Client stopped");
            }
        }
    }
}
