using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CodeClassPlatform.BackEnd.Entities;

public class Lecture
{
    [Key]
    public int Id { get; set; }

    public int? CourseId { get; set; }
    public int? TeacherId { get; set; }

    [MaxLength(200)]
    public string? TitleEn { get; set; }

    [MaxLength(200)]
    public string? TitleAr { get; set; }

    [MaxLength(2000)]
    public string? DescriptionEn { get; set; }

    [MaxLength(2000)]
    public string? DescriptionAr { get; set; }

    public int? VideoAssetId { get; set; }
    public int? ThumbnailAssetId { get; set; }
    public int? DurationSeconds { get; set; }
    public int Order { get; set; }
    public bool IsPublished { get; set; }
    public bool IsFree { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    [ForeignKey(nameof(CourseId))]
    public Course? Course { get; set; }

    [ForeignKey(nameof(TeacherId))]
    public Account? Teacher { get; set; }

    [ForeignKey(nameof(VideoAssetId))]
    public ExternalResource? VideoAsset { get; set; }

    [ForeignKey(nameof(ThumbnailAssetId))]
    public ExternalResource? ThumbnailAsset { get; set; }

    public ICollection<StudentLectureProgress> Progress { get; set; } = new List<StudentLectureProgress>();
}
