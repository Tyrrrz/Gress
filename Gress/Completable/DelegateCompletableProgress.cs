using System;

namespace Gress.Completable;

internal class DelegateCompletableProgress<T> : ICompletableProgress<T>
{
    private readonly Action<T> _report;
    private readonly Action _reportCompletion;

    public DelegateCompletableProgress(Action<T> report, Action reportCompletion)
    {
        _report = report;
        _reportCompletion = reportCompletion;
    }

    public void Report(T value) => _report(value);

    public void ReportCompletion() => _reportCompletion();
}