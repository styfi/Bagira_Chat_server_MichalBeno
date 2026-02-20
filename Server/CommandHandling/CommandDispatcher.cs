using Protocol;

namespace Server.CommandHandling
{
    public sealed class CommandDispatcher
    {
        private readonly LoginHandler _login = new();
        private readonly PublicHandler _public;
        private readonly PrivateHandler _private;

        public CommandDispatcher(ChatServer server)
        {
            _public = new PublicHandler(server);
            _private = new PrivateHandler(server);
        }

        public Task DispatchAsync(
            ClientConnection ctx,
            IChatCommand command,
            CancellationToken ct)
        {
            return command switch
            {
                LoginCommand c => _login.HandleAsync(ctx, c, ct),
                PublicCommand c => _public.HandleAsync(ctx, c, ct),
                PrivateCommand c => _private.HandleAsync(ctx, c, ct),
                _ => Task.CompletedTask
            };
        }
    }
}
