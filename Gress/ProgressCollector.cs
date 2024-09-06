using System;
using System.Collections.Generic;
using System.Threading;

namespace Gress;

/// <summary>
/// Terminal progress handler that stores all reported progress updates in a collection.
/// </summary>
public class ProgressCollector<T> : IProgress<T>
{
    private readonly Lock _lock = new();
    private readonly List<T> _reports = [];

    /// <summary>
    /// Clears the reported progress updates.
    /// </summary>
    public void Reset()
    {
        using (_lock.EnterScope())
            _reports.Clear();
    }

    /// <summary>
    /// Returns the progress updates reported so far.
    /// </summary>
    public IReadOnlyList<T> GetValues()
    {
        using (_lock.EnterScope())
            return _reports.ToArray();
    }

    /// <inheritdoc />
    public void Report(T value)
    {
        using (_lock.EnterScope())
            _reports.Add(value);
    }
}
