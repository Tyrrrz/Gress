using System;
using FluentAssertions;
using Xunit;

namespace Gress.Tests;

public class MuxingSpecs : SpecsBase
{
    [Fact]
    public void Progress_handler_muxed_from_a_single_handler_reports_progress_as_is()
    {
        // Arrange
        var progress = Percentage.Zero;
        var handler = new Progress<Percentage>(p => progress = p);

        var muxer = handler.CreateMuxer();
        var subHandler = muxer.CreateInput();

        // Act
        subHandler.Report(Percentage.FromFraction(0.5));

        // Assert
        progress.Should().Be(Percentage.FromFraction(0.5));
    }

    [Fact]
    public void Progress_handler_muxed_from_multiple_handlers_reports_aggregated_progress()
    {
        // Arrange
        var progress = Percentage.Zero;
        var handler = new Progress<Percentage>(p => progress = p);

        var muxer = handler.CreateMuxer();
        var subHandler1 = muxer.CreateInput();
        var subHandler2 = muxer.CreateInput();
        var subHandler3 = muxer.CreateInput();

        // Act
        subHandler1.Report(Percentage.FromFraction(0.65));
        subHandler2.Report(Percentage.FromFraction(0.25));
        subHandler3.Report(Percentage.FromFraction(0.09));

        // Assert
        progress.Should().Be(Percentage.FromFraction(0.33));
    }

    [Fact]
    public void Progress_handler_muxed_from_multiple_weighted_handlers_reports_aggregated_progress()
    {
        // Arrange
        var progress = Percentage.Zero;
        var handler = new Progress<Percentage>(p => progress = p);

        var muxer = handler.CreateMuxer();
        var subHandler1 = muxer.CreateInput();
        var subHandler2 = muxer.CreateInput(2);
        var subHandler3 = muxer.CreateInput(7);

        // Act
        subHandler1.Report(Percentage.FromFraction(1));
        subHandler2.Report(Percentage.FromFraction(0.5));
        subHandler3.Report(Percentage.FromFraction(0.25));

        // Assert
        progress.Should().Be(Percentage.FromFraction(0.375));
    }
}