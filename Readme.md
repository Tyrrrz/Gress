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

Gress provides a unified progress unit -- the `Percentage` type.
Unlike raw `int` and `double` values commonly used with `IProgress<T>`, this type unambiguously represents progress as a portion of work that has been completed so far.

An instance of `Percentage` can be created from either a value or a fraction:

```csharp
using Gress;

// 50% mapped from percentage value
var fiftyPercent = Percentage.FromValue(50);

// 20% mapped from fractional (decimal) representation
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

var progress = new Progress<Percentage>(p => /* ... */); // IProgress<Percentage>

// Expects the reported value to represent a percentage in fractional form
var progressOfDouble1 = progress.ToDoubleBased(); // IProgress<double>

// Expects the reported value to represent a percentage in value form
var progressOfDouble2 = progress.ToDoubleBased(false); // IProgress<double>

// Expects the reported value to represent a percentage in value form
var progressOfInt = progress.ToInt32Based(); // IProgress<int>
```

There are also methods that allow conversion in the other direction as well:

```csharp
using Gress;

var progressOfDouble = new Progress<double>(p => /* ... */); // IProgress<double>
var progressOfInt = new Progress<double>(p => /* ... */); // IProgress<double>

// Reports the percentage in fractional form
var progressOfPercentage1 = progressOfDouble.ToPercentageBased(); // IProgress<Percentage>

// Reports the percentage in value form
var progressOfPercentage2 = progressOfDouble.ToPercentageBased(false); // IProgress<Percentage>

// Reports the percentage in value form
var progressOfPercentage3 = progressOfInt.ToPercentageBased(); // IProgress<Percentage>
```

### Filters and transforms

### Muxing

Muxing allows

### Terminals