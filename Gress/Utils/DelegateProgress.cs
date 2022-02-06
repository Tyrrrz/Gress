using System;

namespace Gress.Utils;

// Simple progress handler without thread scheduling
internal class DelegateProgress<T> : IProgress<T>
{
    private readonly Action<T> _handle;

    public DelegateProgress(Action<T> handle) => _handle = handle;

    public void Report(T value) => _handle(value);
}