using Moq;
using FluentAssertions;

namespace KlabTestFramework.Core.Tests;

public class DemoTests
{
    private Demo _sut;
    private Mock<IDemoDownloader> _demoDownloaderMock;

    public DemoTests()
    {
        _demoDownloaderMock = new();
        _sut = new("Test", _demoDownloaderMock.Object);
    }

    [Fact]
    public async Task TestMethod1()
    {
        _demoDownloaderMock.Setup(m => m.DownloadNameAsync()).ReturnsAsync("Test");
        string result = await _sut.GetDownloadedNameAsync();
        result.Should().Be("Test");
    }
}
