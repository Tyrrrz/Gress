namespace Gress.Demo.ViewModels;

public class OperationViewModel(double weight) : ViewModelBase
{
    public double Weight { get; } = weight;

    public ProgressContainer<Percentage> Progress { get; } = new();
}
