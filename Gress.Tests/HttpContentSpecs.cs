using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Gress.Http;
using Xunit;

namespace Gress.Tests;

public class HttpContentSpecs
{
    private static byte[] CreateTestData(int length)
    {
        var data = new byte[length];
        for (var i = 0; i < data.Length; i++)
            data[i] = (byte)(i % 256);
        return data;
    }

    [Fact]
    public async Task I_can_copy_http_content_to_a_stream_with_progress()
    {
        // Arrange
        // ByteArrayContent sets Content-Length automatically
        var data = CreateTestData(1000);
        using var content = new ByteArrayContent(data);
        var destination = new MemoryStream();
        var progress = new ProgressCollector<Percentage>();

        // Act
        await content.CopyToAsync(destination, progress);

        // Assert
        destination.ToArray().Should().Equal(data);
        progress.GetValues().Should().NotBeEmpty();
        progress.GetValues().Last().Should().Be(Percentage.FromFraction(1.0));
    }

    [Fact]
    public async Task I_can_copy_http_content_to_a_stream_without_reporting_progress_when_the_handler_is_null()
    {
        // Arrange
        var data = CreateTestData(1000);
        using var content = new ByteArrayContent(data);
        var destination = new MemoryStream();

        // Act
        await content.CopyToAsync(destination, null);

        // Assert
        destination.ToArray().Should().Equal(data);
    }

    [Fact]
    public async Task I_can_copy_http_content_to_a_stream_without_reporting_progress_when_the_content_length_is_unknown()
    {
        // Arrange
        var data = CreateTestData(1000);
        using var content = new UnknownLengthContent(data);
        var destination = new MemoryStream();
        var progress = new ProgressCollector<Percentage>();

        // Act
        await content.CopyToAsync(destination, progress);

        // Assert
        destination.ToArray().Should().Equal(data);
        progress.GetValues().Should().BeEmpty();
    }

    [Fact]
    public async Task I_can_read_http_content_as_a_byte_array_with_progress()
    {
        // Arrange
        // ByteArrayContent sets Content-Length automatically
        var data = CreateTestData(1000);
        using var content = new ByteArrayContent(data);
        var progress = new ProgressCollector<Percentage>();

        // Act
        var result = await content.ReadAsByteArrayAsync(progress);

        // Assert
        result.Should().Equal(data);
        progress.GetValues().Should().NotBeEmpty();
        progress.GetValues().Last().Should().Be(Percentage.FromFraction(1.0));
    }

    [Fact]
    public async Task I_can_read_http_content_as_a_byte_array_without_reporting_progress_when_the_handler_is_null()
    {
        // Arrange
        var data = CreateTestData(1000);
        using var content = new ByteArrayContent(data);

        // Act
        var result = await content.ReadAsByteArrayAsync(null);

        // Assert
        result.Should().Equal(data);
    }

    [Fact]
    public async Task I_can_read_http_content_as_a_byte_array_without_reporting_progress_when_the_content_length_is_unknown()
    {
        // Arrange
        var data = CreateTestData(1000);
        using var content = new UnknownLengthContent(data);
        var progress = new ProgressCollector<Percentage>();

        // Act
        var result = await content.ReadAsByteArrayAsync(progress);

        // Assert
        result.Should().Equal(data);
        progress.GetValues().Should().BeEmpty();
    }

    [Fact]
    public async Task I_can_read_http_content_as_a_string_with_progress()
    {
        // Arrange
        // StringContent sets Content-Length automatically
        using var content = new StringContent("hello world");
        var progress = new ProgressCollector<Percentage>();

        // Act
        var result = await content.ReadAsStringAsync(progress);

        // Assert
        result.Should().Be("hello world");
        progress.GetValues().Should().NotBeEmpty();
        progress.GetValues().Last().Should().Be(Percentage.FromFraction(1.0));
    }

    [Fact]
    public async Task I_can_read_http_content_as_a_string_without_reporting_progress_when_the_handler_is_null()
    {
        // Arrange
        using var content = new StringContent("hello world");

        // Act
        var result = await content.ReadAsStringAsync(null);

        // Assert
        result.Should().Be("hello world");
    }

    [Fact]
    public async Task I_can_read_http_content_as_a_string_without_reporting_progress_when_the_content_length_is_unknown()
    {
        // Arrange
        var data = System.Text.Encoding.UTF8.GetBytes("hello world");
        using var content = new UnknownLengthContent(data);
        var progress = new ProgressCollector<Percentage>();

        // Act
        var result = await content.ReadAsStringAsync(progress);

        // Assert
        result.Should().Be("hello world");
        progress.GetValues().Should().BeEmpty();
    }

    private class UnknownLengthContent(byte[] data) : HttpContent
    {
        protected override Task SerializeToStreamAsync(Stream stream, TransportContext? context) =>
            stream.WriteAsync(data, 0, data.Length);

        protected override bool TryComputeLength(out long length)
        {
            length = 0;
            return false;
        }
    }
}
