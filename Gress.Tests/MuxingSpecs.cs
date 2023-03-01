using FluentAssertions;
using Gress.Completable;
using Xunit;

namespace Gress.Tests;

public class MuxingSpecs
{
    [Fact]
    public void I_can_mux_progress_updates_from_a_single_operation()
    {
        // Arrange
        var collector = new ProgressCollector<Percentage>();
        var muxer = collector.CreateMuxer();

        // Act
        var progress = muxer.CreateInput();

        progress.Report(Percentage.FromFraction(0.1));
        progress.Report(Percentage.FromFraction(0.3));
        progress.Report(Percentage.FromFraction(0.5));

        // Assert
        collector.GetValues().Should().Equal(
            Percentage.FromFraction(0.1),
            Percentage.FromFraction(0.3),
            Percentage.FromFraction(0.5)
        );
    }

    [Fact]
    public void I_can_mux_progress_updates_from_multiple_operations()
    {
        // Arrange
        var collector = new ProgressCollector<Percentage>();
        var muxer = collector.CreateMuxer();

        // Act
        var progress1 = muxer.CreateInput();
        var progress2 = muxer.CreateInput();
        var progress3 = muxer.CreateInput();

        progress1.Report(Percentage.FromFraction(0.65));
        progress2.Report(Percentage.FromFraction(0.25));
        progress3.Report(Percentage.FromFraction(0.09));

        // Assert
        collector.GetValues().Should().Equal(
            Percentage.FromFraction(0.65 / 3),
            Percentage.FromFraction((0.65 + 0.25) / 3),
            Percentage.FromFraction((0.65 + 0.25 + 0.09) / 3)
        );
    }

    [Fact]
    public void I_can_mux_progress_updates_from_multiple_operations_with_different_weights()
    {
        // Arrange
        var collector = new ProgressCollector<Percentage>();
        var muxer = collector.CreateMuxer();

        // Act
        var progress1 = muxer.CreateInput();
        var progress2 = muxer.CreateInput(2);
        var progress3 = muxer.CreateInput(7);

        progress1.Report(Percentage.FromFraction(1));
        progress2.Report(Percentage.FromFraction(0.5));
        progress3.Report(Percentage.FromFraction(0.25));

        // Assert
        collector.GetValues().Should().Equal(
            Percentage.FromFraction(1.0 * 1 / 10),
            Percentage.FromFraction((1.0 * 1 + 2.0 * 0.5) / 10),
            Percentage.FromFraction((1.0 * 1 + 2.0 * 0.5 + 7.0 * 0.25) / 10)
        );
    }

    [Fact]
    public void I_can_mux_progress_updates_from_multiple_operations_with_auto_reset_behavior()
    {
        // Arrange
        var collector = new ProgressCollector<Percentage>();
        var muxer = collector.CreateMuxer().WithAutoReset();

        // Act
        using (var progress1 = muxer.CreateInput().ToDisposable())
        using (var progress2 = muxer.CreateInput().ToDisposable())
        using (var progress3 = muxer.CreateInput().ToDisposable())
        {
            progress1.Report(Percentage.FromFraction(1));
            progress2.Report(Percentage.FromFraction(0.5));
            progress3.Report(Percentage.FromFraction(0.25));
        }

        using (var progress4 = muxer.CreateInput().ToDisposable())
        using (var progress5 = muxer.CreateInput().ToDisposable())
        using (var progress6 = muxer.CreateInput().ToDisposable())
        {
            progress4.Report(Percentage.FromFraction(0.65));
            progress5.Report(Percentage.FromFraction(0.25));
            progress6.Report(Percentage.FromFraction(0.09));
        }

        // Assert
        collector.GetValues().Should().Equal(
            Percentage.FromFraction(1.0 / 3),
            Percentage.FromFraction((1.0 + 0.5) / 3),
            Percentage.FromFraction((1.0 + 0.5 + 0.25) / 3),
            Percentage.FromFraction(0),
            Percentage.FromFraction(0.65 / 3),
            Percentage.FromFraction((0.65 + 0.25) / 3),
            Percentage.FromFraction((0.65 + 0.25 + 0.09) / 3),
            Percentage.FromFraction(0)
        );
    }
}