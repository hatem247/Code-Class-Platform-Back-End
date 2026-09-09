using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CodeClassPlatform.BackEnd.Entities;

public class StudentLectureProgress
{
    [Key]
    public int Id { get; set; }

    public int StudentId { get; set; }
    public int LectureId { get; set; }

    public int WatchedSeconds { get; set; }
    public int CompletionPercentage { get; set; }
    public bool IsCompleted { get; set; }
    public DateTime? FirstOpenedAt { get; set; }
    public DateTime? LastWatchedAt { get; set; }
    public int LastPositionSeconds { get; set; }

    [ForeignKey(nameof(StudentId))]
    public Account? Student { get; set; }

    [ForeignKey(nameof(LectureId))]
    public Lecture? Lecture { get; set; }
}
