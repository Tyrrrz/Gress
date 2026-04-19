using System;
using System.Buffers;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

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
        /// Reads the bytes from the current stream and writes them to another stream,
        /// reporting progress as a <see cref="Percentage" /> value.
        /// The <paramref name="streamLength" /> parameter specifies the total number of bytes
        /// to use when computing progress. If set to a negative value, the length is inferred
        /// from the stream when it supports seeking; otherwise progress is not reported.
        /// </summary>
        public void CopyTo(Stream destination, long streamLength, IProgress<Percentage>? progress)
        {
            if (progress is null)
            {
                source.CopyTo(destination);
                return;
            }

            var totalBytes =
                streamLength >= 0 ? streamLength
                : source.CanSeek ? source.Length - source.Position
                : -1;

            var bytesRead = 0L;

            var buffer = ArrayPool<byte>.Shared.Rent(DefaultBufferSize);
            try
            {
                while (source.Read(buffer, 0, DefaultBufferSize) is > 0 and var count)
                {
                    destination.Write(buffer, 0, count);
                    bytesRead += count;

                    if (totalBytes > 0)
                    {
                        progress.Report(
                            Percentage.FromFraction(Math.Min(1.0, (double)bytesRead / totalBytes))
                        );
                    }
                }
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(buffer);
            }
        }

        /// <summary>
        /// Reads the bytes from the current stream and writes them to another stream,
        /// reporting progress as a <see cref="Percentage" /> value.
        /// Progress is only reported when the source stream supports seeking,
        /// as the total byte count must be known to compute a percentage.
        /// </summary>
        public void CopyTo(Stream destination, IProgress<Percentage>? progress) =>
            source.CopyTo(destination, -1, progress);

        /// <summary>
        /// Asynchronously reads the bytes from the current stream and writes them to another
        /// stream, reporting progress as a <see cref="Percentage" /> value.
        /// The <paramref name="streamLength" /> parameter specifies the total number of bytes
        /// to use when computing progress. If set to a negative value, the length is inferred
        /// from the stream when it supports seeking; otherwise progress is not reported.
        /// </summary>
        public async Task CopyToAsync(
            Stream destination,
            long streamLength,
            IProgress<Percentage>? progress,
            CancellationToken cancellationToken = default
        )
        {
            if (progress is null)
            {
                await source
                    .CopyToAsync(destination, DefaultBufferSize, cancellationToken)
                    .ConfigureAwait(false);
                return;
            }

            var totalBytes =
                streamLength >= 0 ? streamLength
                : source.CanSeek ? source.Length - source.Position
                : -1;

            var bytesRead = 0L;

            var buffer = ArrayPool<byte>.Shared.Rent(DefaultBufferSize);
            try
            {
                while (
                    await source
                        .ReadAsync(buffer, 0, DefaultBufferSize, cancellationToken)
                        .ConfigureAwait(false)
                        is > 0
                            and var count
                )
                {
                    await destination
                        .WriteAsync(buffer, 0, count, cancellationToken)
                        .ConfigureAwait(false);
                    bytesRead += count;

                    if (totalBytes > 0)
                    {
                        progress.Report(
                            Percentage.FromFraction(Math.Min(1.0, (double)bytesRead / totalBytes))
                        );
                    }
                }
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(buffer);
            }
        }

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
