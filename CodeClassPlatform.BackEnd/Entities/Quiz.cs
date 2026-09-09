using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CodeClassPlatform.BackEnd.Entities;

public class Quiz
{
    [Key]
    public int Id { get; set; }

    [MaxLength(200)]
    public string? TitleEn { get; set; }

    [MaxLength(200)]
    public string? TitleAr { get; set; }

    [MaxLength(2000)]
    public string? DescriptionEn { get; set; }

    [MaxLength(2000)]
    public string? DescriptionAr { get; set; }

    public int? CourseId { get; set; }
    public int? LectureId { get; set; }
    public int? TeacherId { get; set; }
    public int? TimeLimitMinutes { get; set; }
    public int? PassingScore { get; set; }
    public int? MaxAttempts { get; set; }
    public bool IsPublished { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    [ForeignKey(nameof(CourseId))]
    public Course? Course { get; set; }

    [ForeignKey(nameof(LectureId))]
    public Lecture? Lecture { get; set; }

    [ForeignKey(nameof(TeacherId))]
    public Account? Teacher { get; set; }

    public ICollection<QuizQuestion> Questions { get; set; } = new List<QuizQuestion>();
    public ICollection<QuizAttempt> Attempts { get; set; } = new List<QuizAttempt>();
}
