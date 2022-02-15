### v2.0.1 (15-Feb-2022)

- Made `DelegateProgress<T>` and `DelegateCompletableProgress<T>` public. You can use `DelegateProgress<T>` as a substitute for `Progress<T>` when you don't need scheduling based on synchronization context (e.g. in console applications). Both classes can be also used to implement your own extension methods similar to the likes of `WithTransform(...)` and `WithFilter(...)`.

### v2.0 (13-Feb-2022)

- Completely reworked the library from the ground up.
- Split up `ProgressManager` into a set of modular and composable utilities.
- Added extension methods for composing and wrapping existing progress handlers.
- Added the `Percentage` type for an unambiguous representation of progress.

Refer to the [migration guide](https://github.com/Tyrrrz/Gress/wiki/Migration-guide-(from-v1.2-to-v2.0)) to see how you can update your old code to work with Gress v3.0. Also check out the new readme to see the complete list of new features.

### v1.2 (19-Apr-2020)

- Added `IProgress<double>.Wrap()` extension method that wraps any existing progress as `ProgressManager`.

### v1.1.1 (15-Jun-2019)

- Added locking to `ProgressManager`.

### v1.1 (13-May-2019)

- Replaced `IProgressManager.GetOperations()` with `IProgressManager.Operations`.

### v1.0.2 (09-Feb-2019)

- Changed `CreateOperations()` extension so that it returns an instance of `IReadOnlyList<IProgressOperation>` now.

### v1.0.1 (04-Feb-2019)

- Re-arranged interfaces so that `IProgressOperation` inherits from `IProgress<double>` and `INotifyPropertyChanged`, while `IProgressManager` inherits from `INotifyPropertyChanged`.