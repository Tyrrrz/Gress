# Gress

[![Build](https://github.com/Tyrrrz/Gress/workflows/CI/badge.svg?branch=master)](https://github.com/Tyrrrz/Gress/actions)
[![Coverage](https://codecov.io/gh/Tyrrrz/Gress/branch/master/graph/badge.svg)](https://codecov.io/gh/Tyrrrz/Gress)
[![Version](https://img.shields.io/nuget/v/Gress.svg)](https://nuget.org/packages/Gress)
[![Downloads](https://img.shields.io/nuget/dt/Gress.svg)](https://nuget.org/packages/Gress)
[![Donate](https://img.shields.io/badge/donate-$$$-purple.svg)](https://tyrrrz.me/donate)

⚠️ **Project status: maintenance mode** (bug fixes only).

Gress is a library that helps aggregate and report progress from sets of concurrent operations.

## Download

📦 [NuGet](https://nuget.org/packages/Gress): `dotnet add package Gress`

## Screenshots

![demo](.screenshots/demo.png)

## Usage

### Basic example

In order to aggregate progress from multiple operations, you need to initialize an instance of `ProgressManager`.
This object represents a container that persists individual operations along with their state.

After that, you can track an operation by calling `progressManager.CreateOperation()` and using the `operation.Report(...)` method to report progress:

```c#
var progressManager = new ProgressManager();

await Task.WhenAll(
    // Start 5 parallel tasks
    Enumerable.Range(0, 5).Select(async _ =>
    {
        // Create an operation for each task
        using (var operation = progressManager.CreateOperation())
        {
            for (var i = 0; i < 100; i++)
            {
                // Do some work
                await Task.Delay(200);
                
                // Report progress for each individual operation
                operation.Report((i + 1) / 100);
                
                // Print total progress
                Console.WriteLine($"Total progress: {progressManager.Progress:P2}");
            }
        }
    }
);
```

The code above creates 5 parallel operations and reports progress on each of them separately.
Total progress is aggregated automatically and can be evaluated at any point by accessing `progressManager.Progress`.

Note that `ProgressOperation` implements `IDisposable` so you will want to encapsulate it in a `using` statement.
Disposing an operation marks it as complete and prevents it from reporting progress in the future.

### Using weight

An operation may be created with custom weight, which specifies how much its own progress affects the total progress relative to other operations:

```c#
var progressManager = new ProgressManager();

// Create a light operation
var operationLight = progressManager.CreateOperation(1);

// Create a heavy operation
var operationHeavy = progressManager.CreateOperation(5);

// Report progress on both
operationLight.Report(0.8); // 80%
operationHeavy.Report(0.4); // 40%

// Print total progress
Console.WriteLine($"{progressManager.Progress:P2}"); // 46.67%
```

### Pre-creating operations

You can create multiple operations at once by calling the `CreateOperations(...)` method.
This can be useful if you have a set of operations that are executed in sequence and you want `ProgressManager` to properly account for them when calculating aggregated progress.

```c#
var manager = new ProgressManager();

// Create 2 operations
var operations = manager.CreateOperations(2);

// Perform first operation (from 0% to 50%)
using (var operation = operations[0])
    ExecuteLongRunningProcess(operation);

// Perform second operation (from 50% to 100%)
using (var operation = operations[1])
    ExecuteOtherLongRunningProcess(operation);
```

### Integrating with other code

The standard guideline for reporting progress in .NET is to use an instance of [`System.IProgress<T>`](https://docs.microsoft.com/en-us/dotnet/api/system.iprogress-1).
Since Gress represents progress as `double`, the `ProgressOperation` class also implements `IProgress<double>` to make integration easier.

If you have existing code or 3rd-party libraries that accept an instance of `IProgress<double>` then you can simply pass `ProgressOperation` directly.
Here's an example that uses [YoutubeExplode](https://github.com/Tyrrrz/YoutubeExplode) and Gress to download a YouTube video while reporting progress.

```c#
var youtube = new YoutubeClient();
var progressManager = new ProgressManager();

using (var operation = progressManager.CreateOperation())
{
    var streamManifest = await youtube.Videos.Streams.GetManifestAsync("u_yIGGhubZs");
    var streamInfo = streamManifest.GetMuxed().GetWithHighestVideoQuality();

    await youtube.Videos.Streams.DownloadAsync(
        streamInfo,
        $"video.{streamInfo.Container}",
        // This method expects IProgress<double> so we just pass ProgressOperation directly
        operation
    );
}
```

### Integrating with XAML

Both `ProgressManager` and `ProgressOperation` implement `INotifyPropertyChanged` so corresponding bound properties will be automatically refreshed every time the progress changes.

As an example, here's how you can use `ProgressManager` in a WPF application:

```c#
public class MainViewModel
{
    public IProgressManager ProgressManager { get; } = new ProgressManager();
    public ICommand ExecuteOperationCommand { get; }

    public MainViewModel()
    {
        ExecuteOperationCommand = new RelayCommand(ExecuteOperation);
    }

    public async void ExecuteOperation()
    {
        using (var operation = ProgressManager.CreateOperation())
        {
            for (var i = 0; i < 100; i++)
            {
                await Task.Delay(200);
                operation.Report((i + 1) / 100);
            }
        }
    }
}
```

```xml
<Window
    x:Class="MainWindow"
    xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    xmlns:d="http://schemas.microsoft.com/expression/blend/2008"
    d:DataContext="{d:DesignInstance Type=MainViewModel}">
    <StackPanel>
        <!-- Button that starts a new operation -->
        <Button
            Margin="32" 
            Content="Execute"
            Command="{Binding ExecuteOperationCommand}" />

        <!-- Progress bar that shows total progress -->
        <ProgressBar
            Margin="32"
            Height="10"
            Minimum="0"
            Maximum="1"
            Value="{Binding ProgressManager.Progress, Mode=OneWay}" />
    </StackPanel>
</Window>
```

You can also check out `Gress.DemoWpf` project for a slightly more involved example.
