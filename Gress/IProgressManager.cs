using System.Collections.ObjectModel;
using System.ComponentModel;

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
}