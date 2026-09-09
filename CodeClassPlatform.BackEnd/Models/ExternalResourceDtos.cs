using System.ComponentModel.DataAnnotations;

namespace CodeClassPlatform.BackEnd.Models;

public class CreateExternalResourceRequest
{
    [Required]
    [MinLength(1)]
    public string Url { get; set; } = string.Empty;

    public string ResourceType { get; set; } = "File";
    public string? TitleEn { get; set; }
    public string? TitleAr { get; set; }
    public string? DescriptionEn { get; set; }
    public string? DescriptionAr { get; set; }
    public string? FileName { get; set; }
    public string? ContentType { get; set; }
    public int? DurationSeconds { get; set; }
    public string? ThumbnailUrl { get; set; }
}

public class UpdateExternalResourceRequest
{
    public string? Url { get; set; }
    public string? ResourceType { get; set; }
    public string? TitleEn { get; set; }
    public string? TitleAr { get; set; }
    public string? DescriptionEn { get; set; }
    public string? DescriptionAr { get; set; }
    public bool? IsActive { get; set; }
    public string? FileName { get; set; }
    public string? ContentType { get; set; }
    public string? ThumbnailUrl { get; set; }
}

public class ExternalResourceResponse
{
    public int Id { get; set; }
    public string Provider { get; set; } = string.Empty;
    public string ResourceType { get; set; } = string.Empty;
    public string OriginalUrl { get; set; } = string.Empty;
    public string CanonicalUrl { get; set; } = string.Empty;
    public string? ExternalId { get; set; }
    public string? FileName { get; set; }
    public string? ContentType { get; set; }
    public long? SizeBytes { get; set; }
    public int? DurationSeconds { get; set; }
    public string? ThumbnailUrl { get; set; }
    public string? TitleEn { get; set; }
    public string? TitleAr { get; set; }
    public string? DescriptionEn { get; set; }
    public string? DescriptionAr { get; set; }
    public bool IsActive { get; set; }
    public string VerificationStatus { get; set; } = "Unknown";
    public DateTime? LastVerifiedAt { get; set; }
    public int? CreatedByAccountId { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class VerifyExternalResourceResponse
{
    public int Id { get; set; }
    public string VerificationStatus { get; set; } = string.Empty;
    public DateTime? LastVerifiedAt { get; set; }
    public string Message { get; set; } = string.Empty;
}
