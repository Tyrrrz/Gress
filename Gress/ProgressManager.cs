using System.Collections.ObjectModel;
using System.Linq;

namespace Gress;
// Property changed notifications are implemented by PropertyChanged.Fody

/// <summary>
/// Default implementation of <see cref="IProgressManager"/>.
/// </summary>
public class ProgressManager : PropertyChangedBase, IProgressManager
{
    private readonly object _lock = new();
    private readonly ObservableCollection<IProgressOperation> _operations = new();

    /// <inheritdoc />
    public ReadOnlyObservableCollection<IProgressOperation> Operations { get; }

    /// <inheritdoc />
    public bool IsActive { get; private set; }

    /// <inheritdoc />
    public double Progress { get; private set; }

    /// <summary>
    /// Initializes an instance of <see cref="ProgressManager"/>.
    /// </summary>
    public ProgressManager()
    {
        Operations = new ReadOnlyObservableCollection<IProgressOperation>(_operations);
    }

    /// <summary>
    /// Refreshes the current state of this progress manager based on the aggregated state of its individual operations.
    /// </summary>
    private void Refresh()
    {
        lock (_lock)
        {
            // If there are no operations or all operations have completed - clear the list and reset progress
            if (_operations.All(o => o.IsCompleted))
            {
                // Clear list
                _operations.Clear();

                // Update properties
                Progress = 0;
                IsActive = false;
            }
            // Otherwise - calculate aggregated progress
            else
            {
                // Calculate current and maximum weighted progress sum
                var weightedProgressSum = 0.0;
                var weightedProgressMax = 0.0;
                foreach (var operation in _operations)
                {
                    weightedProgressSum += operation.Progress * operation.Weight;
                    weightedProgressMax += 1.0 * operation.Weight;
                }

                // Update properties
                Progress = weightedProgressSum / weightedProgressMax;
                IsActive = true;
            }
        }
    }

    /// <inheritdoc />
    public IProgressOperation CreateOperation(double weight = 1)
    {
        lock (_lock)
        {
            // Create operation
            var operation = new ProgressOperation(weight);

            // Wire property changed event to refresh
            operation.PropertyChanged += (_, _) => Refresh();

            // Add operation to the list
            _operations.Add(operation);

            // Perform refresh
            Refresh();

            return operation;
        }
    }
}