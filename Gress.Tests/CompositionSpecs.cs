using System.Linq;
using FluentAssertions;
using Gress.Completable;
using Xunit;

namespace Gress.Tests;

public class CompositionSpecs
{
    [Fact]
    public void I_can_transform_progress_updates_into_a_different_type()
    {
        // Arrange
        var collector = new ProgressCollector<int>();

        // Act
        var progress = collector.WithTransform((string p) => p.Length);

        progress.Report("");
        progress.Report("a");
        progress.Report("abc");
        progress.Report("abcdef");

        // Assert
        collector.GetValues().Should().Equal(0, 1, 3, 6);
    }

    [Fact]
    public void I_can_transform_progress_updates_within_the_same_type()
    {
        // Arrange
        var collector = new ProgressCollector<int>();

        // Act
        var progress = collector.WithTransform(p => p * 2);

        progress.Report(1);
        progress.Report(2);
        progress.Report(3);

        // Assert
        collector.GetValues().Should().Equal(2, 4, 6);
    }

    [Fact]
    public void I_can_filter_out_progress_updates_that_do_not_satisfy_a_predicate()
    {
        // Arrange
        var collector = new ProgressCollector<int>();

        // Act
        var progress = collector.WithFilter(p => p % 2 == 0);

        progress.Report(0);
        progress.Report(1);
        progress.Report(2);
        progress.Report(3);
        progress.Report(4);
        progress.Report(5);

        // Assert
        collector.GetValues().Should().Equal(0, 2, 4);
    }

    [Fact]
    public void I_can_filter_out_progress_updates_with_duplicate_values()
    {
        // Arrange
        var collector = new ProgressCollector<Percentage>();

        // Act
        var progress = collector.WithDeduplication();

        progress.Report(Percentage.FromFraction(0.0));
        progress.Report(Percentage.FromFraction(0.1));
        progress.Report(Percentage.FromFraction(0.3));
        progress.Report(Percentage.FromFraction(0.3));
        progress.Report(Percentage.FromFraction(0.5));
        progress.Report(Percentage.FromFraction(0.6));
        progress.Report(Percentage.FromFraction(0.6));
        progress.Report(Percentage.FromFraction(0.9));

        // Assert
        collector
            .GetValues()
            .Should()
            .Equal(
                Percentage.FromFraction(0.0),
                Percentage.FromFraction(0.1),
                Percentage.FromFraction(0.3),
                Percentage.FromFraction(0.5),
                Percentage.FromFraction(0.6),
                Percentage.FromFraction(0.9)
            );
    }

    [Fact]
    public void I_can_filter_out_progress_updates_that_arrive_out_of_order()
    {
        // Arrange
        var collector = new ProgressCollector<Percentage>();

        // Act
        var progress = collector.WithOrdering();

        progress.Report(Percentage.FromFraction(0.0));
        progress.Report(Percentage.FromFraction(0.1));
        progress.Report(Percentage.FromFraction(0.3));
        progress.Report(Percentage.FromFraction(0.2));
        progress.Report(Percentage.FromFraction(0.5));
        progress.Report(Percentage.FromFraction(0.3));
        progress.Report(Percentage.FromFraction(0.6));
        progress.Report(Percentage.FromFraction(0.9));

        // Assert
        collector
            .GetValues()
            .Should()
            .Equal(
                Percentage.FromFraction(0.0),
                Percentage.FromFraction(0.1),
                Percentage.FromFraction(0.3),
                Percentage.FromFraction(0.5),
                Percentage.FromFraction(0.6),
                Percentage.FromFraction(0.9)
            );
    }

    [Fact]
    public void I_can_merge_two_progress_handlers_together()
    {
        // Arrange
        var collector1 = new ProgressCollector<Percentage>();
        var collector2 = new ProgressCollector<Percentage>();

        // Act
        var progress = collector1.Merge(collector2);

        progress.Report(Percentage.FromFraction(0.0));
        progress.Report(Percentage.FromFraction(0.1));
        progress.Report(Percentage.FromFraction(0.3));

        // Assert
        collector1
            .GetValues()
            .Should()
            .Equal(
                Percentage.FromFraction(0.0),
                Percentage.FromFraction(0.1),
                Percentage.FromFraction(0.3)
            );

        collector2
            .GetValues()
            .Should()
            .Equal(
                Percentage.FromFraction(0.0),
                Percentage.FromFraction(0.1),
                Percentage.FromFraction(0.3)
            );
    }

    [Fact]
    public void I_can_merge_multiple_progress_handlers_together()
    {
        // Arrange
        var collectors = Enumerable
            .Range(0, 10)
            .Select(_ => new ProgressCollector<Percentage>())
            .ToArray();

        // Act
        var progress = collectors.Merge();

        progress.Report(Percentage.FromFraction(0.0));
        progress.Report(Percentage.FromFraction(0.1));
        progress.Report(Percentage.FromFraction(0.3));

        // Assert
        foreach (var collector in collectors)
        {
            collector
                .GetValues()
                .Should()
                .Equal(
                    Percentage.FromFraction(0.0),
                    Percentage.FromFraction(0.1),
                    Percentage.FromFraction(0.3)
                );
        }
    }

    [Fact]
    public void I_can_convert_a_double_based_progress_handler_into_a_percentage_based_handler_using_fraction_mapping()
    {
        // Arrange
        var collector = new ProgressCollector<double>();

        // Act
        var progress = collector.ToPercentageBased();

        progress.Report(Percentage.FromFraction(0.1));
        progress.Report(Percentage.FromFraction(0.3));
        progress.Report(Percentage.FromFraction(0.5));

        // Assert
        collector.GetValues().Should().Equal(0.1, 0.3, 0.5);
    }

    [Fact]
    public void I_can_convert_a_double_based_progress_handler_into_a_percentage_based_handler_using_value_mapping()
    {
        // Arrange
        var collector = new ProgressCollector<double>();

        // Act
        var progress = collector.ToPercentageBased(false);

        progress.Report(Percentage.FromFraction(0.1));
        progress.Report(Percentage.FromFraction(0.3));
        progress.Report(Percentage.FromFraction(0.5));

        // Assert
        collector.GetValues().Should().Equal(10, 30, 50);
    }

    [Fact]
    public void I_can_convert_an_integer_based_progress_handler_into_a_percentage_based_handler()
    {
        // Arrange
        var collector = new ProgressCollector<int>();

        // Act
        var progress = collector.ToPercentageBased();

        progress.Report(Percentage.FromFraction(0.1));
        progress.Report(Percentage.FromFraction(0.3));
        progress.Report(Percentage.FromFraction(0.5));

        // Assert
        collector.GetValues().Should().Equal(10, 30, 50);
    }

    [Fact]
    public void I_can_convert_a_percentage_based_progress_handler_into_a_double_based_handler_using_fraction_mapping()
    {
        // Arrange
        var collector = new ProgressCollector<Percentage>();

        // Act
        var progress = collector.ToDoubleBased();

        progress.Report(0.1);
        progress.Report(0.3);
        progress.Report(0.5);

        // Assert
        collector
            .GetValues()
            .Should()
            .Equal(
                Percentage.FromFraction(0.1),
                Percentage.FromFraction(0.3),
                Percentage.FromFraction(0.5)
            );
    }

    [Fact]
    public void I_can_convert_a_percentage_based_progress_handler_into_a_double_based_handler_using_value_mapping()
    {
        // Arrange
        var collector = new ProgressCollector<Percentage>();

        // Act
        var progress = collector.ToDoubleBased(false);

        progress.Report(10);
        progress.Report(30);
        progress.Report(50);

        // Assert
        collector
            .GetValues()
            .Should()
            .Equal(
                Percentage.FromFraction(0.1),
                Percentage.FromFraction(0.3),
                Percentage.FromFraction(0.5)
            );
    }

    [Fact]
    public void I_can_convert_a_percentage_based_progress_handler_into_an_integer_based_handler()
    {
        // Arrange
        var collector = new ProgressCollector<Percentage>();

        // Act
        var progress = collector.ToInt32Based();

        progress.Report(10);
        progress.Report(30);
        progress.Report(50);

        // Assert
        collector
            .GetValues()
            .Should()
            .Equal(
                Percentage.FromFraction(0.1),
                Percentage.FromFraction(0.3),
                Percentage.FromFraction(0.5)
            );
    }

    [Fact]
    public void I_can_convert_a_normal_progress_handler_into_a_completable_handler()
    {
        // Arrange
        var isCompleted = false;
        var collector = new ProgressCollector<Percentage>();

        // Act
        var progress = collector.ToCompletable(() => isCompleted = true);

        progress.Report(Percentage.FromFraction(0.1));
        progress.Report(Percentage.FromFraction(0.3));
        progress.Report(Percentage.FromFraction(0.5));
        progress.ReportCompletion();

        // Assert
        isCompleted.Should().BeTrue();
        collector
            .GetValues()
            .Should()
            .Equal(
                Percentage.FromFraction(0.1),
                Percentage.FromFraction(0.3),
                Percentage.FromFraction(0.5)
            );
    }
}
