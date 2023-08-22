using System;
using System.Collections.Generic;

namespace Gress;

/// <summary>
/// Terminal progress handler that stores all reported progress updates in a collection.
/// </summary>
public class ProgressCollector<T> : IProgress<T>
{
    private readonly object _lock = new();
    private readonly List<T> _reports = new();

    /// <summary>
    /// Clears the reported progress updates.
    /// </summary>
    public void Reset()
    {
        lock (_lock)
            _reports.Clear();
    }

    /// <summary>
    /// Returns the progress updates reported so far.
    /// </summary>
    public IReadOnlyList<T> GetValues()
    {
        lock (_lock)
            return _reports.ToArray();
    }

    /// <inheritdoc />
    public void Report(T value)
    {
        lock (_lock)
            _reports.Add(value);
    }
}
