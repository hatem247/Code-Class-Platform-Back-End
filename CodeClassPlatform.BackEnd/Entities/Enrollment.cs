using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CodeClassPlatform.BackEnd.Entities;

public class Enrollment
{
    [Key]
    public int Id { get; set; }

    public int StudentId { get; set; }
    public int CourseId { get; set; }

    [MaxLength(50)]
    public string Status { get; set; } = "ACTIVE";

    public DateTime EnrolledAt { get; set; } = DateTime.UtcNow;
    public DateTime? LastActivityAt { get; set; }
    public decimal? ProgressPercentage { get; set; }
    public bool IsCompleted { get; set; }

    [ForeignKey(nameof(StudentId))]
    public Account? Student { get; set; }

    [ForeignKey(nameof(CourseId))]
    public Course? Course { get; set; }
}
