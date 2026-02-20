using System;
using System.Collections.Generic;
using System.Text;

namespace Protocol
{
    public interface ITransport : IAsyncDisposable
    {
        Task<string?> ReceiveAsync(CancellationToken cancellationToken);
        Task SendAsync(string message, CancellationToken cancellationToken);
    }
}
