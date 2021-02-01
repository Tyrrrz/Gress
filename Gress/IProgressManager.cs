using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace Gress
{
    /// <summary>
    /// Manages and keeps track of <see cref="IProgressOperation"/>s.
    /// </summary>
    public interface IProgressManager : INotifyPropertyChanged
    {
        /// <summary>
        /// The list of all active (uncompleted) operations.
        /// </summary>
        ReadOnlyObservableCollection<IProgressOperation> Operations { get; }

        /// <summary>
        /// Whether there are any active (uncompleted) operations.
        /// </summary>
        bool IsActive { get; }

        /// <summary>
        /// Current progress aggregated from all operations, with weight taken into account.
        /// </summary>
        double Progress { get; }

        /// <summary>
        /// Creates a new operation.
        /// The weight parameter specifies how much the progress of an individual operation will affect the total aggregated progress in comparison to other operations.
        /// </summary>
        IProgressOperation CreateOperation(double weight = 1);
    }

    /// <summary>
    /// Extensions for <see cref="IProgressManager"/>.
    /// </summary>
    public static class ProgressManagerExtensions
    {
        /// <summary>
        /// Wraps the specified progress abstraction in a progress manager.
        /// </summary>
        public static IProgressManager Wrap(this IProgress<double> progress)
        {
            var progressManager = new ProgressManager();

            progressManager.PropertyChanged += (_, args) =>
            {
                if (string.Equals(args.PropertyName, nameof(ProgressManager.Progress), StringComparison.Ordinal))
                {
                    progress.Report(progressManager.Progress);
                }
            };

            return progressManager;
        }

        /// <summary>
        /// Creates multiple new operations.
        /// </summary>
        public static IReadOnlyList<IProgressOperation> CreateOperations(
            this IProgressManager progressManager,
            int count,
            Func<int, double>? weightSelector = null) =>
            Enumerable.Range(0, count)
                .Select(i => weightSelector?.Invoke(i) ?? 1)
                .Select(progressManager.CreateOperation)
                .ToArray();
    }
}