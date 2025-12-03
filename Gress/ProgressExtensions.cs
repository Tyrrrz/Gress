using System;
using System.Collections.Generic;
using System.Threading;
using Gress.Utils;

namespace Gress;

/// <summary>
/// Extensions for <see cref="IProgress{T}" />.
/// </summary>
public static class ProgressExtensions
{
    /// <inheritdoc cref="ProgressExtensions" />
    extension<T>(IProgress<T> progress)
    {
        /// <summary>
        /// Projects progress updates into a different shape.
        /// </summary>
        public IProgress<TTransformed> WithTransform<TTransformed>(Func<TTransformed, T> map) =>
            new DelegateProgress<TTransformed>(p => progress.Report(map(p)));

        /// <summary>
        /// Projects progress updates into a different shape.
        /// </summary>
        public IProgress<T> WithTransform(Func<T, T> map) => progress.WithTransform<T, T>(map);

        /// <summary>
        /// Filters progress updates based on the specified predicate.
        /// </summary>
        public IProgress<T> WithFilter(Func<T, bool> shouldReport) =>
            new DelegateProgress<T>(p =>
            {
                if (shouldReport(p))
                    progress.Report(p);
            });

        /// <summary>
        /// Filters out consecutive progress updates with the same value of the specified key.
        /// </summary>
        public IProgress<T> WithDeduplication<TKey>(
            Func<T, TKey> getKey,
            IEqualityComparer<TKey>? comparer = null
        )
        {
            var syncRoot = new Lock();
            var actualComparer = comparer ?? EqualityComparer<TKey>.Default;
            var lastValueBox = new Box<TKey>();

            return new DelegateProgress<T>(p =>
            {
                using (syncRoot.EnterScope())
                {
                    var value = getKey(p);

                    if (
                        lastValueBox.TryOpen(out var lastValue)
                        && actualComparer.Equals(lastValue, value)
                    )
                    {
                        return;
                    }

                    progress.Report(p);
                    lastValueBox.Store(value);
                }
            });
        }

        /// <summary>
        /// Filters out consecutive progress updates with the same value.
        /// </summary>
        public IProgress<T> WithDeduplication(IEqualityComparer<T>? comparer = null) =>
            progress.WithDeduplication(p => p, comparer);

        /// <summary>
        /// Filters out progress updates that arrive out of order.
        /// </summary>
        public IProgress<T> WithOrdering(IComparer<T>? comparer = null)
        {
            var syncRoot = new Lock();
            var actualComparer = comparer ?? Comparer<T>.Default;
            var lastValueBox = new Box<T>();

            return new DelegateProgress<T>(p =>
            {
                using (syncRoot.EnterScope())
                {
                    if (
                        lastValueBox.TryOpen(out var lastValue)
                        && actualComparer.Compare(lastValue, p) > 0
                    )
                    {
                        return;
                    }

                    progress.Report(p);
                    lastValueBox.Store(p);
                }
            });
        }

        /// <summary>
        /// Merges two progress handlers into one.
        /// </summary>
        public IProgress<T> Merge(IProgress<T> otherProgress) =>
            new DelegateProgress<T>(p =>
            {
                progress.Report(p);
                otherProgress.Report(p);
            });

        /// <summary>
        /// Converts the specified progress handler into a <see cref="Percentage" />-based progress handler.
        /// </summary>
        public IProgress<Percentage> ToPercentageBased(Func<Percentage, T> map) =>
            progress.WithTransform(map);
    }

    /// <inheritdoc cref="ProgressExtensions" />
    extension(IProgress<double> progress)
    {
        /// <summary>
        /// Converts the specified <see cref="double" />-based progress handler into a
        /// <see cref="Percentage" />-based progress handler.
        /// Parameter <paramref name="asFraction" /> specifies whether the percentage-based
        /// progress is reported in its decimal form (true) or in percentage form (false).
        /// </summary>
        public IProgress<Percentage> ToPercentageBased(bool asFraction = true) =>
            asFraction
                ? progress.ToPercentageBased(p => p.Fraction)
                : progress.ToPercentageBased(p => p.Value);
    }

    /// <inheritdoc cref="ProgressExtensions" />
    extension(IProgress<int> progress)
    {
        /// <summary>
        /// Converts the specified <see cref="int" />-based progress handler into a
        /// <see cref="Percentage" />-based progress handler.
        /// </summary>
        public IProgress<Percentage> ToPercentageBased() =>
            progress.ToPercentageBased(p => (int)p.Value);
    }

    /// <inheritdoc cref="ProgressExtensions" />
    extension(IProgress<Percentage> progress)
    {
        /// <summary>
        /// Converts the specified <see cref="Percentage" />-based progress handler into a
        /// <see cref="double" />-based progress handler.
        /// Parameter <paramref name="asFraction" /> specifies whether the percentage-based
        /// progress is reported in its decimal form (true) or in percentage form (false).
        /// </summary>
        public IProgress<double> ToDoubleBased(bool asFraction = true) =>
            asFraction
                ? progress.WithTransform((double p) => Percentage.FromFraction(p))
                : progress.WithTransform((double p) => Percentage.FromValue(p));

        /// <summary>
        /// Converts the specified <see cref="Percentage" />-based progress handler into an
        /// <see cref="int" />-based progress handler.
        /// </summary>
        public IProgress<int> ToInt32Based() =>
            progress.WithTransform((int p) => Percentage.FromValue(p));

        /// <summary>
        /// Creates a muxer for the specified progress handler, allowing it to aggregate
        /// reports from multiple sources.
        /// </summary>
        public ProgressMuxer CreateMuxer() => new(progress);
    }

    /// <inheritdoc cref="ProgressExtensions" />
    extension<T>(IReadOnlyList<IProgress<T>> progresses)
    {
        /// <summary>
        /// Merges multiple progress handlers into one.
        /// </summary>
        public IProgress<T> Merge() =>
            new DelegateProgress<T>(p =>
            {
                foreach (var progress in progresses)
                    progress.Report(p);
            });
    }
}
