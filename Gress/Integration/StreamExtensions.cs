using System;
using System.Buffers;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using PowerKit.Extensions;

namespace Gress.Integration;

/// <summary>
/// Provides progress-aware extensions for <see cref="Stream" />.
/// </summary>
public static class StreamExtensions
{
    /// <inheritdoc cref="StreamExtensions" />
    extension(Stream source)
    {
        /// <summary>
        /// Asynchronously copies bytes from the source stream to the destination stream.
        /// </summary>
        public async Task CopyToAsync(
            Stream destination,
            long sourceLength,
            IProgress<Percentage>? progress,
            CancellationToken cancellationToken = default
        ) =>
            await source.CopyToAsync(
                destination,
                sourceLength,
                progress?.ToDoubleBased(),
                cancellationToken
            );

        /// <summary>
        /// Asynchronously copies bytes from the source stream to the destination stream.
        /// </summary>
        public Task CopyToAsync(
            Stream destination,
            IProgress<Percentage>? progress,
            CancellationToken cancellationToken = default
        ) => source.CopyToAsync(destination, -1, progress, cancellationToken);
    }
}
