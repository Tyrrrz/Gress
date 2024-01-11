using System;

namespace Gress;

/// <summary>
/// Simple implementation of <see cref="IProgress{T}" /> that uses the provided
/// delegate to report progress updates.
/// </summary>
/// <remarks>
/// Unlike <see cref="Progress{T}" />, this implementation does not post updates
/// through a synchronization context, but rather calls the delegate directly.
/// </remarks>
public class DelegateProgress<T>(Action<T> report) : IProgress<T>
{
    /// <inheritdoc />
    public void Report(T value) => report(value);
}
