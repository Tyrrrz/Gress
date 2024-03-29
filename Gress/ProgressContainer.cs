﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Gress;

/// <summary>
/// Terminal progress handler that records the last reported progress update.
/// </summary>
public partial class ProgressContainer<T>(T initial) : IProgress<T>
{
    /// <summary>
    /// Initializes an instance of <see cref="ProgressContainer{T}" />.
    /// </summary>
    /// <remarks>
    /// If <typeparamref name="T" /> is a reference type, the initial value of the
    /// <see cref="Current"/> property will be <c>null</c>.
    /// Consider using the other constructor overload to provide a non-null initial value.
    /// </remarks>
    public ProgressContainer()
        : this(default!) { }

    /// <summary>
    /// Last reported progress update.
    /// </summary>
    /// <remarks>
    /// If this property is accessed before any progress has been reported,
    /// it will evaluate to the initial value provided by the constructor.
    /// </remarks>
    public T Current
    {
        get => initial;
        private set
        {
            if (EqualityComparer<T>.Default.Equals(value, initial))
                return;

            initial = value;
            OnPropertyChanged();
        }
    }

    /// <inheritdoc />
    public void Report(T value) => Current = value;
}

public partial class ProgressContainer<T> : INotifyPropertyChanged
{
    private event PropertyChangedEventHandler? PropertyChanged;

    event PropertyChangedEventHandler? INotifyPropertyChanged.PropertyChanged
    {
        add => PropertyChanged += value;
        remove => PropertyChanged -= value;
    }

    // Instrumented automatically by Fody
    // ReSharper disable once UnusedMember.Local
    private void OnPropertyChanged([CallerMemberName] string? propertyName = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
