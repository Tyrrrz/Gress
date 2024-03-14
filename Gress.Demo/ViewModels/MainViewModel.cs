using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Gress.Completable;

namespace Gress.Demo.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private readonly AutoResetProgressMuxer _progressMuxer;

    public ProgressContainer<Percentage> Progress { get; } = new();

    public ObservableCollection<OperationViewModel> Operations { get; } = [];

    public MainViewModel() => _progressMuxer = Progress.CreateMuxer().WithAutoReset();

    // Start an operation that simulates some work and reports progress
    [RelayCommand(AllowConcurrentExecutions = true)]
    private async Task PerformWorkAsync(double weight)
    {
        using var progress = _progressMuxer.CreateInput(weight).ToDisposable();

        var operation = new OperationViewModel(weight);
        var mergedProgress = progress.Merge(operation.Progress);

        Operations.Add(operation);

        for (var i = 1; i <= 100; i++)
        {
            // Simulate work
            await Task.Delay(TimeSpan.FromSeconds(Random.Shared.Next(1, 5) / 10.0));

            // Report progress as a value in the 0..100 range
            mergedProgress.Report(Percentage.FromValue(i));
        }

        Operations.Remove(operation);
    }
}
