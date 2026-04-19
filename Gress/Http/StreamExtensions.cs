using System;
using System.Buffers;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using PowerKit.Extensions;

namespace Gress.Http;

/// <summary>
/// Extensions for <see cref="Stream" />.
/// </summary>
public static class StreamExtensions
{
    private const int DefaultBufferSize = 81920;

    /// <inheritdoc cref="StreamExtensions" />
    extension(Stream source)
    {
        /// <summary>
        /// Asynchronously reads the bytes from the current stream and writes them to another
        /// stream, reporting progress as a <see cref="Percentage" /> value.
        /// The <paramref name="sourceLength" /> parameter specifies the total number of bytes
        /// to use when computing progress. If set to a negative value, the length is inferred
        /// from the stream when it supports seeking; otherwise progress is not reported.
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
        /// Asynchronously reads the bytes from the current stream and writes them to another
        /// stream, reporting progress as a <see cref="Percentage" /> value.
        /// Progress is only reported when the source stream supports seeking,
        /// as the total byte count must be known to compute a percentage.
        /// </summary>
        public Task CopyToAsync(
            Stream destination,
            IProgress<Percentage>? progress,
            CancellationToken cancellationToken = default
        ) => source.CopyToAsync(destination, -1, progress, cancellationToken);
    }
}
