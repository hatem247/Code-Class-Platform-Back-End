using System.ComponentModel.DataAnnotations;

namespace CodeClassPlatform.BackEnd.Entities;

public class ExternalResource
{
    [Key]
    public int Id { get; set; }

    [MaxLength(50)]
    public string Provider { get; set; } = "GoogleDrive";

    [MaxLength(50)]
    public string ResourceType { get; set; } = "File";

    [MaxLength(4000)]
    public string OriginalUrl { get; set; } = string.Empty;

    [MaxLength(4000)]
    public string CanonicalUrl { get; set; } = string.Empty;

    [MaxLength(200)]
    public string? ExternalId { get; set; }

    [MaxLength(200)]
    public string? FileName { get; set; }

    [MaxLength(200)]
    public string? ContentType { get; set; }

    public long? SizeBytes { get; set; }
    public int? DurationSeconds { get; set; }

    [MaxLength(1000)]
    public string? ThumbnailUrl { get; set; }

    [MaxLength(200)]
    public string? TitleEn { get; set; }

    [MaxLength(200)]
    public string? TitleAr { get; set; }

    [MaxLength(2000)]
    public string? DescriptionEn { get; set; }

    [MaxLength(2000)]
    public string? DescriptionAr { get; set; }

    public bool IsActive { get; set; } = true;

    [MaxLength(50)]
    public string VerificationStatus { get; set; } = "Unknown";

    public DateTime? LastVerifiedAt { get; set; }
    public int? CreatedByAccountId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public string? MetadataJson { get; set; }
}
