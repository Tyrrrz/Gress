using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Gress.Completable;
using Gress.DemoWpf.ViewModels.Framework;

namespace Gress.DemoWpf.ViewModels;

public class MainViewModel : ViewModelBase
{
    private readonly AutoResetProgressMuxer _progressMuxer;

    public ProgressContainer<Percentage> Progress { get; } = new();

    public ObservableCollection<OperationViewModel> Operations { get; } = new();

    public RelayCommand<double> StartOperationCommand { get; }

    public MainViewModel()
    {
        _progressMuxer = Progress.CreateMuxer().WithAutoReset();
        StartOperationCommand = new RelayCommand<double>(StartOperation);
    }

    // Start an operation that simulates some work and reports progress
    public async void StartOperation(double weight)
    {
        using var progress = _progressMuxer.CreateInput(weight).ToDisposable();

        var operation = new OperationViewModel(weight);
        var progressMerged = progress.Merge(operation.Progress);

        Operations.Add(operation);

        for (var i = 1; i <= 100; i++)
        {
            await Task.Delay(TimeSpan.FromSeconds(0.1));
            progressMerged.Report(Percentage.FromValue(i));
        }

        Operations.Remove(operation);
    }
}