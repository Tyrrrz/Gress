using System;
using System.Collections.Generic;
using Gress.Internal;

namespace Gress
{
    /// <summary>
    /// Extensions for <see cref="Gress"/>.
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Creates multiple new operations.
        /// </summary>
        public static IEnumerable<IProgressOperation> CreateOperations(this IProgressManager progressManager, int count,
            Func<int, double> weightSelector = null)
        {
            progressManager.GuardNotNull(nameof(progressManager));
            count.GuardNotNegative(nameof(count));

            for (var i = 0; i < count; i++)
            {
                var weight = weightSelector?.Invoke(i) ?? 1;
                yield return progressManager.CreateOperation(weight);
            }
        }
    }
}