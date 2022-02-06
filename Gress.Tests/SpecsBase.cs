using System;
using System.Threading;
using Gress.Tests.Utils;

namespace Gress.Tests;

public abstract class SpecsBase : IDisposable
{
    // Progress<T> uses synchronization context to report updates, so we need this
    // to make all operations run synchronously.
    protected SpecsBase() =>
        SynchronizationContext.SetSynchronizationContext(LockingSynchronizationContext.Instance);

    public void Dispose() =>
        SynchronizationContext.SetSynchronizationContext(null);
}