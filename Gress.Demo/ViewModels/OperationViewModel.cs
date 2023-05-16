using Gress.Demo.ViewModels.Framework;

namespace Gress.Demo.ViewModels;

public class OperationViewModel : ViewModelBase
{
    public double Weight { get; }

    public ProgressContainer<Percentage> Progress { get; } = new();

    public OperationViewModel(double weight) => Weight = weight;
}