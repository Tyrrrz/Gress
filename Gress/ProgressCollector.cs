using System;
using System.Collections.Generic;

namespace Gress;

/// <summary>
/// Progress handler that persists all reported progress values in a collection.
/// Useful for testing scenarios.
/// </summary>
public class ProgressCollector<T> : IProgress<T>
{
    private readonly object _lock = new();
    private readonly List<T> _reports = new();

    /// <summary>
    /// Clears the reports.
    /// </summary>
    public void Clear()
    {
        lock (_lock)
            _reports.Clear();
    }

    /// <summary>
    /// Returns the list of values reported so far.
    /// </summary>
    public IReadOnlyList<T> GetReports()
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