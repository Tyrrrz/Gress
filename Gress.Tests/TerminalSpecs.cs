using System.ComponentModel;
using System.Threading;
using FluentAssertions;
using Xunit;

namespace Gress.Tests;

public class TerminalSpecs
{
    [Fact]
    public void I_can_route_progress_updates_into_a_collection()
    {
        // Arrange
        var progress = new ProgressCollector<Percentage>();
        progress.Reset();

        // Act
        progress.Report(Percentage.FromFraction(0.1));
        progress.Report(Percentage.FromFraction(0.3));
        progress.Report(Percentage.FromFraction(0.5));

        // Assert
        progress.GetValues().Should().Equal(
            Percentage.FromFraction(0.1),
            Percentage.FromFraction(0.3),
            Percentage.FromFraction(0.5)
        );
    }

    [Fact]
    public void I_can_route_progress_updates_into_a_property()
    {
        // Arrange
        var progress = new ProgressContainer<Percentage>();

        // Act
        progress.Report(Percentage.FromFraction(0.1));
        progress.Report(Percentage.FromFraction(0.3));
        progress.Report(Percentage.FromFraction(0.5));

        // Assert
        progress.Current.Should().Be(Percentage.FromFraction(0.5));
    }

    [Fact]
    public void I_can_route_progress_updates_into_a_property_with_change_notifications()
    {
        // Arrange
        var progress = new ProgressContainer<Percentage>();

        var triggerCount = 0;
        ((INotifyPropertyChanged)progress).PropertyChanged += (_, _) => Interlocked.Increment(ref triggerCount);

        // Act
        progress.Report(Percentage.FromFraction(0.1));
        progress.Report(Percentage.FromFraction(0.3));
        progress.Report(Percentage.FromFraction(0.5));

        // Assert
        triggerCount.Should().Be(3);
    }
}