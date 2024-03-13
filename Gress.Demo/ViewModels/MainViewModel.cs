using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Gress.Completable;

namespace Gress.Demo.ViewModels;

public class MainViewModel : ObservableObject
{
    private readonly AutoResetProgressMuxer _progressMuxer;

    public ProgressContainer<Percentage> Progress { get; } = new();

    public ObservableCollection<OperationViewModel> Operations { get; } = [];

    public RelayCommand<double> EnqueueOperationCommand { get; }

    public MainViewModel()
    {
        _progressMuxer = Progress.CreateMuxer().WithAutoReset();
        EnqueueOperationCommand = new RelayCommand<double>(EnqueueOperation);
    }

    // Start an operation that simulates some work and reports progress
    public void EnqueueOperation(double weight) =>
        Task.Run(async () =>
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
        });
}
