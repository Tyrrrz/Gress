using System;
using NUnit.Framework;

namespace Gress.Tests
{
    [TestFixture]
    public class ProgressOperationTests
    {
        [Test]
        public void ProgressOperation_Report_Test()
        {
            // Create operation
            var operation = new ProgressOperation();

            // Assert initial state
            Assert.That(operation.Progress, Is.Zero);

            // Report new progress
            const double newProgress = 0.5;
            operation.Report(newProgress);

            // Assert final state
            Assert.That(operation.Progress, Is.EqualTo(newProgress));
        }

        [Test]
        public void ProgressOperation_Report_Disposed_Test()
        {
            // Create operation
            var operation = new ProgressOperation();

            // Report completion
            operation.Dispose();

            // It shouldn't be possible to report progress anymore
            Assert.Throws<InvalidOperationException>(() => operation.Report(0.5));
        }

        [Test]
        public void ProgressOperation_Dispose_Test()
        {
            // Create operation
            var operation = new ProgressOperation();

            // Assert initial state
            Assert.That(operation.IsCompleted, Is.False);

            // Report completion
            operation.Dispose();

            // Assert final state
            Assert.That(operation.IsCompleted, Is.True);
        }

        [Test]
        public void ProgressOperation_NotifyPropertyChanged_Test()
        {
            // Create operation
            var operation = new ProgressOperation();

            // Subscribe to event
            var eventTriggerCount = 0;
            operation.PropertyChanged += (sender, args) => eventTriggerCount++;

            // Invoke changes
            operation.Report(0.1);
            operation.Report(0.3);
            operation.Dispose();

            // Assert that the event was triggered accordingly
            Assert.That(eventTriggerCount, Is.GreaterThanOrEqualTo(3));
        }
    }
}