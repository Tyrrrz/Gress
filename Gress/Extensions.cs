using System;
using System.Collections.Generic;
using System.Linq;

namespace Gress
{
    /// <summary>
    /// Extensions for <see cref="Gress"/>.
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Wraps the specified progress abstraction in a progress manager.
        /// </summary>
        public static IProgressManager Wrap(this IProgress<double> progress)
        {
            var progressManager = new ProgressManager();

            progressManager.PropertyChanged += (sender, args) => progress.Report(progressManager.Progress);

            return progressManager;
        }

        /// <summary>
        /// Creates multiple new operations.
        /// </summary>
        public static IReadOnlyList<IProgressOperation> CreateOperations(this IProgressManager progressManager,
            int count, Func<int, double>? weightSelector = null) =>
            Enumerable.Range(0, count)
                .Select(i => weightSelector?.Invoke(i) ?? 1)
                .Select(progressManager.CreateOperation)
                .ToArray();
    }
}