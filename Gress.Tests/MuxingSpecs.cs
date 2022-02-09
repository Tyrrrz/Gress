using FluentAssertions;
using Xunit;

namespace Gress.Tests;

public class MuxingSpecs
{
    [Fact]
    public void Progress_handler_can_be_muxed_from_a_single_input()
    {
        // Arrange
        var collector = new ProgressCollector<Percentage>();
        var muxer = collector.CreateMuxer();

        var subHandler = muxer.CreateInput();

        // Act
        subHandler.Report(Percentage.FromFraction(0.1));
        subHandler.Report(Percentage.FromFraction(0.3));
        subHandler.Report(Percentage.FromFraction(0.5));

        // Assert
        collector.GetReports().Should().Equal(
            Percentage.FromFraction(0.1),
            Percentage.FromFraction(0.3),
            Percentage.FromFraction(0.5)
        );
    }

    [Fact]
    public void Progress_handler_can_be_muxed_from_multiple_inputs()
    {
        // Arrange
        var collector = new ProgressCollector<Percentage>();
        var muxer = collector.CreateMuxer();

        var subHandler1 = muxer.CreateInput();
        var subHandler2 = muxer.CreateInput();
        var subHandler3 = muxer.CreateInput();

        // Act
        subHandler1.Report(Percentage.FromFraction(0.65));
        subHandler2.Report(Percentage.FromFraction(0.25));
        subHandler3.Report(Percentage.FromFraction(0.09));

        // Assert
        collector.GetReports().Should().Equal(
            Percentage.FromFraction(0.65 / 3),
            Percentage.FromFraction((0.65 + 0.25) / 3),
            Percentage.FromFraction((0.65 + 0.25 + 0.09) / 3)
        );
    }

    [Fact]
    public void Progress_handler_can_be_muxed_from_multiple_inputs_with_different_weights()
    {
        // Arrange
        var collector = new ProgressCollector<Percentage>();
        var muxer = collector.CreateMuxer();

        var subHandler1 = muxer.CreateInput();
        var subHandler2 = muxer.CreateInput(2);
        var subHandler3 = muxer.CreateInput(7);

        // Act
        subHandler1.Report(Percentage.FromFraction(1));
        subHandler2.Report(Percentage.FromFraction(0.5));
        subHandler3.Report(Percentage.FromFraction(0.25));

        // Assert
        collector.GetReports().Should().Equal(
            Percentage.FromFraction(1.0 * 1 / 10),
            Percentage.FromFraction((1.0 * 1 + 2.0 * 0.5) / 10),
            Percentage.FromFraction((1.0 * 1 + 2.0 * 0.5 + 7.0 * 0.25) / 10)
        );
    }
}