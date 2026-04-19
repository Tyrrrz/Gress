using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Gress.Http;
using PowerKit;
using Xunit;

namespace Gress.Tests;

public class ExtensionSpecs
{
    [Fact]
    public void I_can_copy_a_stream_to_another_stream_with_progress()
    {
        // Arrange
        // Use data larger than the buffer (81920 bytes) to get multiple progress reports
        var data = new byte[81920 * 2 + 1000];
        for (var i = 0; i < data.Length; i++)
            data[i] = (byte)(i % 256);
        var source = new MemoryStream(data);
        var destination = new MemoryStream();
        var progress = new ProgressCollector<Percentage>();

        // Act
        source.CopyTo(destination, progress);

        // Assert
        destination.ToArray().Should().Equal(data);
        progress.GetValues().Should().NotBeEmpty();
        progress.GetValues().Last().Should().Be(Percentage.FromFraction(1.0));
        progress.GetValues().Should().BeInAscendingOrder();
    }

    [Fact]
    public async Task I_can_copy_a_stream_to_another_stream_asynchronously_with_progress()
    {
        // Arrange
        // Use data larger than the buffer (81920 bytes) to get multiple progress reports
        var data = new byte[81920 * 2 + 1000];
        for (var i = 0; i < data.Length; i++)
            data[i] = (byte)(i % 256);
        var source = new MemoryStream(data);
        var destination = new MemoryStream();
        var progress = new ProgressCollector<Percentage>();

        // Act
        await source.CopyToAsync(destination, progress);

        // Assert
        destination.ToArray().Should().Equal(data);
        progress.GetValues().Should().NotBeEmpty();
        progress.GetValues().Last().Should().Be(Percentage.FromFraction(1.0));
        progress.GetValues().Should().BeInAscendingOrder();
    }

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
        File.ReadAllBytes(tempFile.Path).Should().NotBeEmpty();
    }
}
