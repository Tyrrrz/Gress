using System.Collections.Generic;
using System.ComponentModel;

namespace Gress
{
    /// <summary>
    /// Manages and keeps track of <see cref="IProgressOperation"/>s.
    /// </summary>
    public interface IProgressManager : INotifyPropertyChanged
    {
        /// <summary>
        /// Current progress aggregated from all operations, with weight taken into account.
        /// </summary>
        double Progress { get; }

        /// <summary>
        /// Whether there are any active (not completed) operations.
        /// </summary>
        bool IsActive { get; }

        /// <summary>
        /// Gets the list of all active (not completed) operations.
        /// </summary>
        IReadOnlyList<IProgressOperation> GetOperations();

        /// <summary>
        /// Creates a new operation.
        /// The weight parameter specifies how much the progress of an individual operation will affect the total aggregated progress in comparison to other operations.
        /// </summary>
        IProgressOperation CreateOperation(double weight = 1);
    }
}