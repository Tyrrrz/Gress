using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using PowerKit.Extensions;

namespace Gress.Integrations;

/// <summary>
/// Provides progress-aware extensions for <see cref="HttpContent" />.
/// </summary>
public static class HttpContentExtensions
{
    /// <inheritdoc cref="HttpContentExtensions" />
    extension(HttpContent content)
    {
        /// <summary>
        /// Serializes the HTTP content and writes it to the specified stream.
        /// </summary>
        public async Task CopyToAsync(
            Stream destination,
            IProgress<Percentage>? progress,
            CancellationToken cancellationToken = default
        ) =>
            await content
                .CopyToStreamAsync(destination, progress?.ToDoubleBased(), cancellationToken)
                .ConfigureAwait(false);

        private async Task<MemoryStream> ReadAsMemoryStreamAsync(
            IProgress<Percentage>? progress,
            CancellationToken cancellationToken = default
        )
        {
            var destination = new MemoryStream();

            await content
                .CopyToAsync(destination, progress, cancellationToken)
                .ConfigureAwait(false);

            destination.Position = 0;

            return destination;
        }

        /// <summary>
        /// Reads the HTTP content and returns it as a byte array.
        /// </summary>
        public async Task<byte[]> ReadAsByteArrayAsync(
            IProgress<Percentage>? progress,
            CancellationToken cancellationToken = default
        )
        {
            using var buffer = await content
                .ReadAsMemoryStreamAsync(progress, cancellationToken)
                .ConfigureAwait(false);

            return buffer.ToArray();
        }

        /// <summary>
        /// Reads the HTTP content and returns it as a string.
        /// </summary>
        public async Task<string> ReadAsStringAsync(
            IProgress<Percentage>? progress,
            CancellationToken cancellationToken = default
        )
        {
            var encoding =
                content
                    .Headers.ContentType?.CharSet?.Pipe(c =>
                        Encoding
                            .GetEncodings()
                            .FirstOrDefault(e =>
                                string.Equals(e.Name, c, StringComparison.OrdinalIgnoreCase)
                            )
                    )
                    ?.GetEncoding()
                ?? Encoding.UTF8;

            using var buffer = await content
                .ReadAsMemoryStreamAsync(progress, cancellationToken)
                .ConfigureAwait(false);

            using var reader = new StreamReader(buffer, encoding, true);

            return await reader.ReadToEndAsync(cancellationToken).ConfigureAwait(false);
        }
    }
}
