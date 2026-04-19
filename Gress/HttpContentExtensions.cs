using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Gress;

/// <summary>
/// Extensions for <see cref="HttpContent" />.
/// </summary>
public static class HttpContentExtensions
{
    private const int DefaultBufferSize = 81920;

    /// <inheritdoc cref="HttpContentExtensions" />
    extension(HttpContent content)
    {
        /// <summary>
        /// Serializes the HTTP content and writes it to a stream,
        /// reporting progress as a <see cref="Percentage" /> value.
        /// Progress is only reported when the <c>Content-Length</c> header is present,
        /// as the total byte count must be known to compute a percentage.
        /// </summary>
        public async Task CopyToAsync(
            Stream destination,
            IProgress<Percentage>? progress,
            CancellationToken cancellationToken = default
        )
        {
            var totalBytes = content.Headers.ContentLength ?? -1;
            var bytesRead = 0L;

            using var sourceStream = await content.ReadAsStreamAsync(cancellationToken);

            var buffer = new byte[DefaultBufferSize];
            int count;
            while (
                (count = await sourceStream.ReadAsync(buffer, 0, buffer.Length, cancellationToken))
                > 0
            )
            {
                await destination.WriteAsync(buffer, 0, count, cancellationToken);
                bytesRead += count;

                if (progress is not null && totalBytes > 0)
                {
                    progress.Report(
                        Percentage.FromFraction(Math.Min(1.0, (double)bytesRead / totalBytes))
                    );
                }
            }
        }

        /// <summary>
        /// Reads the HTTP content and returns it as a byte array,
        /// reporting progress as a <see cref="Percentage" /> value.
        /// Progress is only reported when the <c>Content-Length</c> header is present,
        /// as the total byte count must be known to compute a percentage.
        /// </summary>
        public async Task<byte[]> ReadAsByteArrayAsync(
            IProgress<Percentage>? progress,
            CancellationToken cancellationToken = default
        )
        {
            using var destination = new MemoryStream();
            await content.CopyToAsync(destination, progress, cancellationToken);
            return destination.ToArray();
        }

        /// <summary>
        /// Reads the HTTP content and returns it as a string,
        /// reporting progress as a <see cref="Percentage" /> value.
        /// Progress is only reported when the <c>Content-Length</c> header is present,
        /// as the total byte count must be known to compute a percentage.
        /// The character encoding is inferred from the <c>Content-Type</c> charset header,
        /// falling back to UTF-8 when absent or unrecognized.
        /// </summary>
        public async Task<string> ReadAsStringAsync(
            IProgress<Percentage>? progress,
            CancellationToken cancellationToken = default
        )
        {
            var bytes = await content.ReadAsByteArrayAsync(progress, cancellationToken);

            var charset = content.Headers.ContentType?.CharSet;
            var encoding = Encoding.UTF8;
            if (charset is not null)
            {
                try
                {
                    encoding = Encoding.GetEncoding(charset);
                }
                catch (ArgumentException) { }
            }

            return encoding.GetString(bytes);
        }
    }
}
