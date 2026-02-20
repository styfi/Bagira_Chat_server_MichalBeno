using System.Net.Sockets;

namespace Protocol
{
    public class TextTransport : ITransport
    {
        private readonly StreamReader _reader;
        private readonly StreamWriter _writer;

        public TextTransport(NetworkStream stream)
        {
            _reader = new StreamReader(stream, leaveOpen: true);
            _writer = new StreamWriter(stream, leaveOpen: true) { AutoFlush = true };
        }

        public ValueTask DisposeAsync()
        {
            _reader.Dispose();
            _writer.Dispose();

            return ValueTask.CompletedTask;
        }

        public async Task<string?> ReceiveAsync(CancellationToken cancellationToken)
        {
            return await _reader.ReadLineAsync(cancellationToken);
        }

        public async Task SendAsync(string message, CancellationToken cancellationToken)
        {
            await _writer.WriteLineAsync(message);
        }
    }
}
