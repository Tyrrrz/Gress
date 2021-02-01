using System;
using System.Collections.Generic;
using FluentAssertions;
using Gress.Tests.Internal;
using Xunit;

namespace Gress.Tests
{
    public class MainSpecs
    {
        [Fact]
        public void Progress_manager_with_no_operations_returns_zero_progress()
        {
            // Arrange
            var manager = new ProgressManager();

            // Assert
            manager.Progress.Should().Be(0);
            manager.IsActive.Should().BeFalse();
            manager.Operations.Should().BeEmpty();
        }

        [Fact]
        public void Progress_manager_with_a_single_operation_returns_its_progress()
        {
            // Arrange
            var manager = new ProgressManager();
            var operation = manager.CreateOperation();

            // Assert
            manager.Progress.Should().Be(0);
            manager.IsActive.Should().BeTrue();
            manager.Operations.Should().BeEquivalentTo(operation);

            // Act
            operation.Report(0.44);

            // Assert
            manager.Progress.Should().BeApproximately(0.44, 10e-5);
            manager.IsActive.Should().BeTrue();
            manager.Operations.Should().BeEquivalentTo(operation);

            // Act
            operation.Dispose();

            // Assert
            manager.Progress.Should().Be(0);
            manager.IsActive.Should().BeFalse();
            manager.Operations.Should().BeEmpty();
        }

        [Fact]
        public void Progress_manager_with_multiple_operations_aggregates_their_progress()
        {
            // Arrange
            var manager = new ProgressManager();
            var operationA = manager.CreateOperation();
            var operationB = manager.CreateOperation();

            // Assert
            manager.Progress.Should().Be(0);
            manager.IsActive.Should().BeTrue();
            manager.Operations.Should().BeEquivalentTo(operationA, operationB);

            // Act
            operationA.Report(0.8);
            operationB.Report(0.4);

            // Assert
            manager.Progress.Should().BeApproximately(0.6, 10e-5);
            manager.IsActive.Should().BeTrue();
            manager.Operations.Should().BeEquivalentTo(operationA, operationB);

            // Act
            operationA.Dispose();
            operationB.Dispose();

            // Assert
            manager.Progress.Should().Be(0);
            manager.IsActive.Should().BeFalse();
            manager.Operations.Should().BeEmpty();
        }

        [Fact]
        public void Progress_manager_with_multiple_operations_aggregates_their_progress_according_to_their_weight()
        {
            // Arrange
            var manager = new ProgressManager();
            var operationA = manager.CreateOperation(2);
            var operationB = manager.CreateOperation(8);

            // Assert
            manager.Progress.Should().Be(0);
            manager.IsActive.Should().BeTrue();
            manager.Operations.Should().BeEquivalentTo(operationA, operationB);

            // Act
            operationA.Report(0.8);
            operationB.Report(0.4);

            // Assert
            manager.Progress.Should().BeApproximately(0.48, 10e-5);
            manager.IsActive.Should().BeTrue();
            manager.Operations.Should().BeEquivalentTo(operationA, operationB);

            // Act
            operationA.Dispose();
            operationB.Dispose();

            // Assert
            manager.Progress.Should().Be(0);
            manager.IsActive.Should().BeFalse();
            manager.Operations.Should().BeEmpty();
        }

        [Fact]
        public void Progress_manager_can_create_multiple_operations_at_once()
        {
            // Arrange
            var manager = new ProgressManager();

            // Act
            var operations = manager.CreateOperations(3);

            // Assert
            operations.Should().HaveCount(3);
        }

        [Fact]
        public void Progress_manager_can_be_created_from_an_existing_instance_of_IProgress()
        {
            // Arrange
            var reportedValues = new List<double>();
            var progress = new DelegateProgress<double>(reportedValues.Add);

            var manager = progress.Wrap();
            var operation = manager.CreateOperation();

            // Act
            operation.Report(0.1);
            operation.Report(0.3);
            operation.Dispose();

            // Assert
            reportedValues.Should().Equal(0.1, 0.3, 0);
        }

        [Fact]
        public void Progress_manager_correctly_implements_INotifyPropertyChanged()
        {
            // Arrange
            var manager = new ProgressManager();

            var eventValues = new List<double>();
            manager.PropertyChanged += (_, args) =>
            {
                if (args.PropertyName == nameof(manager.Progress))
                {
                    eventValues.Add(manager.Progress);
                }
            };

            var operation = manager.CreateOperation();

            // Act
            operation.Report(0.1);
            operation.Report(0.3);
            operation.Dispose();

            // Assert
            eventValues.Should().Equal(0.1, 0.3, 0);
        }

        [Fact]
        public void Progress_operation_correctly_implements_INotifyPropertyChanged()
        {
            // Arrange
            var manager = new ProgressManager();

            var operation = manager.CreateOperation();

            var eventValues = new List<double>();
            operation.PropertyChanged += (_, args) =>
            {
                if (args.PropertyName == nameof(manager.Progress))
                {
                    eventValues.Add(manager.Progress);
                }
            };

            // Act
            operation.Report(0.1);
            operation.Report(0.3);
            operation.Dispose();

            // Assert
            eventValues.Should().Equal(0.1, 0.3, 0);
        }

        [Fact]
        public void Progress_operation_cannot_report_progress_after_it_has_been_disposed()
        {
            // Arrange
            var manager = new ProgressManager();

            var operation = manager.CreateOperation();

            operation.Report(0.1);
            operation.Report(0.3);
            operation.Dispose();

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => operation.Report(1));
        }
    }
}