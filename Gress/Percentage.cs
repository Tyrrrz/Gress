﻿using System;

namespace Gress;

/// <summary>
/// Unit of progress.
/// </summary>
public readonly partial struct Percentage
{
    /// <summary>
    /// Percentage value.
    /// </summary>
    public double Value { get; }

    /// <summary>
    /// Percentage value in decimal form (e.g. 0.75 for 75%).
    /// </summary>
    public double Fraction => Value / 100.0;

    /// <summary>
    /// Initializes an instance of <see cref="Percentage"/>.
    /// </summary>
    public Percentage(double value) => Value = value;

    /// <inheritdoc />
    public override string ToString() => Fraction.ToString("P1");
}

public partial struct Percentage
{
    /// <summary>
    /// Creates a percentage from its value.
    /// </summary>
    public static Percentage FromValue(double value) => new(value);

    /// <summary>
    /// Creates a percentage from its value in decimal form (e.g. from 0.75 to 75%).
    /// </summary>
    public static Percentage FromFraction(double fraction) => FromValue(fraction * 100.0);
}

public partial struct Percentage : IEquatable<Percentage>, IComparable<Percentage>
{
    /// <inheritdoc />
    public int CompareTo(Percentage other) => Value.CompareTo(other.Value);

    /// <inheritdoc />
    public bool Equals(Percentage other) => Value.CompareTo(other.Value) == 0;

    /// <inheritdoc />
    public override bool Equals(object? obj) => obj is Percentage other && Equals(other);

    /// <inheritdoc />
    public override int GetHashCode() => Value.GetHashCode();

    /// <summary>
    /// Greater than operator.
    /// </summary>
    public static bool operator >(Percentage left, Percentage right) => left.CompareTo(right) > 0;

    /// <summary>
    /// Lesser than operator.
    /// </summary>
    public static bool operator <(Percentage left, Percentage right) => left.CompareTo(right) < 0;

    /// <summary>
    /// Equality operator.
    /// </summary>
    public static bool operator ==(Percentage left, Percentage right) => left.Equals(right);

    /// <summary>
    /// Inequality operator.
    /// </summary>
    public static bool operator !=(Percentage left, Percentage right) => !(left == right);
}