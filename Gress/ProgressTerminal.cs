using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Gress;

/// <summary>
/// Progress handler that persists the last reported progress value.
/// </summary>
public partial class ProgressTerminal<T> : IProgress<T>
{
    private readonly IProgress<T> _target;

    /// <summary>
    /// Last reported progress value.
    /// </summary>
    public T Progress { get; private set; } = default!;

    /// <summary>
    /// Initializes an instance of <see cref="ProgressTerminal{T}"/>.
    /// </summary>
    public ProgressTerminal() =>
        // Proxy through Progress<T> to reuse its SynchronizationContext behavior
        _target = new Progress<T>(p => Progress = p);

    /// <inheritdoc />
    public void Report(T value) => _target.Report(value);
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