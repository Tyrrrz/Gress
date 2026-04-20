using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Gress.Integration;
using PowerKit;
using PowerKit.Extensions;
using Xunit;

namespace Gress.Tests;

public class IntegrationSpecs
{
    [Fact]
    public async Task I_can_copy_a_stream_to_another_stream_with_progress()
    {
        // Arrange
        using var buffer = SpanPool<byte>.Shared.Rent(
            // Longer buffer to ensure multiple progress reports
            81920 * 2
                + 1000
        );

        Random.Shared.NextBytes(buffer.Span);
        var data = buffer.Span.ToArray();

        using var source = new MemoryStream(data);
        using var destination = new MemoryStream();
        var progress = new ProgressCollector<Percentage>();

        // Act
        await source.CopyToAsync(destination, progress);

        // Assert
        destination.ToArray().Should().Equal(data);
        progress.GetValues().Should().NotBeEmpty();
        progress.GetValues().Should().BeInAscendingOrder();
    }

    [Fact]
    public async Task I_can_download_a_web_resource_as_a_byte_array_with_progress()
    {
        // Arrange
        using var http = new HttpClient();
        var progress = new ProgressCollector<Percentage>();

        // Act
        var result = await http.GetByteArrayAsync(
            // Need something that reports content length
            "https://github.com/Tyrrrz/CliWrap/releases/download/3.10.1/CliWrap.3.10.1.nupkg",
            progress
        );

        // Assert
        result.Should().NotBeNullOrEmpty();
        progress.GetValues().Should().NotBeEmpty();
    }

    [Fact]
    public async Task I_can_download_a_web_resource_to_a_file_with_progress()
    {
        // Arrange
        using var http = new HttpClient();
        using var tempFile = TempFile.Create();
        var progress = new ProgressCollector<Percentage>();

        // Act
        await http.DownloadAsync(
            // Need something that reports content length
            "https://github.com/Tyrrrz/CliWrap/releases/download/3.10.1/CliWrap.3.10.1.nupkg",
            tempFile.Path,
            progress
        );

        // Assert
        new FileInfo(tempFile.Path)
            .Length.Should()
            .BeGreaterThan(0);

        progress.GetValues().Should().NotBeEmpty();
    }
}
