# Gress

[![Build](https://github.com/Tyrrrz/Gress/workflows/main/badge.svg?branch=master)](https://github.com/Tyrrrz/Gress/actions)
[![Coverage](https://codecov.io/gh/Tyrrrz/Gress/branch/master/graph/badge.svg)](https://codecov.io/gh/Tyrrrz/Gress)
[![Version](https://img.shields.io/nuget/v/Gress.svg)](https://nuget.org/packages/Gress)
[![Downloads](https://img.shields.io/nuget/dt/Gress.svg)](https://nuget.org/packages/Gress)
[![Discord](https://img.shields.io/discord/869237470565392384?label=discord)](https://discord.gg/2SUWKFnHSm)
[![Donate](https://img.shields.io/badge/donate-$$$-purple.svg)](https://tyrrrz.me/donate)

✅ **Project status: active**. [What does it mean?](https://github.com/Tyrrrz/.github/blob/master/docs/project-status.md)

**Gress** is a library that builds upon the standard interface for reporting progress in .NET ([`IProgress<T>`](https://docs.microsoft.com/en-us/dotnet/api/system.iprogress-1)) to provide a exhaustive set of tools for tracking and routing progress in your code.
It offers utilities for collecting, transforming, and filtering progress updates, merging and muxing progress handlers, and more.

💬 **If you want to chat, join my [Discord server](https://discord.gg/2SUWKFnHSm)**.

## Download

📦 [NuGet](https://nuget.org/packages/Gress): `dotnet add package Gress`

## Screenshots

![demo](.screenshots/demo.png)

## Usage

### Percentage type

**Gress** provides a universal progress unit -- the `Percentage` type.
Unlike raw `int` and `double` values commonly used with `IProgress<T>`, this type unambiguously represents progress as a portion of work that has been completed so far.

An instance of `Percentage` can be created from either a value or a fraction:

```csharp
using Gress;

// Mapped from value
var fiftyPercent = Percentage.FromValue(50);       // 50%

// Mapped from fractional representation
var twentyPercent = Percentage.FromFraction(0.2);  // 20%
```

Similarly, both value and fraction can be extracted from an initialized `Percentage` by accessing the corresponding properties:

```csharp
using Gress;

var fiftyPercent = Percentage.FromValue(50);

var asValue = fiftyPercent.Value;        // 50.0 (double)
var asFraction = fiftyPercent.Fraction;  // 0.5  (double)
```

You can use `Percentage` as the type argument in `IProgress<T>` handlers to establish a consistent foundation for reporting progress in your code. When interfacing with external methods that already define their own progress handlers, you can also use one of the provided conversion methods to wrap an existing percentage-based handler into a handler of an appropriate type:

```csharp
using Gress;

var progress = new Progress<Percentage>(p => /* ... */);  // IProgress<Percentage>

var progressOfDouble = progress.ToDoubleBased();          // IProgress<double>
var progressOfInt = progress.ToInt32Based();              // IProgress<int>
```

Likewise, there are methods that facilitate conversion in the other direction as well:

```csharp
using Gress;

var progressOfDouble = new Progress<double>(p => /* ... */);       // IProgress<double>
var progressOfInt = new Progress<int>(p => /* ... */);             // IProgress<int>

var progressOfPercentage1 = progressOfDouble.ToPercentageBased();  // IProgress<Percentage>
var progressOfPercentage2 = progressOfInt.ToPercentageBased();     // IProgress<Percentage>
```

> 💡 When converting between percentage-based and double-based handlers, percentages are mapped using their fractional form by default.
To override this behavior and map by value instead, use `ToDoubleBased(false)` and `ToPercentageBased(false)`.

### Composing handlers

Existing progress handlers can be composed into more complex handlers using some of the extension methods that **Gress** offers.
These can be used to easily apply transformations, inject filtering logic, or merge multiple handlers together.

#### Transformation

You can use `WithTransform(...)` method to create a handler that transforms all reported progress values:

```csharp
using Gress;

enum Status { Started, HalfWay, Completed }

var progress = new Progress<Percentage>(p => /* ... */);  // IProgress<Percentage>

// Transform into a progress handler that accepts an enum value and maps
// it into a value of the original type (Percentage in this case)
var progressTransformed = progress.WithTransform((Status s) => s switch
{
    Status.Completed => Percentage.FromValue(100), // 100%
    Status.HalfWay => Percentage.FromValue(50),    // 50%
    _ => Percentage.FromValue(0)                   // 0%
}); // IProgress<Status>

// This effectively reports 50% on the original handler
progressTransformed.Report(Status.HalfWay);
```

A simpler overload of the above method can also be used when transforming between values of the same type:

```csharp
using Gress;

var progress = new Progress<int>(p => /* ... */);              // IProgress<int>

var progressTransformed = progress.WithTransform(p => 5 * p);  // IProgress<int>

// This effectively reports 50 on the original handler
progressTransformed.Report(10);
```

> 💡 Method `WithTransform(...)` bears some resemblance to LINQ's `Select(...)`, however they are not equivalent.
The main difference is that the flow of data in `IProgress<T>` is inverse to that of `IEnumerable<T>`, which means that the transformations in `WithTransform(...)` are applied in the opposite direction.

#### Filtering

You can use `WithFilter(...)` method to create a handler that selectively filters reported progress values:

```csharp
using Gress;

var progress = new Progress<Percentage>(p => /* ... */);             // IProgress<Percentage>

// Filter out values below 10%
var progressFiltered = progress.WithFilter(p => p.Fraction >= 0.1);  // IProgress<Percentage>

// Will not be reported
progressFiltered.Report(Percentage.FromFraction(0.05));

// Will be reported
progressFiltered.Report(Percentage.FromFraction(0.25));
```

#### Deduplication

You can use `WithDeduplication(...)` method to create a handler that filters out consecutive progress reports with the same value:

```csharp
using Gress;

var progress = new Progress<Percentage>(p => /* ... */);    // IProgress<Percentage>

var progressDeduplicated = progress.WithDeduplication();    // IProgress<Percentage>

progressDeduplicated.Report(Percentage.FromFraction(0.1));  // reported
progressDeduplicated.Report(Percentage.FromFraction(0.3));  // reported
progressDeduplicated.Report(Percentage.FromFraction(0.3));  // not reported
progressDeduplicated.Report(Percentage.FromFraction(0.3));  // not reported
progressDeduplicated.Report(Percentage.FromFraction(0.5));  // reported
```

#### Merging

You can use `Merge(...)` method to combine multiple progress handlers into one:

```csharp
using Gress;

var progress1 = new Progress<Percentage>(p => /* ... */);  // IProgress<Percentage>
var progress2 = new Progress<Percentage>(p => /* ... */);  // IProgress<Percentage>

var progressMerged = progress1.Merge(progress2);           // IProgress<Percentage>

// Reports 50% on both progress handlers
progressMerged.Report(Percentage.FromFraction(0.5));
```

This method can also be called on collections:

```csharp
using Gress;

var progresses = new[]
{
    new Progress<Percentage>(p => /* ... */),
    new Progress<Percentage>(p => /* ... */),
    new Progress<Percentage>(p => /* ... */),
    new Progress<Percentage>(p => /* ... */)
};  // IProgress<Percentage>[]

var progressMerged = progresses.Merge();  // IProgress<Percentage>

// Reports 50% on all progress handlers
progressMerged.Report(Percentage.FromFraction(0.5));
```

### Muxing

Muxing allows a single handler to aggregate progress reports from multiple input handlers.
This is useful when you want to track progress of an operation that itself encapsulates other operations.

To do this, call `CreateMuxer()` on a progress handler and then create an input corresponding to each operation:

```csharp
using Gress;

var progress = new Progress<Percentage>(p => /* ... */);  // IProgress<Percentage>

var muxer = progress.CreateMuxer();
var progressInput1 = muxer.CreateInput();                 // IProgress<Percentage>
var progressInput2 = muxer.CreateInput();                 // IProgress<Percentage>
var progressInput3 = muxer.CreateInput();                 // IProgress<Percentage>
```

When progress is reported on any of the individual inputs, its value is aggregated with values reported on other inputs, and then routed to the original target handler.
The sample below illustrates this process:

```csharp
// ...

progressInput1.Report(Percentage.FromFraction(0.5));

// Input 1 ->  50%
// Input 2 ->   0%
// Input 3 ->   0%
// Total   -> ~17%

progressInput1.Report(Percentage.FromFraction(1));
progressInput2.Report(Percentage.FromFraction(0.75));

// Input 1 -> 100%
// Input 2 ->  75%
// Input 3 ->   0%
// Total   -> ~58%

progressInput2.Report(Percentage.FromFraction(1));
progressInput3.Report(Percentage.FromFraction(0.9));

// Input 1 -> 100%
// Input 2 -> 100%
// Input 3 ->  90%
// Total   -> ~97%

progressInput3.Report(Percentage.FromFraction(1));

// Input 1 -> 100%
// Input 2 -> 100%
// Input 3 -> 100%
// Total   -> 100%
```

> 💡 `ProgressMuxer` is thread-safe and can be used to aggregate progress across parallel operations.

> ⚠️ Muxing is only available for percentage-based handlers. 
If you need to, you can convert the handler to the expected type by calling `ToPercentageBased()` on it.

Inputs can also be muxed further:
 
#### With custom weight

A muxer input may be created with a custom weight, which determines the priority of a particular input in relation to others.
Progress reported on a handler with higher weight influences the final progress to a greater degree and vice versa.

To create a weighted muxer input, pass the weight when calling the `CreateInput(...)` method:

```csharp
using Gress;

var progress = new Progress<Percentage>(p => /* ... */);  // IProgress<Percentage>

var muxer = progress.CreateMuxer();
var progressInput1 = muxer.CreateInput(2);                 // IProgress<Percentage>
var progressInput2 = muxer.CreateInput(8);                 // IProgress<Percentage>

// Weight split:
// Input 1 -> 20% of total
// Input 2 -> 80% of total

progressInput1.Report(Percentage.FromFraction(0.9));
progressInput2.Report(Percentage.FromFraction(0.3));

// Input 1 -> 90%
// Input 2 -> 30%
// Total   -> 42%
```

#### With auto-reset



### Terminals

**Gress** offers a few progress handlers that can be used to collect progress.
These handlers do not route the progress anywhere and just store it instead, which effectively makes them terminal nodes in progress-routing pipelines.

#### `ProgressContainer`

#### `ProgressCollector`

### Completable handlers