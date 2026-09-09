using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CodeClassPlatform.BackEnd.Entities;

public class QuizQuestion
{
    [Key]
    public int Id { get; set; }

    public int QuizId { get; set; }

    [MaxLength(1000)]
    public string? QuestionTextEn { get; set; }

    [MaxLength(1000)]
    public string? QuestionTextAr { get; set; }

    [MaxLength(1000)]
    public string? ExplanationEn { get; set; }

    [MaxLength(1000)]
    public string? ExplanationAr { get; set; }

    [MaxLength(50)]
    public string QuestionType { get; set; } = "MULTIPLE_CHOICE";

    public int Points { get; set; } = 1;
    public int Order { get; set; }

    [ForeignKey(nameof(QuizId))]
    public Quiz? Quiz { get; set; }

    public ICollection<QuizOption> Options { get; set; } = new List<QuizOption>();
    public ICollection<QuizAnswer> Answers { get; set; } = new List<QuizAnswer>();
}
