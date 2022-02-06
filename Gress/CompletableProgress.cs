using System;

namespace Gress;

public class CompletableProgress<T> : ICompletableProgress<T>
{
    private readonly Action<T> _handleReport;
    private readonly Action _handleCompletion;

    public CompletableProgress(Action<T> handleReport, Action handleCompletion)
    {
        _handleReport = handleReport;
        _handleCompletion = handleCompletion;
    }

    public void Report(T value) => _handleReport(value);

    public void Dispose() => _handleCompletion();
}