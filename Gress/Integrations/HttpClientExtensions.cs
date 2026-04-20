using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Gress.Integrations;
using PowerKit.Extensions;

namespace Gress.Integrations;

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
        /// Sends a GET request and saves the response body to a file.
        /// </summary>
        public async Task DownloadAsync(
            Uri requestUri,
            string filePath,
            IProgress<Percentage>? progress,
            CancellationToken cancellationToken = default
        ) =>
            await client
                .DownloadAsync(requestUri, filePath, progress?.ToDoubleBased(), cancellationToken)
                .ConfigureAwait(false);

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
