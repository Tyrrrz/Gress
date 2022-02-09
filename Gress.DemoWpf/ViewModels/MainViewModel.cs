using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Gress.DemoWpf.ViewModels.Framework;
using Gress.Specialized;

namespace Gress.DemoWpf.ViewModels;

public class MainViewModel : ViewModelBase
{
    public ProgressTerminal<Percentage> ProgressTerminal { get; } = new();

    public CompletableProgressMuxer ProgressMuxer { get; }

    public ObservableCollection<TaskViewModel> Tasks { get; } = new();

    // Commands
    public RelayCommand<double> StartTaskCommand { get; }

    public MainViewModel()
    {
        ProgressMuxer = ProgressTerminal.CreateMuxer().CreateCompletableProgressMuxer();

        // Commands
        StartTaskCommand = new RelayCommand<double>(StartTask);
    }

    // Start a task that simulates some work and reports progress
    public async void StartTask(double weight)
    {
        using var progress = ProgressMuxer.CreateInput(weight);

        var task = new TaskViewModel(weight);
        var merged = progress.Merge(task.ProgressTerminal);

        Tasks.Add(task);

        for (var i = 1; i <= 100; i++)
        {
            await Task.Delay(TimeSpan.FromSeconds(0.1));
            merged.Report(Percentage.FromValue(i));
        }

        Tasks.Remove(task);
    }
}