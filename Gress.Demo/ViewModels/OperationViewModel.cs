using CommunityToolkit.Mvvm.ComponentModel;

namespace Gress.Demo.ViewModels;

public class OperationViewModel(double weight) : ObservableObject
{
    public double Weight { get; } = weight;

    public ProgressContainer<Percentage> Progress { get; } = new();
}
