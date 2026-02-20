using Protocol;

namespace Server.CommandHandling
{
    public sealed class PrivateHandler : ICommandHandler<PrivateCommand>
    {
        private readonly ChatServer _server;

        public PrivateHandler(ChatServer server) => _server = server;

        public async Task HandleAsync(ClientConnection ctx, PrivateCommand cmd, CancellationToken ct)
        {
            if (ctx.UserName is null)
            {
                await ctx.SendAsync("ERROR Not logged in", ct);

                return;
            }

            await _server.SendPrivateAsync(ctx.UserName, cmd.Target, cmd.Content, ct);
        }
    }
}
