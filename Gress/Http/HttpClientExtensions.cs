using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Gress.Http;

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
        public Task<byte[]> GetByteArrayAsync(
            string requestUri,
            IProgress<Percentage>? progress,
            CancellationToken cancellationToken = default
        ) => client.GetByteArrayAsync(new Uri(requestUri), progress, cancellationToken);

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
        public Task<string> GetStringAsync(
            string requestUri,
            IProgress<Percentage>? progress,
            CancellationToken cancellationToken = default
        ) => client.GetStringAsync(new Uri(requestUri), progress, cancellationToken);

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

        /// <summary>
        /// Sends a GET request to the specified URI and copies the response body to the
        /// provided stream, reporting progress as a <see cref="Percentage" /> value.
        /// Progress is only reported when the response includes a <c>Content-Length</c> header,
        /// as the total byte count must be known to compute a percentage.
        /// </summary>
        public Task DownloadAsync(
            string requestUri,
            Stream destination,
            IProgress<Percentage>? progress,
            CancellationToken cancellationToken = default
        ) => client.DownloadAsync(new Uri(requestUri), destination, progress, cancellationToken);

        /// <summary>
        /// Sends a GET request to the specified URI and copies the response body to the
        /// provided stream, reporting progress as a <see cref="Percentage" /> value.
        /// Progress is only reported when the response includes a <c>Content-Length</c> header,
        /// as the total byte count must be known to compute a percentage.
        /// </summary>
        public async Task DownloadAsync(
            Uri requestUri,
            Stream destination,
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
            await response.Content.CopyToAsync(destination, progress, cancellationToken);
        }

        /// <summary>
        /// Sends a GET request to the specified URI and saves the response body to a file,
        /// reporting progress as a <see cref="Percentage" /> value.
        /// Progress is only reported when the response includes a <c>Content-Length</c> header,
        /// as the total byte count must be known to compute a percentage.
        /// </summary>
        public Task DownloadAsync(
            string requestUri,
            string filePath,
            IProgress<Percentage>? progress,
            CancellationToken cancellationToken = default
        ) => client.DownloadAsync(new Uri(requestUri), filePath, progress, cancellationToken);

        /// <summary>
        /// Sends a GET request to the specified URI and saves the response body to a file,
        /// reporting progress as a <see cref="Percentage" /> value.
        /// Progress is only reported when the response includes a <c>Content-Length</c> header,
        /// as the total byte count must be known to compute a percentage.
        /// </summary>
        public async Task DownloadAsync(
            Uri requestUri,
            string filePath,
            IProgress<Percentage>? progress,
            CancellationToken cancellationToken = default
        )
        {
            using var fileStream = File.Create(filePath);
            await client.DownloadAsync(requestUri, fileStream, progress, cancellationToken);
        }
    }
}
