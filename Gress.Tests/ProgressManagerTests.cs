using System.Linq;
using NUnit.Framework;

namespace Gress.Tests
{
    [TestFixture]
    public class ProgressManagerTests
    {
        [Test]
        public void ProgressManager_NoOperations_Test()
        {
            // Create manager
            var manager = new ProgressManager();

            // Assert state
            Assert.That(manager.Progress, Is.Zero);
            Assert.That(manager.IsActive, Is.False);
            Assert.That(manager.GetOperations().Count, Is.Zero);
        }

        [Test]
        public void ProgressManager_SingleOperation_Test()
        {
            // Create manager
            var manager = new ProgressManager();

            // Create operation
            var operation = manager.CreateOperation();

            // Assert initial state after creating operation
            Assert.That(manager.Progress, Is.Zero);
            Assert.That(manager.IsActive, Is.True);
            Assert.That(manager.GetOperations().Count, Is.EqualTo(1));

            // Report new progress
            const double newProgress = 0.5;
            operation.Report(newProgress);

            // Assert intermediate state
            Assert.That(manager.Progress, Is.EqualTo(newProgress));
            Assert.That(manager.IsActive, Is.True);
            Assert.That(manager.GetOperations().Count, Is.EqualTo(1));

            // Report completion
            operation.Dispose();

            // Assert final state
            Assert.That(manager.Progress, Is.Zero);
            Assert.That(manager.IsActive, Is.False);
            Assert.That(manager.GetOperations().Count, Is.Zero);
        }

        [Test]
        public void ProgressManager_MultipleOperations_Test()
        {
            // Create manager
            var manager = new ProgressManager();

            // Create operations
            var operations = manager.CreateOperations(3).ToArray();

            // Assert initial state after creating operations
            Assert.That(manager.Progress, Is.Zero);
            Assert.That(manager.IsActive, Is.True);
            Assert.That(manager.GetOperations().Count, Is.EqualTo(operations.Length));

            // Report new progress
            const double newProgress = 0.5;
            foreach (var operation in operations)
                operation.Report(newProgress);

            // Assert intermediate state
            Assert.That(manager.Progress, Is.EqualTo(newProgress));
            Assert.That(manager.IsActive, Is.True);
            Assert.That(manager.GetOperations().Count, Is.EqualTo(operations.Length));

            // Report completion
            foreach (var operation in operations)
                operation.Dispose();

            // Assert final state
            Assert.That(manager.Progress, Is.Zero);
            Assert.That(manager.IsActive, Is.False);
            Assert.That(manager.GetOperations().Count, Is.Zero);
        }

        [Test]
        public void ProgressManager_MultipleOperations_DifferentWeight_Test()
        {
            // Create manager
            var manager = new ProgressManager();

            // Create operations
            var operations = manager.CreateOperations(3, i => i + 1).ToArray();

            // Assert initial state after creating operations
            Assert.That(manager.Progress, Is.Zero);
            Assert.That(manager.IsActive, Is.True);
            Assert.That(manager.GetOperations().Count, Is.EqualTo(operations.Length));

            // Report new progress
            const double newProgress = 0.5;
            foreach (var operation in operations)
                operation.Report(newProgress);

            // Assert intermediate state
            Assert.That(manager.Progress, Is.EqualTo(newProgress));
            Assert.That(manager.IsActive, Is.True);
            Assert.That(manager.GetOperations().Count, Is.EqualTo(operations.Length));

            // Report completion
            foreach (var operation in operations)
                operation.Dispose();

            // Assert final state
            Assert.That(manager.Progress, Is.Zero);
            Assert.That(manager.IsActive, Is.False);
            Assert.That(manager.GetOperations().Count, Is.Zero);
        }

        [Test]
        public void ProgressManager_MultipleOperations_Sequential_Test()
        {
            // Create manager
            var manager = new ProgressManager();

            // Create operations
            var operations = manager.CreateOperations(3).ToArray();

            // Assert initial state after creating operations
            Assert.That(manager.Progress, Is.Zero);
            Assert.That(manager.IsActive, Is.True);
            Assert.That(manager.GetOperations().Count, Is.EqualTo(operations.Length));

            // Loop through each operation and report progress
            for (var i = 0; i < operations.Length; i++)
            {
                // Assert intermediate state
                Assert.That(manager.Progress, Is.EqualTo(1.0 * i / operations.Length));
                Assert.That(manager.IsActive, Is.True);
                Assert.That(manager.GetOperations().Count, Is.EqualTo(operations.Length));

                using (var operation = operations[i])
                {
                    operation.Report(0.2);
                    operation.Report(0.5);
                    operation.Report(0.9);
                }
            }

            // Assert final state
            Assert.That(manager.Progress, Is.Zero);
            Assert.That(manager.IsActive, Is.False);
            Assert.That(manager.GetOperations().Count, Is.Zero);
        }

        [Test]
        public void ProgressManager_NotifyPropertyChanged_Test()
        {
            // Create manager
            var manager = new ProgressManager();

            // Subscribe to event
            var eventTriggerCount = 0;
            manager.PropertyChanged += (sender, args) => eventTriggerCount++;

            // Create operation
            var operation = manager.CreateOperation();

            // Invoke changes
            operation.Report(0.1);
            operation.Report(0.3);
            operation.Dispose();

            // Assert that the event was triggered accordingly
            Assert.That(eventTriggerCount, Is.GreaterThanOrEqualTo(3));
        }
    }
}