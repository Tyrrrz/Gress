using System;
using System.Collections.Generic;
using Gress.Utils;

namespace Gress;

/// <summary>
/// Extensions for <see cref="IProgress{T}" />.
/// </summary>
public static class ProgressExtensions
{
    /// <summary>
    /// Projects progress updates into a different shape.
    /// </summary>
    public static IProgress<TTransformed> WithTransform<TOriginal, TTransformed>(
        this IProgress<TOriginal> progress,
        Func<TTransformed, TOriginal> map) =>
        new DelegateProgress<TTransformed>(p => progress.Report(map(p)));

    /// <summary>
    /// Projects progress updates into a different shape.
    /// </summary>
    public static IProgress<T> WithTransform<T>(this IProgress<T> progress, Func<T, T> map) =>
        progress.WithTransform<T, T>(map);

    /// <summary>
    /// Filters progress updates based on the specified predicate.
    /// </summary>
    public static IProgress<T> WithFilter<T>(this IProgress<T> progress, Func<T, bool> shouldReport) =>
        new DelegateProgress<T>(p =>
        {
            if (shouldReport(p))
                progress.Report(p);
        });

    /// <summary>
    /// Filters out consecutive progress updates with the same value of the specified key.
    /// </summary>
    public static IProgress<T> WithDeduplication<T, TKey>(
        this IProgress<T> progress,
        Func<T, TKey> getKey,
        IEqualityComparer<TKey>? comparer = null)
    {
        var syncRoot = new object();
        var actualComparer = comparer ?? EqualityComparer<TKey>.Default;
        var lastValueBox = new Box<TKey>();

        return new DelegateProgress<T>(p =>
        {
            lock (syncRoot)
            {
                var value = getKey(p);

                if (lastValueBox.TryOpen(out var lastValue) &&
                    actualComparer.Equals(lastValue, value))
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
    public static IProgress<T> WithDeduplication<T>(
        this IProgress<T> progress,
        IEqualityComparer<T>? comparer = null) =>
        progress.WithDeduplication(p => p, comparer);

    /// <summary>
    /// Filters out progress updates that arrive out of order.
    /// </summary>
    public static IProgress<T> WithOrdering<T>(
        this IProgress<T> progress,
        IComparer<T>? comparer = null)
    {
        var syncRoot = new object();
        var actualComparer = comparer ?? Comparer<T>.Default;
        var lastValueBox = new Box<T>();

        return new DelegateProgress<T>(p =>
        {
            lock (syncRoot)
            {
                if (lastValueBox.TryOpen(out var lastValue) &&
                    actualComparer.Compare(lastValue, p) > 0)
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
    public static IProgress<T> Merge<T>(this IProgress<T> progress, IProgress<T> otherProgress) =>
        new DelegateProgress<T>(p =>
        {
            progress.Report(p);
            otherProgress.Report(p);
        });

    /// <summary>
    /// Merges multiple progress handlers into one.
    /// </summary>
    public static IProgress<T> Merge<T>(this IReadOnlyList<IProgress<T>> progresses) =>
        new DelegateProgress<T>(p =>
        {
            foreach (var progress in progresses)
                progress.Report(p);
        });

    /// <summary>
    /// Converts the specified progress handler into a <see cref="Percentage" />-based progress handler.
    /// </summary>
    public static IProgress<Percentage> ToPercentageBased<T>(this IProgress<T> progress, Func<Percentage, T> map) =>
        progress.WithTransform(map);

    /// <summary>
    /// Converts the specified <see cref="double" />-based progress handler into a
    /// <see cref="Percentage" />-based progress handler.
    /// Parameter <paramref name="asFraction" /> specifies whether the percentage-based
    /// progress is reported in its decimal form (true) or in percentage form (false).
    /// </summary>
    public static IProgress<Percentage> ToPercentageBased(this IProgress<double> progress, bool asFraction = true) =>
        asFraction
            ? progress.ToPercentageBased(p => p.Fraction)
            : progress.ToPercentageBased(p => p.Value);

    /// <summary>
    /// Converts the specified <see cref="int" />-based progress handler into a
    /// <see cref="Percentage" />-based progress handler.
    /// </summary>
    public static IProgress<Percentage> ToPercentageBased(this IProgress<int> progress) =>
        progress.ToPercentageBased(p => (int)p.Value);

    /// <summary>
    /// Converts the specified <see cref="Percentage" />-based progress handler into a
    /// <see cref="double" />-based progress handler.
    /// Parameter <paramref name="asFraction" /> specifies whether the percentage-based
    /// progress is reported in its decimal form (true) or in percentage form (false).
    /// </summary>
    public static IProgress<double> ToDoubleBased(this IProgress<Percentage> progress, bool asFraction = true) =>
        asFraction
            ? progress.WithTransform((double p) => Percentage.FromFraction(p))
            : progress.WithTransform((double p) => Percentage.FromValue(p));

    /// <summary>
    /// Converts the specified <see cref="Percentage" />-based progress handler into an
    /// <see cref="int" />-based progress handler.
    /// </summary>
    public static IProgress<int> ToInt32Based(this IProgress<Percentage> progress) =>
        progress.WithTransform((int p) => Percentage.FromValue(p));

    /// <summary>
    /// Creates a muxer for the specified progress handler, allowing it to aggregate
    /// reports from multiple sources.
    /// </summary>
    public static ProgressMuxer CreateMuxer(this IProgress<Percentage> progress) => new(progress);
}