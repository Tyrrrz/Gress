using System;

namespace Gress
{
    /// <summary>
    /// Represents an operation that can report progress.
    /// </summary>
    public interface IProgressOperation : IDisposable
    {
        /// <summary>
        /// Weight of this operation.
        /// </summary>
        double Weight { get; }

        /// <summary>
        /// Current progress of this operation.
        /// </summary>
        double Progress { get; }

        /// <summary>
        /// Whether this operation has completed.
        /// </summary>
        bool IsCompleted { get; }

        /// <summary>
        /// Reports new progress.
        /// </summary>
        void Report(double progress);
    }
}