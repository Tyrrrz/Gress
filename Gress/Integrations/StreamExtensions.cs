using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Gress.Integrations;
using PowerKit.Extensions;

namespace Gress.Integrations;

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
            await source
                .CopyToAsync(
                    destination,
                    sourceLength,
                    progress?.ToDoubleBased(),
                    cancellationToken
                )
                .ConfigureAwait(false);

        /// <inheritdoc cref="StreamExtensions.CopyToAsync(Stream, Stream, long, IProgress{Percentage}?, CancellationToken)" />
        public async Task CopyToAsync(
            Stream destination,
            IProgress<Percentage>? progress,
            CancellationToken cancellationToken = default
        ) =>
            await source
                .CopyToAsync(
                    destination,
                    source.CanSeek ? source.Length : -1,
                    progress,
                    cancellationToken
                )
                .ConfigureAwait(false);
    }
}
