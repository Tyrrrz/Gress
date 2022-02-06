using System;
using System.Collections.Generic;
using Gress.Utils;

namespace Gress;

/// <summary>
/// Extensions for <see cref="IProgress{T}"/>.
/// </summary>
public static class ProgressExtensions
{
    /// <summary>
    /// Projects each progress report into a new form.
    /// </summary>
    public static IProgress<TResult> Select<TResult, TOriginal>(
        this IProgress<TOriginal> progress,
        Func<TResult, TOriginal> map) =>
        new DelegateProgress<TResult>(p => progress.Report(map(p)));

    /// <summary>
    /// Filters progress reports based on the specified predicate.
    /// </summary>
    public static IProgress<T> Where<T>(this IProgress<T> progress, Func<T, bool> shouldReport) =>
        new DelegateProgress<T>(p =>
        {
            if (shouldReport(p))
                progress.Report(p);
        });

    /// <summary>
    /// Filters out progress reports with duplicate values of the specified key.
    /// </summary>
    public static IProgress<T> DistinctBy<T, TKey>(
        this IProgress<T> progress,
        Func<T, TKey> getKey,
        IEqualityComparer<TKey>? comparer = null)
    {
        var set = new HashSet<TKey>(comparer ?? EqualityComparer<TKey>.Default);
        return new DelegateProgress<T>(p =>
        {
            if (set.Add(getKey(p)))
                progress.Report(p);
        });
    }

    /// <summary>
    /// Filters out duplicate progress reports.
    /// </summary>
    public static IProgress<T> Distinct<T>(this IProgress<T> progress, IEqualityComparer<T>? comparer = null) =>
        progress.DistinctBy(p => p, comparer);

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
    /// Converts the specified <see cref="double"/>-based progress handler into a
    /// <see cref="Percentage"/>-based progress handler.
    /// Parameter <paramref name="asFraction"/> specifies whether the percentage-based
    /// progress is reported in its decimal form (true) or in percentage form (false).
    /// </summary>
    public static IProgress<Percentage> ToPercentageBased(this IProgress<double> progress, bool asFraction = true) =>
        asFraction
            ? progress.Select((Percentage p) => p.Fraction)
            : progress.Select((Percentage p) => p.Value);

    /// <summary>
    /// Converts the specified <see cref="int"/>-based progress handler into a
    /// <see cref="Percentage"/>-based progress handler.
    /// </summary>
    public static IProgress<Percentage> ToPercentageBased(this IProgress<int> progress) =>
        progress.Select((Percentage p) => (int)p.Value);

    /// <summary>
    /// Converts the specified <see cref="Percentage"/>-based progress handler into a
    /// <see cref="double"/>-based progress handler.
    /// Parameter <paramref name="asFraction"/> specifies whether the percentage-based
    /// progress is reported in its decimal form (true) or in percentage form (false).
    /// </summary>
    public static IProgress<double> ToDoubleBased(this IProgress<Percentage> progress, bool asFraction = true) =>
        asFraction
            ? progress.Select((double p) => Percentage.FromFraction(p))
            : progress.Select((double p) => Percentage.FromValue(p));

    /// <summary>
    /// Converts the specified <see cref="Percentage"/>-based progress handler into an
    /// <see cref="int"/>-based progress handler.
    /// </summary>
    public static IProgress<int> ToInt32Based(this IProgress<Percentage> progress) =>
        progress.Select((int p) => Percentage.FromValue(p));
}