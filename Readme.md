# Gress

[![Build](https://github.com/Tyrrrz/Gress/workflows/main/badge.svg?branch=master)](https://github.com/Tyrrrz/Gress/actions)
[![Coverage](https://codecov.io/gh/Tyrrrz/Gress/branch/master/graph/badge.svg)](https://codecov.io/gh/Tyrrrz/Gress)
[![Version](https://img.shields.io/nuget/v/Gress.svg)](https://nuget.org/packages/Gress)
[![Downloads](https://img.shields.io/nuget/dt/Gress.svg)](https://nuget.org/packages/Gress)
[![Discord](https://img.shields.io/discord/869237470565392384?label=discord)](https://discord.gg/2SUWKFnHSm)
[![Donate](https://img.shields.io/badge/donate-$$$-purple.svg)](https://tyrrrz.me/donate)

✅ **Project status: active**. [What does it mean?](https://github.com/Tyrrrz/.github/blob/master/docs/project-status.md)

**Gress** is a library that extends the standard interface for reporting progress ([`IProgress<T>`](https://docs.microsoft.com/en-us/dotnet/api/system.iprogress-1)) with a set of utilities for transforming, filtering, aggregating, and muxing progress handlers.

💬 **If you want to chat, join my [Discord server](https://discord.gg/2SUWKFnHSm)**.

## Download

📦 [NuGet](https://nuget.org/packages/Gress): `dotnet add package Gress`

## Screenshots

![demo](.screenshots/demo.png)

## Usage

### Percentages

**Gress** provides a unified progress unit -- the `Percentage` type.
Unlike raw `int` and `double` values commonly used with `IProgress<T>`, this type unambiguously represents progress as a portion of work that has been completed so far.

An instance of `Percentage` can be created from either a value or a fraction:

```csharp
using Gress;

// 50%; mapped from value
var fiftyPercent = Percentage.FromValue(50);

// 20%; mapped from fractional representation
var twentyPercent = Percentage.FromFraction(0.2);
```

Similarly, both value and fraction can be extracted from an initialized `Percentage` by accessing the corresponding properties:

```csharp
using Gress;

var fiftyPercent = Percentage.FromValue(50);

var asValue = fiftyPercent.Value; // 50.0 (double)
var asFraction = fiftyPercent.Fraction; // 0.5 (double)
```

You can use the `Percentage` type as the generic argument for `IProgress<T>` handlers to establish a more consistent foundation for progress reporting in your application or library.

When interfacing with external methods that define their own progress handlers, you can use one of the provided extensions to convert a percentage-based handler into a handler of a different type:

```csharp
using Gress;

var progress = new Progress<Percentage>(p => /* ... */);  // IProgress<Percentage>

var progressOfDouble = progress.ToDoubleBased();          // IProgress<double>
var progressOfInt = progress.ToInt32Based();              // IProgress<int>
```

There are also methods that allow conversion in the other direction as well:

```csharp
using Gress;

var progressOfDouble = new Progress<double>(p => /* ... */);       // IProgress<double>
var progressOfInt = new Progress<int>(p => /* ... */);             // IProgress<int>

var progressOfPercentage1 = progressOfDouble.ToPercentageBased();  // IProgress<Percentage>
var progressOfPercentage2 = progressOfInt.ToPercentageBased();     // IProgress<Percentage>
```

> 💡 When converting between percentage-based and double-based handlers, percentages are mapped using their fractional form by default.
To override this behavior and map by value instead, use `ToDoubleBased(false)` and `ToPercentageBased(false)`.

### Composition

Existing progress handlers can be also composed into more complex handlers using some of the extension methods that **Gress** offers.

#### Transformation

You can use `WithTransform(...)` method to apply custom transformations to all progress values reported by a given handler:

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

A simpler overload of the same method can be used when transforming the value to the same type:

```csharp
using Gress;

var progress = new Progress<int>(p => /* ... */);              // IProgress<int>

var progressTransformed = progress.WithTransform(p => 5 * p);  // IProgress<int>

// This effectively reports 50 on the original handler
progressTransformed.Report(10);
```

> 💡 Method `WithTransform(...)` bears some resemblance to LINQ's `Select(...)`.
The main difference is that the flow of data in `IProgress<T>` is inverse to that of `IEnumerable<T>`, which means that the transformations in `WithTransform(...)` are applied in the opposite direction compared to `Select(...)`.

#### Filtering

You can use `WithFilter(...)` method to selectively filter progress values reported by a given handler:

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

You can use `WithDeduplication(...)` method to filter out consecutive progress reports with the same value:

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

Muxing allows

### Terminals

Terminal progress handlers