using System;

namespace Gress.Completable;

/// <summary>
/// Progress handler with explicit completion feedback.
/// </summary>
public interface ICompletableProgress<in T> : IProgress<T>
{
    /// <summary>
    /// Reports overall completion of an operation.
    /// </summary>
    void ReportCompletion();
}