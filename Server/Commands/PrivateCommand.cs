namespace Server.CommandHandling
{
    public sealed class PrivateCommand : IChatCommand
    {
        private ChatServer Server { get; }
        private string Target { get; }
        private string Content { get; }

        public PrivateCommand(ChatServer server, string target, string content)
        {
            Server = server;
            Target = target;
            Content = content;
        }

        public async Task HandleAsync(ClientConnection clientConnection, CancellationToken ct)
        {
            if (clientConnection.UserName is null)
            {
                await clientConnection.SendAsync("ERROR Not logged in", ct);

                return;
            }

            await Server.SendPrivateAsync(clientConnection.UserName, Target, Content, ct);
        }
    }
}
