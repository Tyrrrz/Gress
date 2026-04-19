using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Gress.Integration;

/// <summary>
/// Provides progress-aware extensions for <see cref="HttpClient" />.
/// </summary>
public static class HttpClientExtensions
{
    /// <inheritdoc cref="HttpClientExtensions" />
    extension(HttpClient client)
    {
        /// <summary>
        /// Sends a GET request and returns the response body as a byte array.
        /// </summary>
        public async Task<byte[]> GetByteArrayAsync(
            Uri requestUri,
            IProgress<Percentage>? progress,
            CancellationToken cancellationToken = default
        )
        {
            using var response = await client
                .GetAsync(requestUri, HttpCompletionOption.ResponseHeadersRead, cancellationToken)
                .ConfigureAwait(false);

            response.EnsureSuccessStatusCode();

            return await response
                .Content.ReadAsByteArrayAsync(progress, cancellationToken)
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Sends a GET request and returns the response body as a byte array.
        /// </summary>
        public async Task<byte[]> GetByteArrayAsync(
            string requestUri,
            IProgress<Percentage>? progress,
            CancellationToken cancellationToken = default
        ) =>
            await client
                .GetByteArrayAsync(
                    new Uri(requestUri, UriKind.RelativeOrAbsolute),
                    progress,
                    cancellationToken
                )
                .ConfigureAwait(false);

        /// <summary>
        /// Sends a GET request and returns the response body as text.
        /// </summary>
        public async Task<string> GetStringAsync(
            Uri requestUri,
            IProgress<Percentage>? progress,
            CancellationToken cancellationToken = default
        )
        {
            using var response = await client
                .GetAsync(requestUri, HttpCompletionOption.ResponseHeadersRead, cancellationToken)
                .ConfigureAwait(false);

            response.EnsureSuccessStatusCode();

            return await response
                .Content.ReadAsStringAsync(progress, cancellationToken)
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Sends a GET request and returns the response body as text.
        /// </summary>
        public async Task<string> GetStringAsync(
            string requestUri,
            IProgress<Percentage>? progress,
            CancellationToken cancellationToken = default
        ) =>
            await client
                .GetStringAsync(
                    new Uri(requestUri, UriKind.RelativeOrAbsolute),
                    progress,
                    cancellationToken
                )
                .ConfigureAwait(false);

        /// <summary>
        /// Sends a GET request and copies the response body to the provided stream.
        /// </summary>
        public async Task DownloadAsync(
            Uri requestUri,
            Stream destination,
            IProgress<Percentage>? progress,
            CancellationToken cancellationToken = default
        )
        {
            using var response = await client
                .GetAsync(requestUri, HttpCompletionOption.ResponseHeadersRead, cancellationToken)
                .ConfigureAwait(false);

            response.EnsureSuccessStatusCode();

            await response
                .Content.CopyToAsync(destination, progress, cancellationToken)
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Sends a GET request and copies the response body to the provided stream.
        /// </summary>
        public async Task DownloadAsync(
            string requestUri,
            Stream destination,
            IProgress<Percentage>? progress,
            CancellationToken cancellationToken = default
        ) =>
            await client
                .DownloadAsync(
                    new Uri(requestUri, UriKind.RelativeOrAbsolute),
                    destination,
                    progress,
                    cancellationToken
                )
                .ConfigureAwait(false);

        /// <summary>
        /// Sends a GET request and saves the response body to a file.
        /// </summary>
        public async Task DownloadAsync(
            Uri requestUri,
            string filePath,
            IProgress<Percentage>? progress,
            CancellationToken cancellationToken = default
        )
        {
            using var fileStream = new FileStream(
                filePath,
                FileMode.Create,
                FileAccess.Write,
                FileShare.None,
                4096,
                FileOptions.Asynchronous
            );

            await client
                .DownloadAsync(requestUri, fileStream, progress, cancellationToken)
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Sends a GET request and saves the response body to a file.
        /// </summary>
        public async Task DownloadAsync(
            string requestUri,
            string filePath,
            IProgress<Percentage>? progress,
            CancellationToken cancellationToken = default
        ) =>
            await client
                .DownloadAsync(
                    new Uri(requestUri, UriKind.RelativeOrAbsolute),
                    filePath,
                    progress,
                    cancellationToken
                )
                .ConfigureAwait(false);
    }
}
