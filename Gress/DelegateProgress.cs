using System;

namespace Gress;

// Simple progress handler without thread scheduling
internal class DelegateProgress<T> : IProgress<T>
{
    private readonly Action<T> _report;

    public DelegateProgress(Action<T> report) => _report = report;

    public void Report(T value) => _report(value);
}