using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Gress;

/// <summary>
/// Terminal progress handler that records the last reported progress update.
/// </summary>
public partial class ProgressContainer<T> : IProgress<T>
{
    /// <summary>
    /// Last reported progress update.
    /// </summary>
    /// <remarks>
    /// If this property is accessed before any progress has been reported,
    /// it will evaluate to the default value of <typeparamref name="T" />, which may be <c>null</c>.
    /// </remarks>
    public T Current { get; private set; } = default!;

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