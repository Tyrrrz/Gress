using System;
using System.Collections.Generic;

namespace Gress;

/// <summary>
/// Aggregates multiple progress reports to a single handler.
/// </summary>
public partial class ProgressMuxer
{
    private readonly object _lock = new();
    private readonly IProgress<Percentage> _target;
    private readonly HashSet<Input> _inputs = new();

    /// <summary>
    /// Initializes an instance of <see cref="ProgressMuxer"/>.
    /// </summary>
    public ProgressMuxer(IProgress<Percentage> target) => _target = target;

    private void ReportAggregatedProgress()
    {
        lock (_lock)
        {
            var weightedSum = 0.0;
            var weightedMax = 0.0;

            foreach (var input in _inputs)
            {
                weightedSum += input.Weight * input.Progress.Fraction;
                weightedMax += input.Weight * 1.0;
            }

            _target.Report(
                Percentage.FromFraction(weightedSum / weightedMax)
            );
        }
    }

    /// <summary>
    /// Creates a progress handler that reports progress to this muxer.
    /// Specified weight determines the priority of this handler relative to other
    /// handlers linked to this muxer. Progress reported by a handler with higher
    /// weight influences the final progress to a greater degree.
    /// </summary>
    public IProgress<Percentage> CreateInput(double weight = 1.0)
    {
        if (weight <= 0)
            throw new ArgumentOutOfRangeException(nameof(weight), "Weight must be positive.");

        lock (_lock)
        {
            var input = new Input(this, weight);
            _inputs.Add(input);

            ReportAggregatedProgress();

            return input;
        }
    }

    /// <summary>
    /// Removes all progress handlers from this muxer.
    /// </summary>
    internal void ClearInputs()
    {
        lock (_lock)
        {
            _inputs.Clear();
        }
    }
}

public partial class ProgressMuxer
{
    private class Input : IProgress<Percentage>
    {
        private readonly ProgressMuxer _muxer;

        public double Weight { get; }

        public Percentage Progress { get; private set; }

        public Input(ProgressMuxer muxer, double weight)
        {
            _muxer = muxer;
            Weight = weight;
        }

        public void Report(Percentage value)
        {
            Progress = value;

            // Trigger progress update, if this input is still in the muxer
            if (_muxer._inputs.Contains(this))
                _muxer.ReportAggregatedProgress();
        }
    }
}

/// <summary>
/// Extensions for <see cref="ProgressMuxer"/>.
/// </summary>
public static class ProgressMuxerExtensions
{
    /// <summary>
    /// Creates a muxer for the specified progress handler, allowing it to aggregate
    /// reports from multiple sources.
    /// </summary>
    public static ProgressMuxer CreateMuxer(this IProgress<Percentage> progress) => new(progress);
}