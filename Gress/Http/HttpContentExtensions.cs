using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using PowerKit.Extensions;

namespace Gress.Http;

/// <summary>
/// Extensions for <see cref="HttpContent" />.
/// </summary>
public static class HttpContentExtensions
{
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
        ) =>
            await content
                .CopyToStreamAsync(destination, progress?.ToDoubleBased(), cancellationToken)
                .ConfigureAwait(false);

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
            var bytes = await content
                .ReadAsByteArrayAsync(progress, cancellationToken)
                .ConfigureAwait(false);

            return (content.TryGetEncoding() ?? Encoding.UTF8).GetString(bytes);
        }
    }
}
