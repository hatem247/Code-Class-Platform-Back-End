using System.Text.RegularExpressions;

namespace CodeClassPlatform.BackEnd.Services;

public interface IGoogleDriveLinkService
{
    GoogleDriveLinkValidationResult Validate(string? url);
    string? ExtractFileId(string? url);
    string? NormalizeToCanonicalUrl(string? url);
}

public sealed record GoogleDriveLinkValidationResult(
    bool IsValid,
    string? FileId,
    string? CanonicalUrl,
    string? ResourceType,
    string? ErrorMessage
);

public class GoogleDriveLinkService : IGoogleDriveLinkService
{
    private static readonly Regex DriveFilePathRegex = new(
        @"(?:^|/)(?:file|document|presentation|spreadsheets|drawings|forms|uc)/d/([A-Za-z0-9_-]+)|(?:^|/)(?:file|document|presentation|spreadsheets|drawings|forms)/d/([A-Za-z0-9_-]+)",
        RegexOptions.IgnoreCase | RegexOptions.Compiled);

    public GoogleDriveLinkValidationResult Validate(string? url)
    {
        if (string.IsNullOrWhiteSpace(url))
        {
            return new GoogleDriveLinkValidationResult(false, null, null, null, "A Google Drive URL is required.");
        }

        var trimmed = url.Trim();
        if (!Uri.TryCreate(trimmed, UriKind.Absolute, out var uri) || !uri.Scheme.Equals("https", StringComparison.OrdinalIgnoreCase))
        {
            return new GoogleDriveLinkValidationResult(false, null, null, null, "Only secure HTTPS Google Drive links are allowed.");
        }

        var host = uri.Host.Trim().ToLowerInvariant();
        if (!IsAllowedHost(host))
        {
            return new GoogleDriveLinkValidationResult(false, null, null, null, "Only Google Drive URLs are allowed.");
        }

        var fileId = ExtractFileId(trimmed);
        if (string.IsNullOrWhiteSpace(fileId))
        {
            return new GoogleDriveLinkValidationResult(false, null, null, null, "The Google Drive URL is malformed or does not contain a valid file ID.");
        }

        var canonicalUrl = BuildCanonicalUrl(host, trimmed, fileId);
        var resourceType = DetermineResourceType(host, trimmed);

        return new GoogleDriveLinkValidationResult(true, fileId, canonicalUrl, resourceType, null);
    }

    public string? ExtractFileId(string? url)
    {
        if (string.IsNullOrWhiteSpace(url))
        {
            return null;
        }

        if (!Uri.TryCreate(url.Trim(), UriKind.Absolute, out var uri))
        {
            return null;
        }

        var host = uri.Host.Trim().ToLowerInvariant();
        if (!IsAllowedHost(host))
        {
            return null;
        }

        if (uri.Query.Contains("id=", StringComparison.OrdinalIgnoreCase))
        {
            var query = System.Web.HttpUtility.ParseQueryString(uri.Query);
            var id = query["id"];
            if (!string.IsNullOrWhiteSpace(id))
            {
                return id;
            }
        }

        var match = DriveFilePathRegex.Match(uri.AbsoluteUri);
        if (match.Success)
        {
            var value = match.Groups[1].Value;
            if (!string.IsNullOrWhiteSpace(value))
            {
                return value;
            }

            return match.Groups[2].Value;
        }

        if (uri.AbsolutePath.Contains("/d/", StringComparison.OrdinalIgnoreCase))
        {
            var segments = uri.AbsolutePath.Split('/', StringSplitOptions.RemoveEmptyEntries);
            for (var i = 0; i < segments.Length - 1; i++)
            {
                if (segments[i].Equals("d", StringComparison.OrdinalIgnoreCase) && !string.IsNullOrWhiteSpace(segments[i + 1]))
                {
                    return segments[i + 1];
                }
            }
        }

        return null;
    }

    public string? NormalizeToCanonicalUrl(string? url)
    {
        if (string.IsNullOrWhiteSpace(url))
        {
            return null;
        }

        var validation = Validate(url);
        if (!validation.IsValid || string.IsNullOrWhiteSpace(validation.FileId))
        {
            return null;
        }

        return validation.CanonicalUrl;
    }

    private static bool IsAllowedHost(string host)
    {
        return host == "drive.google.com"
            || host == "docs.google.com"
            || host == "www.google.com"
            || host == "google.com";
    }

    private static string BuildCanonicalUrl(string host, string originalUrl, string fileId)
    {
        if (host == "docs.google.com")
        {
            return $"https://docs.google.com/document/d/{fileId}/edit";
        }

        return $"https://drive.google.com/file/d/{fileId}/view";
    }

    private static string? DetermineResourceType(string host, string originalUrl)
    {
        if (host == "docs.google.com")
        {
            return "Document";
        }

        var lowerUrl = originalUrl.ToLowerInvariant();
        if (lowerUrl.Contains("/presentation/")) return "Presentation";
        if (lowerUrl.Contains("/spreadsheets/")) return "Document";
        if (lowerUrl.Contains("/forms/")) return "Document";
        if (lowerUrl.Contains("/drawings/")) return "Image";
        if (lowerUrl.Contains("/file/d/")) return "File";
        return "File";
    }
}
