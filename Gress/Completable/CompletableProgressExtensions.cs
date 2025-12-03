using System;

namespace Gress.Completable;

/// <summary>
/// Extensions for <see cref="CompletableProgressExtensions" />.
/// </summary>
public static class CompletableProgressExtensions
{
    /// <inheritdoc cref="CompletableProgressExtensions" />
    extension<T>(IProgress<T> progress)
    {
        /// <summary>
        /// Converts a regular progress handler into a progress handler with explicit completion.
        /// </summary>
        public ICompletableProgress<T> ToCompletable(Action reportCompletion) =>
            new DelegateCompletableProgress<T>(progress.Report, reportCompletion);
    }

    /// <inheritdoc cref="CompletableProgressExtensions" />
    extension<T>(ICompletableProgress<T> progress)
    {
        /// <summary>
        /// Wraps the specified completable progress handler in a disposable container.
        /// Disposing the container reports completion on the handler.
        /// </summary>
        public DisposableCompletableProgress<T> ToDisposable() => new(progress);
    }

    /// <inheritdoc cref="CompletableProgressExtensions" />
    extension(ProgressMuxer muxer)
    {
        /// <summary>
        /// Wraps the muxer in a special adapter that disconnects all inputs from the muxer
        /// after they all report completion.
        /// </summary>
        public AutoResetProgressMuxer WithAutoReset() => new(muxer);
    }
}
