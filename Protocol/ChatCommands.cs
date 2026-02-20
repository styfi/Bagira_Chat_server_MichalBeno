namespace Protocol
{
    public sealed record LoginCommand(string Name) : IChatCommand;

    public sealed record PublicCommand(string Content) : IChatCommand;

    public sealed record PrivateCommand(string Target, string Content) : IChatCommand;
}
