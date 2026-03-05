using Server.CommandHandling;

namespace Server
{
    public class CommandParser
    {
        private ChatServer Server { get; }

        public CommandParser(ChatServer server)
        {
            Server = server;
        }

        public IChatCommand? Parse(string line)
        {
            if (string.IsNullOrWhiteSpace(line))
                return null;

            var parts = line.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);
            var cmd = parts[0].ToUpperInvariant();
            var arg = parts.Length > 1 ? parts[1] : "";

            return cmd switch
            {
                "LOGIN" => new LoginCommand(arg.Trim()),
                "PUBLIC" => new PublicCommand(Server, arg),
                "PRIVATE" => ParsePrivate(arg),

                _ => null
            };
        }

        private IChatCommand? ParsePrivate(string arg)
        {
            var parts = arg.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length < 2) return null;

            return new PrivateCommand(Server, parts[0], parts[1]);
        }
    }
}
