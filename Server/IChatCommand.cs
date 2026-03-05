namespace Server

{
    public interface IChatCommand
    {
        Task HandleAsync(ClientConnection clientConnection, CancellationToken ct);
    }
}
