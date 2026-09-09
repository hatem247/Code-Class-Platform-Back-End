using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CodeClassPlatform.BackEnd.Entities;

public class QuizAttempt
{
    [Key]
    public int Id { get; set; }

    public int StudentId { get; set; }
    public int QuizId { get; set; }

    public DateTime StartedAt { get; set; } = DateTime.UtcNow;
    public DateTime? SubmittedAt { get; set; }
    public int? Score { get; set; }
    public decimal? Percentage { get; set; }
    public bool Passed { get; set; }
    public int AttemptNumber { get; set; }

    [ForeignKey(nameof(StudentId))]
    public Account? Student { get; set; }

    [ForeignKey(nameof(QuizId))]
    public Quiz? Quiz { get; set; }

    public ICollection<QuizAnswer> Answers { get; set; } = new List<QuizAnswer>();
}
