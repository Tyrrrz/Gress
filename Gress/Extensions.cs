using System;
using System.Collections.Generic;
using System.Linq;

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
        public static IReadOnlyList<IProgressOperation> CreateOperations(this IProgressManager progressManager,
            int count, Func<int, double>? weightSelector = null) =>
            Enumerable.Range(0, count)
                .Select(i => weightSelector?.Invoke(i) ?? 1)
                .Select(progressManager.CreateOperation)
                .ToArray();
    }
}