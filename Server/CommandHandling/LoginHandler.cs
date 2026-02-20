using Protocol;

namespace Server.CommandHandling
{
    public sealed class LoginHandler : ICommandHandler<LoginCommand>
    {
        public Task HandleAsync(ClientConnection clientConnection, LoginCommand cmd, CancellationToken ct)
        {
            if (clientConnection.SetUserName(cmd.Name))
            {
                return clientConnection.SendAsync($"{cmd.Name} - Client connected successfully", ct);
            }
            {
                return clientConnection.SendAsync($"ERROR - client {cmd.Name} already on server!", ct);
            }
        }
    }
}
