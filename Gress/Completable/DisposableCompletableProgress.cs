using System;

namespace Gress.Completable;

/// <summary>
/// Convenience wrapper for <see cref="ICompletableProgress{T}" /> that reports completion
/// on disposal.
/// </summary>
public class DisposableCompletableProgress<T>(ICompletableProgress<T> target)
    : ICompletableProgress<T>,
        IDisposable
{
    /// <inheritdoc />
    public void Report(T value) => target.Report(value);

    /// <inheritdoc />
    public void ReportCompletion() => target.ReportCompletion();

    /// <inheritdoc cref="ReportCompletion" />
    public void Dispose() => ReportCompletion();
}
