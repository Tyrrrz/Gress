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
    private const string TestUrl = "http://example.com/";

    private readonly HttpClient _http = new();

    [Fact]
    public async Task I_can_download_a_byte_array_with_progress()
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
    public async Task I_can_download_a_string_with_progress()
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
    public async Task I_can_download_to_a_stream_with_progress()
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
    public async Task I_can_download_to_a_file_with_progress()
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
}
