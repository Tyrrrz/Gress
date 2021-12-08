﻿using System;

namespace Gress;
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
        Weight = weight > 0 ? weight : throw new ArgumentException("Weight must not be negative.", nameof(weight));
    }

    /// <inheritdoc />
    public void Report(double progress)
    {
        if (progress < 0 || progress > 1)
            throw new ArgumentException("Progress must be between 0 and 1.", nameof(progress));

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