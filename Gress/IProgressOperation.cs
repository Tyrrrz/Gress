using System;
using System.ComponentModel;

namespace Gress
{
    /// <summary>
    /// Represents an operation that can report progress.
    /// </summary>
    public interface IProgressOperation : IProgress<double>, INotifyPropertyChanged, IDisposable
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
    }
}