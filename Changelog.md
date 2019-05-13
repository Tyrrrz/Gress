### v1.1 (13-May-2019)

- Replaced `IProgressManager.GetOperations()` with `IProgressManager.Operations`.

### v1.0.2 (09-Feb-2019)

- Changed `CreateOperations()` extension so that it returns an instance of `IReadOnlyList<IProgressOperation>` now.

### v1.0.1 (04-Feb-2019)

- Re-arranged interfaces so that `IProgressOperation` inherits from `IProgress<double>` and `INotifyPropertyChanged`, while `IProgressManager` inherits from `INotifyPropertyChanged`.