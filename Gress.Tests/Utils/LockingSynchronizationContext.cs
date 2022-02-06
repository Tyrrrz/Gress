using System.Threading;

namespace Gress.Tests.Utils;

internal partial class LockingSynchronizationContext : SynchronizationContext
{
    public override void Post(SendOrPostCallback d, object? state) => d(state);

    public override void Send(SendOrPostCallback d, object? state) => Post(d, state);
}

internal partial class LockingSynchronizationContext
{
    public static LockingSynchronizationContext Instance { get; } = new();
}