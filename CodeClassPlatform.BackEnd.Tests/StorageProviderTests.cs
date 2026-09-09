using CodeClassPlatform.BackEnd.Services;

namespace CodeClassPlatform.BackEnd.Tests;

public class GoogleDriveLinkServiceTests
{
    [Fact]
    public void Validate_AllowsValidDriveFileUrl()
    {
        var service = new GoogleDriveLinkService();

        var result = service.Validate("https://drive.google.com/file/d/abc123xyz/view");

        Assert.True(result.IsValid);
        Assert.Equal("abc123xyz", result.FileId);
        Assert.Equal("https://drive.google.com/file/d/abc123xyz/view", result.CanonicalUrl);
    }

    [Fact]
    public void Validate_RejectsNonDriveUrl()
    {
        var service = new GoogleDriveLinkService();

        var result = service.Validate("https://example.com/file.pdf");

        Assert.False(result.IsValid);
        Assert.NotNull(result.ErrorMessage);
    }

    [Fact]
    public void Validate_RejectsUnsafeProtocol()
    {
        var service = new GoogleDriveLinkService();

        var result = service.Validate("javascript:alert(1)");

        Assert.False(result.IsValid);
    }

    [Fact]
    public void ExtractFileId_SupportsOpenIdFormat()
    {
        var service = new GoogleDriveLinkService();

        var fileId = service.ExtractFileId("https://drive.google.com/open?id=abc123xyz");

        Assert.Equal("abc123xyz", fileId);
    }
}
