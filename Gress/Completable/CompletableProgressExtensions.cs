﻿namespace Gress.Completable;

/// <summary>
/// Extensions for <see cref="CompletableProgressExtensions"/>.
/// </summary>
public static class CompletableProgressExtensions
{
    /// <summary>
    /// Wraps the specified completable progress handler in a disposable container.
    /// Disposing the container reports completion on the handler.
    /// </summary>
    public static DisposableCompletableProgress<T> AsDisposable<T>(this ICompletableProgress<T> progress) =>
        new(progress);

    /// <summary>
    /// Wraps the muxer in a special adapter that disconnects all inputs from the muxer
    /// after each of them has reported completion.
    /// </summary>
    public static AutoResetProgressMuxer WithAutoReset(this ProgressMuxer muxer) => new(muxer);
}