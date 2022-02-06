using System.Linq;
using FluentAssertions;
using Xunit;

namespace Gress.Tests;

public class UtilitySpecs : SpecsBase
{
    [Fact]
    public void Progress_reports_can_be_projected_into_a_different_form()
    {
        // Arrange
        var collector = new ProgressCollector<int>();

        // Act
        var progress = collector.Select((string p) => p.Length);

        progress.Report("");
        progress.Report("a");
        progress.Report("abc");
        progress.Report("abcdef");

        // Assert
        collector.GetReports().Should().Equal(0, 1, 3, 6);
    }

    [Fact]
    public void Progress_reports_can_be_filtered_out_based_on_a_predicate()
    {
        // Arrange
        var collector = new ProgressCollector<int>();

        // Act
        var progress = collector.Where(p => p % 2 == 0);

        progress.Report(0);
        progress.Report(1);
        progress.Report(2);
        progress.Report(3);
        progress.Report(4);
        progress.Report(5);

        // Assert
        collector.GetReports().Should().Equal(0, 2, 4);
    }

    [Fact]
    public void Progress_reports_can_be_deduplicated()
    {
        // Arrange
        var collector = new ProgressCollector<Percentage>();

        // Act
        var progress = collector.Distinct();

        progress.Report(Percentage.Zero);
        progress.Report(Percentage.FromFraction(0.1));
        progress.Report(Percentage.FromFraction(0.3));
        progress.Report(Percentage.FromFraction(0.3));
        progress.Report(Percentage.FromFraction(0.5));
        progress.Report(Percentage.FromFraction(0.6));
        progress.Report(Percentage.FromFraction(0.6));
        progress.Report(Percentage.FromFraction(0.9));

        // Assert
        collector.GetReports().Should().Equal(
            Percentage.Zero,
            Percentage.FromFraction(0.1),
            Percentage.FromFraction(0.3),
            Percentage.FromFraction(0.5),
            Percentage.FromFraction(0.6),
            Percentage.FromFraction(0.9)
        );
    }

    [Fact]
    public void Progress_handler_can_be_merged_with_another_progress_handler()
    {
        // Arrange
        var collector1 = new ProgressCollector<Percentage>();
        var collector2 = new ProgressCollector<Percentage>();

        // Act
        var progress = collector1.Merge(collector2);

        progress.Report(Percentage.Zero);
        progress.Report(Percentage.FromFraction(0.1));
        progress.Report(Percentage.FromFraction(0.3));

        // Assert
        collector1.GetReports().Should().Equal(
            Percentage.Zero,
            Percentage.FromFraction(0.1),
            Percentage.FromFraction(0.3)
        );

        collector2.GetReports().Should().Equal(
            Percentage.Zero,
            Percentage.FromFraction(0.1),
            Percentage.FromFraction(0.3)
        );
    }

    [Fact]
    public void Multiple_progress_handlers_can_be_merged_into_one()
    {
        // Arrange
        var collectors = Enumerable
            .Range(0, 10)
            .Select(_ => new ProgressCollector<Percentage>())
            .ToArray();

        // Act
        var progress = collectors.Merge();

        progress.Report(Percentage.Zero);
        progress.Report(Percentage.FromFraction(0.1));
        progress.Report(Percentage.FromFraction(0.3));

        // Assert
        foreach (var collector in collectors)
        {
            collector.GetReports().Should().Equal(
                Percentage.Zero,
                Percentage.FromFraction(0.1),
                Percentage.FromFraction(0.3)
            );
        }
    }

    [Fact]
    public void Double_based_progress_handler_can_be_converted_into_a_percentage_based_handler_using_fraction_mapping()
    {
        // Arrange
        var collector = new ProgressCollector<double>();

        // Act
        var progress = collector.ToPercentageBased();

        progress.Report(Percentage.FromFraction(0.1));
        progress.Report(Percentage.FromFraction(0.3));
        progress.Report(Percentage.FromFraction(0.5));

        // Assert
        collector.GetReports().Should().Equal(0.1, 0.3, 0.5);
    }

    [Fact]
    public void Double_based_progress_handler_can_be_converted_into_a_percentage_based_handler_using_value_mapping()
    {
        // Arrange
        var collector = new ProgressCollector<double>();

        // Act
        var progress = collector.ToPercentageBased(false);

        progress.Report(Percentage.FromFraction(0.1));
        progress.Report(Percentage.FromFraction(0.3));
        progress.Report(Percentage.FromFraction(0.5));

        // Assert
        collector.GetReports().Should().Equal(10, 30, 50);
    }

    [Fact]
    public void Integer_based_progress_handler_can_be_converted_into_a_percentage_based_handler()
    {
        // Arrange
        var collector = new ProgressCollector<int>();

        // Act
        var progress = collector.ToPercentageBased();

        progress.Report(Percentage.FromFraction(0.1));
        progress.Report(Percentage.FromFraction(0.3));
        progress.Report(Percentage.FromFraction(0.5));

        // Assert
        collector.GetReports().Should().Equal(10, 30, 50);
    }

    [Fact]
    public void Percentage_based_progress_handler_can_be_converted_into_a_double_based_handler_using_fraction_mapping()
    {
        // Arrange
        var collector = new ProgressCollector<Percentage>();

        // Act
        var progress = collector.ToDoubleBased();

        progress.Report(0.1);
        progress.Report(0.3);
        progress.Report(0.5);

        // Assert
        collector.GetReports().Should().Equal(
            Percentage.FromFraction(0.1),
            Percentage.FromFraction(0.3),
            Percentage.FromFraction(0.5)
        );
    }

    [Fact]
    public void Percentage_based_progress_handler_can_be_converted_into_a_double_based_handler_using_value_mapping()
    {
        // Arrange
        var collector = new ProgressCollector<Percentage>();

        // Act
        var progress = collector.ToDoubleBased(false);

        progress.Report(10);
        progress.Report(30);
        progress.Report(50);

        // Assert
        collector.GetReports().Should().Equal(
            Percentage.FromFraction(0.1),
            Percentage.FromFraction(0.3),
            Percentage.FromFraction(0.5)
        );
    }

    [Fact]
    public void Percentage_based_progress_handler_can_be_converted_into_an_integer_based_handler()
    {
        // Arrange
        var collector = new ProgressCollector<Percentage>();

        // Act
        var progress = collector.ToInt32Based();

        progress.Report(10);
        progress.Report(30);
        progress.Report(50);

        // Assert
        collector.GetReports().Should().Equal(
            Percentage.FromFraction(0.1),
            Percentage.FromFraction(0.3),
            Percentage.FromFraction(0.5)
        );
    }
}