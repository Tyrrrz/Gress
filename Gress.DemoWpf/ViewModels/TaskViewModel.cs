using Gress.DemoWpf.ViewModels.Framework;

namespace Gress.DemoWpf.ViewModels;

public class TaskViewModel : ViewModelBase
{
    public double Weight { get; }

    public ProgressTerminal<Percentage> ProgressTerminal { get; } = new();

    public TaskViewModel(double weight) => Weight = weight;
}