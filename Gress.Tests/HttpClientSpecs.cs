using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Gress.Http;
using PowerKit;
using Xunit;

namespace Gress.Tests;

public class HttpClientSpecs(HttpClientSpecs.Fixture fixture)
    : IClassFixture<HttpClientSpecs.Fixture>
{
    private const string TestUrl = "http://example.com/";

    private readonly HttpClient _http = fixture.Http;

    public class Fixture : IDisposable
    {
        public HttpClient Http { get; } = new();

        public void Dispose() => Http.Dispose();
    }

    [Fact]
    public async Task I_can_download_a_byte_array_via_string_url_with_progress()
    {
        // Arrange
        var progress = new ProgressCollector<Percentage>();

        // Act
        var result = await _http.GetByteArrayAsync(TestUrl, progress);

        // Assert
        result.Should().NotBeEmpty();
        progress.GetValues().Should().NotBeEmpty();
        progress.GetValues().Last().Should().Be(Percentage.FromFraction(1.0));
    }

    [Fact]
    public async Task I_can_download_a_byte_array_via_uri_with_progress()
    {
        // Arrange
        var progress = new ProgressCollector<Percentage>();

        // Act
        var result = await _http.GetByteArrayAsync(new Uri(TestUrl), progress);

        // Assert
        result.Should().NotBeEmpty();
        progress.GetValues().Should().NotBeEmpty();
        progress.GetValues().Last().Should().Be(Percentage.FromFraction(1.0));
    }

    [Fact]
    public async Task I_can_download_a_byte_array_without_reporting_progress_when_the_handler_is_null()
    {
        // Act
        var result = await _http.GetByteArrayAsync(TestUrl, null);

        // Assert
        result.Should().NotBeEmpty();
    }

    [Fact]
    public async Task I_can_download_a_string_via_string_url_with_progress()
    {
        // Arrange
        var progress = new ProgressCollector<Percentage>();

        // Act
        var result = await _http.GetStringAsync(TestUrl, progress);

        // Assert
        result.Should().NotBeNullOrEmpty();
        progress.GetValues().Should().NotBeEmpty();
        progress.GetValues().Last().Should().Be(Percentage.FromFraction(1.0));
    }

    [Fact]
    public async Task I_can_download_a_string_via_uri_with_progress()
    {
        // Arrange
        var progress = new ProgressCollector<Percentage>();

        // Act
        var result = await _http.GetStringAsync(new Uri(TestUrl), progress);

        // Assert
        result.Should().NotBeNullOrEmpty();
        progress.GetValues().Should().NotBeEmpty();
        progress.GetValues().Last().Should().Be(Percentage.FromFraction(1.0));
    }

    [Fact]
    public async Task I_can_download_a_string_without_reporting_progress_when_the_handler_is_null()
    {
        // Act
        var result = await _http.GetStringAsync(TestUrl, null);

        // Assert
        result.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task I_can_download_to_a_stream_via_string_url_with_progress()
    {
        // Arrange
        var destination = new MemoryStream();
        var progress = new ProgressCollector<Percentage>();

        // Act
        await _http.DownloadAsync(TestUrl, destination, progress);

        // Assert
        destination.ToArray().Should().NotBeEmpty();
        progress.GetValues().Should().NotBeEmpty();
        progress.GetValues().Last().Should().Be(Percentage.FromFraction(1.0));
    }

    [Fact]
    public async Task I_can_download_to_a_stream_via_uri_with_progress()
    {
        // Arrange
        var destination = new MemoryStream();
        var progress = new ProgressCollector<Percentage>();

        // Act
        await _http.DownloadAsync(new Uri(TestUrl), destination, progress);

        // Assert
        destination.ToArray().Should().NotBeEmpty();
        progress.GetValues().Should().NotBeEmpty();
        progress.GetValues().Last().Should().Be(Percentage.FromFraction(1.0));
    }

    [Fact]
    public async Task I_can_download_to_a_stream_without_reporting_progress_when_the_handler_is_null()
    {
        // Arrange
        var destination = new MemoryStream();

        // Act
        await _http.DownloadAsync(TestUrl, destination, null);

        // Assert
        destination.ToArray().Should().NotBeEmpty();
    }

    [Fact]
    public async Task I_can_download_to_a_file_via_string_url_with_progress()
    {
        // Arrange
        using var tempFile = TempFile.Create();
        var progress = new ProgressCollector<Percentage>();

        // Act
        await _http.DownloadAsync(TestUrl, tempFile.Path, progress);

        // Assert
        File.ReadAllBytes(tempFile.Path).Should().NotBeEmpty();
        progress.GetValues().Should().NotBeEmpty();
        progress.GetValues().Last().Should().Be(Percentage.FromFraction(1.0));
    }

    [Fact]
    public async Task I_can_download_to_a_file_via_uri_with_progress()
    {
        // Arrange
        using var tempFile = TempFile.Create();
        var progress = new ProgressCollector<Percentage>();

        // Act
        await _http.DownloadAsync(new Uri(TestUrl), tempFile.Path, progress);

        // Assert
        File.ReadAllBytes(tempFile.Path).Should().NotBeEmpty();
        progress.GetValues().Should().NotBeEmpty();
        progress.GetValues().Last().Should().Be(Percentage.FromFraction(1.0));
    }

    [Fact]
    public async Task I_can_download_to_a_file_without_reporting_progress_when_the_handler_is_null()
    {
        // Arrange
        using var tempFile = TempFile.Create();

        // Act
        await _http.DownloadAsync(TestUrl, tempFile.Path, null);

        // Assert
        File.ReadAllBytes(tempFile.Path).Should().NotBeEmpty();
    }
}
