using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CodeClassPlatform.BackEnd.Entities;

public class QuizAnswer
{
    [Key]
    public int Id { get; set; }

    public int QuestionId { get; set; }
    public int AttemptId { get; set; }
    public int? SelectedOptionId { get; set; }

    [MaxLength(1000)]
    public string? AnswerText { get; set; }

    public bool IsCorrect { get; set; }
    public int EarnedPoints { get; set; }

    [ForeignKey(nameof(QuestionId))]
    public QuizQuestion? Question { get; set; }

    [ForeignKey(nameof(AttemptId))]
    public QuizAttempt? Attempt { get; set; }

    [ForeignKey(nameof(SelectedOptionId))]
    public QuizOption? SelectedOption { get; set; }
}
