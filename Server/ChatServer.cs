using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Xml.Linq;

namespace Server
{
    public sealed class ChatServer
    {
        private readonly TcpListener _listener;
        private readonly ConcurrentDictionary<int, ClientConnection> _clients = new();
        private int _nextId = 0;
        private readonly CancellationTokenSource _cts = new();
        private readonly LetterCounter _letterCounter = new();

        public ChatServer(int port)
        {
            _listener = new TcpListener(IPAddress.Any, port);
        }

        public void RequestStop() => _cts.Cancel();

        public async Task RunAsync()
        {
            _listener.Start();
            Console.WriteLine($"Server listening on port {((IPEndPoint)_listener.LocalEndpoint).Port}");

            try
            {
                while (!_cts.IsCancellationRequested)
                {
                    var tcpClient = await _listener.AcceptTcpClientAsync(_cts.Token);
                    tcpClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);

                    var id = Interlocked.Increment(ref _nextId);

                    var conn = new ClientConnection(id, tcpClient, this);
                    _clients[id] = conn;

                    _ = conn.RunAsync(_cts.Token);
                }
            }
            catch (OperationCanceledException) { }
            finally
            {
                _listener.Stop();
                foreach (var c in _clients.Values)
                {
                    c.Close();
                }
                Console.WriteLine("Server stopped");
            }
        }

        internal void RemoveClient(int id)
        {
            _clients.TryRemove(id, out _);
            Console.WriteLine($"Client {id} removed");
        }

        internal IEnumerable<string> GetUserNames()
            => _clients.Values
                .Select(c => c.UserName.ToLowerInvariant())
                .Where(n => !string.IsNullOrWhiteSpace(n))!
                .Cast<string>();

        internal async Task SendPublicAsync(string fromUser, string text, CancellationToken ct)
        {
            Console.WriteLine(_letterCounter.ToString());
            _letterCounter.UpdateCounter(text);

            foreach (var c in _clients.Values.Where(c => !c.UserName.Equals(fromUser, StringComparison.OrdinalIgnoreCase)))
            {
                await c.SendAsync($"{fromUser.ToUpper()} - {text}", ct);
            }
        }

        internal async Task SendPrivateAsync(string fromUser, string toUser, string text, CancellationToken ct)
        {
            Console.WriteLine(_letterCounter.ToString());
            _letterCounter.UpdateCounter(text);

            var conn = FindByUserName(toUser);
            if (conn != null)
            {
                await conn.SendAsync($"{fromUser.ToUpper()} - {text}", ct);
            } 
        }

        internal ClientConnection? FindByUserName(string name)
        {
            return _clients.Values.FirstOrDefault(c =>
                c.UserName != null &&
                c.UserName.Equals(name, StringComparison.OrdinalIgnoreCase));
        }
    }
}
