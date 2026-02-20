using Protocol;

namespace Server.CommandHandling
{
    public interface ICommandHandler<in T> where T : IChatCommand
    {
        Task HandleAsync(ClientConnection clientConnection, T command, CancellationToken ct);
    }
}
