using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using PowerKit.Extensions;

namespace Gress.Integration;

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

        /// <summary>
        /// Reads the HTTP content and returns it as a byte array.
        /// </summary>
        public async Task<byte[]> ReadAsByteArrayAsync(
            IProgress<Percentage>? progress,
            CancellationToken cancellationToken = default
        )
        {
            using var destination = new MemoryStream();

            await content
                .CopyToAsync(destination, progress, cancellationToken)
                .ConfigureAwait(false);

            return destination.ToArray();
        }

        /// <summary>
        /// Reads the HTTP content and returns it as a string.
        /// </summary>
        public async Task<string> ReadAsStringAsync(
            IProgress<Percentage>? progress,
            CancellationToken cancellationToken = default
        )
        {
            var bytes = await content
                .ReadAsByteArrayAsync(progress, cancellationToken)
                .ConfigureAwait(false);

            var charset = content.Headers.ContentType?.CharSet;

            var encoding =
                charset
                    ?.Pipe(c =>
                        Encoding
                            .GetEncodings()
                            .FirstOrDefault(e =>
                                string.Equals(e.Name, c, StringComparison.OrdinalIgnoreCase)
                            )
                    )
                    ?.GetEncoding()
                ?? Encoding.UTF8;

            return encoding.GetString(bytes);
        }
    }
}
