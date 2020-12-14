using System;
using System.Threading.Tasks;
using System.Windows.Data;
using Gress.DemoWpf.ViewModels.Framework;

namespace Gress.DemoWpf.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private readonly object _lock = new();

        public IProgressManager ProgressManager { get; } = new ProgressManager();

        // Commands
        public RelayCommand<double> StartOperationCommand { get; }

        public MainViewModel()
        {
            // Enable collection synchronization so that UI can be bound to Operations collection which is changed in non-UI thread
            BindingOperations.EnableCollectionSynchronization(ProgressManager.Operations, _lock);

            // Commands
            StartOperationCommand = new RelayCommand<double>(StartOperation);
        }

        public void StartOperation(double weight)
        {
            // Start a task that simulates some work and reports progress
            Task.Run(async () =>
            {
                using var operation = ProgressManager.CreateOperation(weight);

                for (var i = 0; i < 100; i++)
                {
                    // Delay execution to simulate activity
                    await Task.Delay(TimeSpan.FromSeconds(0.1));

                    // Report new progress
                    operation.Report((i + 1) / 100.0);
                }
            });
        }
    }
}