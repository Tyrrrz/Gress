using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Gress.Completable;
using Gress.Demo.ViewModels.Framework;

namespace Gress.Demo.ViewModels;

public class MainViewModel : ViewModelBase
{
    private readonly AutoResetProgressMuxer _progressMuxer;

    public ProgressContainer<Percentage> Progress { get; } = new();

    public ObservableCollection<OperationViewModel> Operations { get; } = new();

    public RelayCommand<double> PerformWorkCommand { get; }

    public MainViewModel()
    {
        _progressMuxer = Progress.CreateMuxer().WithAutoReset();
        PerformWorkCommand = new AsyncRelayCommand<double>(PerformWorkAsync);
    }

    // Start an operation that simulates some work and reports progress
    public async Task PerformWorkAsync(double weight)
    {
        using var progress = _progressMuxer.CreateInput(weight).ToDisposable();

        var operation = new OperationViewModel(weight);
        var mergedProgress = progress.Merge(operation.Progress);

        Operations.Add(operation);

        for (var i = 1; i <= 100; i++)
        {
            await Task.Delay(TimeSpan.FromSeconds(0.1));
            mergedProgress.Report(Percentage.FromValue(i));
        }

        Operations.Remove(operation);
    }
}