using System;

namespace Gress.Completable;

/// <summary>
/// Simple implementation of <see cref="ICompletableProgress{T}" /> that uses the provided
/// delegates to report progress updates and signal completion.
/// </summary>
public class DelegateCompletableProgress<T>(Action<T> report, Action reportCompletion)
    : ICompletableProgress<T>
{
    /// <inheritdoc />
    public void Report(T value) => report(value);

    /// <inheritdoc />
    public void ReportCompletion() => reportCompletion();
}
