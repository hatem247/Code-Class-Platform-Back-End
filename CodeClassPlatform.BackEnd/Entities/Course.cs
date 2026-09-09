using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CodeClassPlatform.BackEnd.Entities;

public class Course
{
    [Key]
    public int Id { get; set; }

    [MaxLength(200)]
    public string? NameEn { get; set; }

    [MaxLength(200)]
    public string? NameAr { get; set; }

    [MaxLength(2000)]
    public string? DescriptionEn { get; set; }

    [MaxLength(2000)]
    public string? DescriptionAr { get; set; }

    public int? ThumbnailAssetId { get; set; }
    public int? CoverAssetId { get; set; }
    public int? CreatedBy { get; set; }
    public int? UpdatedBy { get; set; }
    public bool IsActive { get; set; } = true;
    public bool IsPublished { get; set; }
    public int Ordering { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    [ForeignKey(nameof(CreatedBy))]
    public Account? CreatedByAccount { get; set; }

    [ForeignKey(nameof(UpdatedBy))]
    public Account? UpdatedByAccount { get; set; }

    [ForeignKey(nameof(ThumbnailAssetId))]
    public ExternalResource? ThumbnailAsset { get; set; }

    [ForeignKey(nameof(CoverAssetId))]
    public ExternalResource? CoverAsset { get; set; }

    public ICollection<Lecture> Lectures { get; set; } = new List<Lecture>();
    public ICollection<Quiz> Quizzes { get; set; } = new List<Quiz>();
    public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
}
