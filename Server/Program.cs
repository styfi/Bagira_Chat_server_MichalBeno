namespace Server
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var port = 5000;
            if (args.Length >= 1 && int.TryParse(args[0], out var p))
            {
                port = p;
            }

            var server = new ChatServer(port);

            Console.CancelKeyPress += (_, e) =>
            {
                e.Cancel = true;
                server.RequestStop();
            };

            await server.RunAsync();
        }
    }
}
