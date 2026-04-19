using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Gress;

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
        /// Progress is only reported when the source stream supports seeking,
        /// as the total byte count must be known to compute a percentage.
        /// </summary>
        public void CopyTo(Stream destination, IProgress<Percentage>? progress)
        {
            if (progress is null)
            {
                source.CopyTo(destination);
                return;
            }

            var totalBytes = source.CanSeek ? source.Length - source.Position : -1;
            var bytesRead = 0L;

            var buffer = new byte[DefaultBufferSize];
            int count;
            while ((count = source.Read(buffer, 0, buffer.Length)) > 0)
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

        /// <summary>
        /// Asynchronously reads the bytes from the current stream and writes them to another
        /// stream, reporting progress as a <see cref="Percentage" /> value.
        /// Progress is only reported when the source stream supports seeking,
        /// as the total byte count must be known to compute a percentage.
        /// </summary>
        public async Task CopyToAsync(
            Stream destination,
            IProgress<Percentage>? progress,
            CancellationToken cancellationToken = default
        )
        {
            if (progress is null)
            {
                await source.CopyToAsync(destination, DefaultBufferSize, cancellationToken);
                return;
            }

            var totalBytes = source.CanSeek ? source.Length - source.Position : -1;
            var bytesRead = 0L;

            var buffer = new byte[DefaultBufferSize];
            int count;
            while (
                (count = await source.ReadAsync(buffer, 0, buffer.Length, cancellationToken)) > 0
            )
            {
                await destination.WriteAsync(buffer, 0, count, cancellationToken);
                bytesRead += count;

                if (totalBytes > 0)
                {
                    progress.Report(
                        Percentage.FromFraction(Math.Min(1.0, (double)bytesRead / totalBytes))
                    );
                }
            }
        }
    }
}
