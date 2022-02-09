using System;

namespace Gress.Specialized;

/// <summary>
/// Defines a provider for progress updates that additionally signal completion.
/// </summary>
public interface ICompletableProgress<in T> : IProgress<T>, IDisposable
{
}