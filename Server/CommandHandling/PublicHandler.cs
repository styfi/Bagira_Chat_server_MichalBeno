using Protocol;

namespace Server.CommandHandling
{
    public sealed class PublicHandler : ICommandHandler<PublicCommand>
    {
        private readonly ChatServer _server;

        public PublicHandler(ChatServer server) => _server = server;

        public async Task HandleAsync(ClientConnection clientConnection, PublicCommand cmd, CancellationToken ct)
        {
            if (clientConnection.UserName is null)
            {
                await clientConnection.SendAsync("ERROR Not logged in", ct);

                return;
            }

            await _server.SendPublicAsync(clientConnection.UserName, cmd.Content, ct);
        }
    }
}
