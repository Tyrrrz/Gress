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

public class HttpClientSpecs
{
    private static readonly HttpClient Http = new();

    [Fact]
    public async Task I_can_download_a_byte_array_via_string_url_with_progress()
    {
        // Arrange
        var progress = new ProgressCollector<Percentage>();

        // Act
        var result = await Http.GetByteArrayAsync("http://example.com/", progress);

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
        var result = await Http.GetByteArrayAsync(new Uri("http://example.com/"), progress);

        // Assert
        result.Should().NotBeEmpty();
        progress.GetValues().Should().NotBeEmpty();
        progress.GetValues().Last().Should().Be(Percentage.FromFraction(1.0));
    }

    [Fact]
    public async Task I_can_download_a_byte_array_without_reporting_progress_when_the_handler_is_null()
    {
        // Act
        var result = await Http.GetByteArrayAsync("http://example.com/", null);

        // Assert
        result.Should().NotBeEmpty();
    }

    [Fact]
    public async Task I_can_download_a_string_via_string_url_with_progress()
    {
        // Arrange
        var progress = new ProgressCollector<Percentage>();

        // Act
        var result = await Http.GetStringAsync("http://example.com/", progress);

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
        var result = await Http.GetStringAsync(new Uri("http://example.com/"), progress);

        // Assert
        result.Should().NotBeNullOrEmpty();
        progress.GetValues().Should().NotBeEmpty();
        progress.GetValues().Last().Should().Be(Percentage.FromFraction(1.0));
    }

    [Fact]
    public async Task I_can_download_a_string_without_reporting_progress_when_the_handler_is_null()
    {
        // Act
        var result = await Http.GetStringAsync("http://example.com/", null);

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
        await Http.DownloadAsync("http://example.com/", destination, progress);

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
        await Http.DownloadAsync(new Uri("http://example.com/"), destination, progress);

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
        await Http.DownloadAsync("http://example.com/", destination, null);

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
        await Http.DownloadAsync("http://example.com/", tempFile.Path, progress);

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
        await Http.DownloadAsync(new Uri("http://example.com/"), tempFile.Path, progress);

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
        await Http.DownloadAsync("http://example.com/", tempFile.Path, null);

        // Assert
        File.ReadAllBytes(tempFile.Path).Should().NotBeEmpty();
    }
}
