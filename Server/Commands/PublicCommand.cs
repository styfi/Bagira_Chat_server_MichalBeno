namespace Server.CommandHandling
{
    public sealed class PublicCommand : IChatCommand
    {
        private readonly ChatServer Server;
        private readonly string Content;

        public PublicCommand(ChatServer server, string content)
        {
            this.Server = server;
            Content = content;
        }
          


        public async Task HandleAsync(ClientConnection clientConnection, CancellationToken ct)
        {
            if (clientConnection.UserName is null)
            {
                await clientConnection.SendAsync("ERROR Not logged in", ct);

                return;
            }

            await Server.SendPublicAsync(clientConnection.UserName, Content, ct);
        }
    }
}
