namespace Protocol
{
    public static class CommandParser
    {
        public static IChatCommand? Parse(string line)
        {
            if (string.IsNullOrWhiteSpace(line))
                return null;

            var parts = line.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);
            var cmd = parts[0].ToUpperInvariant();
            var arg = parts.Length > 1 ? parts[1] : "";

            return cmd switch
            {
                "LOGIN" => new LoginCommand(arg.Trim()),
                "PUBLIC" => new PublicCommand(arg),
                "PRIVATE" => ParsePrivate(arg),

                _ => null
            };
        }

        private static IChatCommand? ParsePrivate(string arg)
        {
            var parts = arg.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length < 2) return null;

            return new PrivateCommand(parts[0], parts[1]);
        }
    }
}
