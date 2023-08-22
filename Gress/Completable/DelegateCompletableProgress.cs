using System;

namespace Gress.Completable;

/// <summary>
/// Simple implementation of <see cref="ICompletableProgress{T}" /> that uses the provided
/// delegates to report progress updates and signal completion.
/// </summary>
public class DelegateCompletableProgress<T> : ICompletableProgress<T>
{
    private readonly Action<T> _report;
    private readonly Action _reportCompletion;

    /// <summary>
    /// Initializes an instance of <see cref="DelegateCompletableProgress{T}" />.
    /// </summary>
    public DelegateCompletableProgress(Action<T> report, Action reportCompletion)
    {
        _report = report;
        _reportCompletion = reportCompletion;
    }

    /// <inheritdoc />
    public void Report(T value) => _report(value);

    /// <inheritdoc />
    public void ReportCompletion() => _reportCompletion();
}
