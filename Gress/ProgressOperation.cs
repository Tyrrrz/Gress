using System;
using Gress.Internal;

namespace Gress
{
    // Property changed notifications are implemented by PropertyChanged.Fody

    /// <summary>
    /// Default implementation of <see cref="IProgressOperation"/>.
    /// </summary>
    public class ProgressOperation : PropertyChangedBase, IProgressOperation
    {
        /// <inheritdoc />
        public double Weight { get; }

        /// <inheritdoc />
        public double Progress { get; private set; }

        /// <inheritdoc />
        public bool IsCompleted { get; private set; }

        /// <summary>
        /// Initializes an instance of <see cref="ProgressOperation"/>.
        /// </summary>
        public ProgressOperation(double weight = 1)
        {
            Weight = weight.GuardNotNegative(nameof(weight));
        }

        /// <inheritdoc />
        public void Report(double progress)
        {
            progress.GuardRange(0, 1, nameof(progress));

            // If completed - throw
            if (IsCompleted)
                throw new InvalidOperationException("Cannot report progress on an operation marked as completed.");

            // Set new progress
            Progress = progress;
        }

        /// <inheritdoc />
        public void Dispose()
        {
            IsCompleted = true;
            Progress = 1;
        }
    }
}