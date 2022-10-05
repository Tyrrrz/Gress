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
public class DelegateProgress<T> : IProgress<T>
{
    private readonly Action<T> _report;

    /// <summary>
    /// Initializes an instance of <see cref="DelegateProgress{T}" />.
    /// </summary>
    public DelegateProgress(Action<T> report) => _report = report;

    /// <inheritdoc />
    public void Report(T value) => _report(value);
}