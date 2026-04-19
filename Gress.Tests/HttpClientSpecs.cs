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
    [Fact]
    public async Task I_can_download_a_byte_array_with_progress()
    {
        // Arrange
        using var http = new HttpClient();
        var progress = new ProgressCollector<Percentage>();

        // Act
        var result = await http.GetByteArrayAsync("http://example.com/", progress);

        // Assert
        result.Should().NotBeEmpty();
        progress.GetValues().Should().NotBeEmpty();
        progress.GetValues().Last().Should().Be(Percentage.FromFraction(1.0));
    }

    [Fact]
    public async Task I_can_download_a_string_with_progress()
    {
        // Arrange
        using var http = new HttpClient();
        var progress = new ProgressCollector<Percentage>();

        // Act
        var result = await http.GetStringAsync("http://example.com/", progress);

        // Assert
        result.Should().NotBeNullOrEmpty();
        progress.GetValues().Should().NotBeEmpty();
        progress.GetValues().Last().Should().Be(Percentage.FromFraction(1.0));
    }

    [Fact]
    public async Task I_can_download_to_a_file_with_progress()
    {
        // Arrange
        using var http = new HttpClient();
        using var tempFile = TempFile.Create();
        var progress = new ProgressCollector<Percentage>();

        // Act
        await http.DownloadAsync("http://example.com/", tempFile.Path, progress);

        // Assert
        new FileInfo(file.Path).Length.Should().BeGreaterThan(0);
        progress.GetValues().Should().NotBeEmpty();
        progress.GetValues().Last().Should().Be(Percentage.FromFraction(1.0));
    }
}
