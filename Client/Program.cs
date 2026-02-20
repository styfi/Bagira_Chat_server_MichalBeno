using ClientLib;

namespace Client
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var host = args.Length >= 1 ? args[0] : "127.0.0.1";
            var port = (args.Length >= 2 && int.TryParse(args[1], out var p)) ? p : 5000;

            var client = new ChatClient(host, port);

            Console.CancelKeyPress += (_, e) =>
            {
                e.Cancel = true;
                client.RequestStop();
            };

            await client.RunAsync();
        }
    }
}
