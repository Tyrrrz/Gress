using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Gress.Http;
using Xunit;

namespace Gress.Tests;

public class HttpClientSpecs
{
    private static byte[] CreateTestData(int length)
    {
        var data = new byte[length];
        for (var i = 0; i < data.Length; i++)
            data[i] = (byte)(i % 256);
        return data;
    }

    private static HttpClient CreateClient(HttpContent responseContent) =>
        new(new FakeHttpMessageHandler(responseContent));

    [Fact]
    public async Task I_can_download_a_byte_array_via_string_url_with_progress()
    {
        // Arrange
        // ByteArrayContent sets Content-Length automatically
        var data = CreateTestData(1000);
        using var client = CreateClient(new ByteArrayContent(data));
        var progress = new ProgressCollector<Percentage>();

        // Act
        var result = await client.GetByteArrayAsync("http://example.com/", progress);

        // Assert
        result.Should().Equal(data);
        progress.GetValues().Should().NotBeEmpty();
        progress.GetValues().Last().Should().Be(Percentage.FromFraction(1.0));
    }

    [Fact]
    public async Task I_can_download_a_byte_array_via_uri_with_progress()
    {
        // Arrange
        var data = CreateTestData(1000);
        using var client = CreateClient(new ByteArrayContent(data));
        var progress = new ProgressCollector<Percentage>();

        // Act
        var result = await client.GetByteArrayAsync(new Uri("http://example.com/"), progress);

        // Assert
        result.Should().Equal(data);
        progress.GetValues().Should().NotBeEmpty();
        progress.GetValues().Last().Should().Be(Percentage.FromFraction(1.0));
    }

    [Fact]
    public async Task I_can_download_a_byte_array_without_reporting_progress_when_the_handler_is_null()
    {
        // Arrange
        var data = CreateTestData(1000);
        using var client = CreateClient(new ByteArrayContent(data));

        // Act
        var result = await client.GetByteArrayAsync("http://example.com/", null);

        // Assert
        result.Should().Equal(data);
    }

    [Fact]
    public async Task I_can_download_a_string_via_string_url_with_progress()
    {
        // Arrange
        // StringContent sets Content-Length automatically
        using var client = CreateClient(new StringContent("hello world"));
        var progress = new ProgressCollector<Percentage>();

        // Act
        var result = await client.GetStringAsync("http://example.com/", progress);

        // Assert
        result.Should().Be("hello world");
        progress.GetValues().Should().NotBeEmpty();
        progress.GetValues().Last().Should().Be(Percentage.FromFraction(1.0));
    }

    [Fact]
    public async Task I_can_download_a_string_via_uri_with_progress()
    {
        // Arrange
        using var client = CreateClient(new StringContent("hello world"));
        var progress = new ProgressCollector<Percentage>();

        // Act
        var result = await client.GetStringAsync(new Uri("http://example.com/"), progress);

        // Assert
        result.Should().Be("hello world");
        progress.GetValues().Should().NotBeEmpty();
        progress.GetValues().Last().Should().Be(Percentage.FromFraction(1.0));
    }

    [Fact]
    public async Task I_can_download_a_string_without_reporting_progress_when_the_handler_is_null()
    {
        // Arrange
        using var client = CreateClient(new StringContent("hello world"));

        // Act
        var result = await client.GetStringAsync("http://example.com/", null);

        // Assert
        result.Should().Be("hello world");
    }

    [Fact]
    public async Task I_can_download_to_a_stream_via_string_url_with_progress()
    {
        // Arrange
        // ByteArrayContent sets Content-Length automatically
        var data = CreateTestData(1000);
        using var client = CreateClient(new ByteArrayContent(data));
        var destination = new System.IO.MemoryStream();
        var progress = new ProgressCollector<Percentage>();

        // Act
        await client.DownloadAsync("http://example.com/", destination, progress);

        // Assert
        destination.ToArray().Should().Equal(data);
        progress.GetValues().Should().NotBeEmpty();
        progress.GetValues().Last().Should().Be(Percentage.FromFraction(1.0));
    }

    [Fact]
    public async Task I_can_download_to_a_stream_via_uri_with_progress()
    {
        // Arrange
        var data = CreateTestData(1000);
        using var client = CreateClient(new ByteArrayContent(data));
        var destination = new System.IO.MemoryStream();
        var progress = new ProgressCollector<Percentage>();

        // Act
        await client.DownloadAsync(new Uri("http://example.com/"), destination, progress);

        // Assert
        destination.ToArray().Should().Equal(data);
        progress.GetValues().Should().NotBeEmpty();
        progress.GetValues().Last().Should().Be(Percentage.FromFraction(1.0));
    }

    [Fact]
    public async Task I_can_download_to_a_stream_without_reporting_progress_when_the_handler_is_null()
    {
        // Arrange
        var data = CreateTestData(1000);
        using var client = CreateClient(new ByteArrayContent(data));
        var destination = new System.IO.MemoryStream();

        // Act
        await client.DownloadAsync("http://example.com/", destination, null);

        // Assert
        destination.ToArray().Should().Equal(data);
    }

    private class FakeHttpMessageHandler(HttpContent content) : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken
        ) => Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK) { Content = content });
    }
}
