using System;

namespace Gress.Tests.Internal
{
    // System.Progress<T> uses weird thread scheduling shenanigans which we want to avoid
    public class DelegateProgress<T> : IProgress<T>
    {
        private readonly Action<T> _handleProgress;

        public DelegateProgress(Action<T> handleProgress) =>
            _handleProgress = handleProgress;

        public void Report(T value) => _handleProgress(value);
    }
}