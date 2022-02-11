using Gress.DemoWpf.ViewModels.Framework;

namespace Gress.DemoWpf.ViewModels;

public class OperationViewModel : ViewModelBase
{
    public double Weight { get; }

    public ProgressContainer<Percentage> Progress { get; } = new();

    public OperationViewModel(double weight) => Weight = weight;
}