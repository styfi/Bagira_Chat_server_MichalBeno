namespace Server.CommandHandling
{
    public sealed class LoginCommand : IChatCommand
    {
        private string Name { get; }

        public LoginCommand(string name)
        {
            Name = name;
        }

        public async Task HandleAsync(ClientConnection clientConnection, CancellationToken ct)
        {
            if (clientConnection.SetUserName(Name))
            {
                await clientConnection.SendAsync($"{Name} - Client connected successfully", ct);
            }
            else
            {
                await clientConnection.SendAsync($"ERROR - client {Name} already on server!", ct);
            }
        }
    }
}
