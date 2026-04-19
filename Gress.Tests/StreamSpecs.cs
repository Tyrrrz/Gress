using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace Gress.Tests;

public class StreamSpecs
{
    private static byte[] CreateTestData(int length)
    {
        var data = new byte[length];
        for (var i = 0; i < data.Length; i++)
            data[i] = (byte)(i % 256);
        return data;
    }

    [Fact]
    public void I_can_copy_a_stream_to_another_stream_with_progress()
    {
        // Arrange
        // Use data larger than the buffer (81920 bytes) to get multiple progress reports
        var data = CreateTestData(81920 * 2 + 1000);
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
    public void I_can_copy_a_stream_to_another_stream_without_reporting_progress_when_the_handler_is_null()
    {
        // Arrange
        var data = CreateTestData(1000);
        var source = new MemoryStream(data);
        var destination = new MemoryStream();

        // Act
        source.CopyTo(destination, null);

        // Assert
        destination.ToArray().Should().Equal(data);
    }

    [Fact]
    public void I_can_copy_a_non_seekable_stream_to_another_stream_without_reporting_progress()
    {
        // Arrange
        var data = CreateTestData(1000);
        var source = new NonSeekableStream(new MemoryStream(data));
        var destination = new MemoryStream();
        var progress = new ProgressCollector<Percentage>();

        // Act
        source.CopyTo(destination, progress);

        // Assert
        destination.ToArray().Should().Equal(data);
        progress.GetValues().Should().BeEmpty();
    }

    [Fact]
    public async Task I_can_copy_a_stream_to_another_stream_asynchronously_with_progress()
    {
        // Arrange
        // Use data larger than the buffer (81920 bytes) to get multiple progress reports
        var data = CreateTestData(81920 * 2 + 1000);
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
    public async Task I_can_copy_a_stream_to_another_stream_asynchronously_without_reporting_progress_when_the_handler_is_null()
    {
        // Arrange
        var data = CreateTestData(1000);
        var source = new MemoryStream(data);
        var destination = new MemoryStream();

        // Act
        await source.CopyToAsync(destination, null);

        // Assert
        destination.ToArray().Should().Equal(data);
    }

    [Fact]
    public async Task I_can_copy_a_non_seekable_stream_to_another_stream_asynchronously_without_reporting_progress()
    {
        // Arrange
        var data = CreateTestData(1000);
        var source = new NonSeekableStream(new MemoryStream(data));
        var destination = new MemoryStream();
        var progress = new ProgressCollector<Percentage>();

        // Act
        await source.CopyToAsync(destination, progress);

        // Assert
        destination.ToArray().Should().Equal(data);
        progress.GetValues().Should().BeEmpty();
    }

    [Fact]
    public void I_can_copy_a_non_seekable_stream_to_another_stream_with_progress_using_an_explicit_stream_length()
    {
        // Arrange
        // Use data larger than the buffer (81920 bytes) to get multiple progress reports
        var data = CreateTestData(81920 * 2 + 1000);
        var source = new NonSeekableStream(new MemoryStream(data));
        var destination = new MemoryStream();
        var progress = new ProgressCollector<Percentage>();

        // Act
        source.CopyTo(destination, data.Length, progress);

        // Assert
        destination.ToArray().Should().Equal(data);
        progress.GetValues().Should().NotBeEmpty();
        progress.GetValues().Last().Should().Be(Percentage.FromFraction(1.0));
        progress.GetValues().Should().BeInAscendingOrder();
    }

    [Fact]
    public async Task I_can_copy_a_non_seekable_stream_to_another_stream_asynchronously_with_progress_using_an_explicit_stream_length()
    {
        // Arrange
        // Use data larger than the buffer (81920 bytes) to get multiple progress reports
        var data = CreateTestData(81920 * 2 + 1000);
        var source = new NonSeekableStream(new MemoryStream(data));
        var destination = new MemoryStream();
        var progress = new ProgressCollector<Percentage>();

        // Act
        await source.CopyToAsync(destination, data.Length, progress);

        // Assert
        destination.ToArray().Should().Equal(data);
        progress.GetValues().Should().NotBeEmpty();
        progress.GetValues().Last().Should().Be(Percentage.FromFraction(1.0));
        progress.GetValues().Should().BeInAscendingOrder();
    }

    private class NonSeekableStream(Stream inner) : Stream
    {
        public override bool CanRead => inner.CanRead;
        public override bool CanSeek => false;
        public override bool CanWrite => inner.CanWrite;
        public override long Length => throw new System.NotSupportedException();
        public override long Position
        {
            get => throw new System.NotSupportedException();
            set => throw new System.NotSupportedException();
        }

        public override void Flush() => inner.Flush();

        public override int Read(byte[] buffer, int offset, int count) =>
            inner.Read(buffer, offset, count);

        public override long Seek(long offset, SeekOrigin origin) =>
            throw new System.NotSupportedException();

        public override void SetLength(long value) => throw new System.NotSupportedException();

        public override void Write(byte[] buffer, int offset, int count) =>
            inner.Write(buffer, offset, count);
    }
}
