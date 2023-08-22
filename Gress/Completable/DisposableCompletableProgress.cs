using System;

namespace Gress.Completable;

/// <summary>
/// Convenience wrapper for <see cref="ICompletableProgress{T}" /> that reports completion
/// on disposal.
/// </summary>
public class DisposableCompletableProgress<T> : ICompletableProgress<T>, IDisposable
{
    private readonly ICompletableProgress<T> _target;

    /// <summary>
    /// Initializes an instance of <see cref="DisposableCompletableProgress{T}" />.
    /// </summary>
    public DisposableCompletableProgress(ICompletableProgress<T> target) => _target = target;

    /// <inheritdoc />
    public void Report(T value) => _target.Report(value);

    /// <inheritdoc />
    public void ReportCompletion() => _target.ReportCompletion();

    /// <inheritdoc cref="ReportCompletion" />
    public void Dispose() => ReportCompletion();
}
