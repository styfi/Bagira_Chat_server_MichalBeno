using Protocol;
using Server.CommandHandling;
using System.Net.Sockets;

namespace Server
{
    public sealed class ClientConnection
    {
        private readonly TcpClient _client;
        private readonly ChatServer _server;
        private readonly ITransport _transport;
        private readonly CommandParser _parser;

        private readonly SemaphoreSlim _writeLock = new(1, 1);

        public int Id { get; }
        public string UserName { get; set; } = string.Empty;

        public ClientConnection(int id, TcpClient client, ChatServer server)
        {
            Id = id;
            _client = client;
            _server = server;

            var _stream = _client.GetStream();
            _transport = new TextTransport(_stream);
            _parser = new CommandParser(server);
        }

        public bool SetUserName(string name)
        {
            if (!_server.GetUserNames().Contains(name.ToLowerInvariant()))
            {
                UserName = name;
                return true;
            }
            else
            {
                return false;
            }
        }

        public void Close()
        {
            try 
            { 
                _client.Close(); 
            } catch 
            { 
            }
        }

        public async Task RunAsync(CancellationToken ct)
        {
            try
            {
                while (!ct.IsCancellationRequested)
                {
                    var readTask = await Task.Run(async () => _transport.ReceiveAsync(ct));

                    var line = await readTask;
                    if (line is null)
                    {
                        break;
                    }

                    var command = _parser.Parse(line);

                    if (command != null)
                    {
                        await command.HandleAsync(this, ct);
                    }
                    else
                    {
                        await SendAsync("ERROR Unknown command", ct);
                    }
                }
            }
            catch (OperationCanceledException) { }
            catch (Exception ex)
            {
                Console.WriteLine($"Client {Id} error: {ex.Message}");
            }
            finally
            {
                _server.RemoveClient(Id);
                Close();
                Console.WriteLine($"Client {Id} disconnected");
            }
        }

        public async Task SendAsync(string line, CancellationToken ct)
        {
            if (!_client.Connected)
            {
                return;
            }

            await _writeLock.WaitAsync();
            try
            {
                await _transport.SendAsync(line, ct);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Send failed (client {Id}): {ex.Message}");
                Close();
            }
            finally
            {
                _writeLock.Release();
            }
        }
    }
}
