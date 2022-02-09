using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Gress;

/// <summary>
/// Progress handler that records the last reported progress value.
/// </summary>
public partial class ProgressTerminal<T> : IProgress<T>
{
    /// <summary>
    /// Last reported progress value.
    /// </summary>
    public T Progress { get; private set; } = default!;

    /// <inheritdoc />
    public void Report(T value) => Progress = value;
}

public partial class ProgressTerminal<T> : INotifyPropertyChanged
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