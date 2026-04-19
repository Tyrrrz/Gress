using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Gress;

/// <summary>
/// Extensions for <see cref="HttpClient" />.
/// </summary>
public static class HttpClientExtensions
{
    /// <inheritdoc cref="HttpClientExtensions" />
    extension(HttpClient client)
    {
        /// <summary>
        /// Sends a GET request to the specified URI and returns the response body as a byte array,
        /// reporting progress as a <see cref="Percentage" /> value.
        /// Progress is only reported when the response includes a <c>Content-Length</c> header,
        /// as the total byte count must be known to compute a percentage.
        /// </summary>
        public async Task<byte[]> GetByteArrayAsync(
            string requestUri,
            IProgress<Percentage>? progress,
            CancellationToken cancellationToken = default
        )
        {
            using var response = await client.GetAsync(
                requestUri,
                HttpCompletionOption.ResponseHeadersRead,
                cancellationToken
            );
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsByteArrayAsync(progress, cancellationToken);
        }

        /// <summary>
        /// Sends a GET request to the specified URI and returns the response body as a byte array,
        /// reporting progress as a <see cref="Percentage" /> value.
        /// Progress is only reported when the response includes a <c>Content-Length</c> header,
        /// as the total byte count must be known to compute a percentage.
        /// </summary>
        public async Task<byte[]> GetByteArrayAsync(
            Uri requestUri,
            IProgress<Percentage>? progress,
            CancellationToken cancellationToken = default
        )
        {
            using var response = await client.GetAsync(
                requestUri,
                HttpCompletionOption.ResponseHeadersRead,
                cancellationToken
            );
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsByteArrayAsync(progress, cancellationToken);
        }

        /// <summary>
        /// Sends a GET request to the specified URI and returns the response body as a string,
        /// reporting progress as a <see cref="Percentage" /> value.
        /// Progress is only reported when the response includes a <c>Content-Length</c> header,
        /// as the total byte count must be known to compute a percentage.
        /// The character encoding is inferred from the <c>Content-Type</c> charset header,
        /// falling back to UTF-8 when absent or unrecognized.
        /// </summary>
        public async Task<string> GetStringAsync(
            string requestUri,
            IProgress<Percentage>? progress,
            CancellationToken cancellationToken = default
        )
        {
            using var response = await client.GetAsync(
                requestUri,
                HttpCompletionOption.ResponseHeadersRead,
                cancellationToken
            );
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync(progress, cancellationToken);
        }

        /// <summary>
        /// Sends a GET request to the specified URI and returns the response body as a string,
        /// reporting progress as a <see cref="Percentage" /> value.
        /// Progress is only reported when the response includes a <c>Content-Length</c> header,
        /// as the total byte count must be known to compute a percentage.
        /// The character encoding is inferred from the <c>Content-Type</c> charset header,
        /// falling back to UTF-8 when absent or unrecognized.
        /// </summary>
        public async Task<string> GetStringAsync(
            Uri requestUri,
            IProgress<Percentage>? progress,
            CancellationToken cancellationToken = default
        )
        {
            using var response = await client.GetAsync(
                requestUri,
                HttpCompletionOption.ResponseHeadersRead,
                cancellationToken
            );
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync(progress, cancellationToken);
        }
    }
}
