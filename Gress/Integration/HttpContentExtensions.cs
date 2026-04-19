using System;
using System.IO;
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

        private Encoding? TryGetEncoding()
        {
            var charset = content.Headers.ContentType?.CharSet;
            if (charset is null)
                return null;

            try
            {
                return Encoding.GetEncoding(charset);
            }
            catch (ArgumentException)
            {
                return null;
            }
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

            return (content.TryGetEncoding() ?? Encoding.UTF8).GetString(bytes);
        }
    }
}
