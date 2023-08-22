using System;

namespace Gress.Completable;

/// <summary>
/// Aggregates multiple progress updates into a single handler.
/// Resets itself once all inputs report completion.
/// </summary>
public partial class AutoResetProgressMuxer
{
    private readonly object _lock = new();
    private readonly ProgressMuxer _muxer;

    private int _pendingInputs;

    /// <summary>
    /// Initializes an instance of <see cref="AutoResetProgressMuxer" />.
    /// </summary>
    public AutoResetProgressMuxer(ProgressMuxer muxer) => _muxer = muxer;

    /// <summary>
    /// Creates a progress handler that reports progress to this muxer.
    /// Specified weight determines the priority of this handler relative to other
    /// handlers connected to this muxer. Progress reported on a handler with higher
    /// weight influences the final progress to a greater degree.
    /// </summary>
    /// <remarks>
    /// Returned progress handler can report completion. Once all linked handlers
    /// report completion, the progress is reset, and existing handlers are disconnected.
    /// </remarks>
    public ICompletableProgress<Percentage> CreateInput(double weight = 1.0)
    {
        lock (_lock)
        {
            var item = new Item(this, _muxer.CreateInput(weight));

            // Make sure the item was created successfully before incrementing
            _pendingInputs++;

            return item;
        }
    }
}

public partial class AutoResetProgressMuxer
{
    private class Item : ICompletableProgress<Percentage>
    {
        private readonly AutoResetProgressMuxer _parent;
        private readonly IProgress<Percentage> _target;

        public Item(AutoResetProgressMuxer parent, IProgress<Percentage> target)
        {
            _parent = parent;
            _target = target;
        }

        public void Report(Percentage value)
        {
            lock (_parent._lock)
            {
                _target.Report(value);
            }
        }

        public void ReportCompletion()
        {
            lock (_parent._lock)
            {
                if (--_parent._pendingInputs <= 0)
                    _parent._muxer.Reset();
            }
        }
    }
}
