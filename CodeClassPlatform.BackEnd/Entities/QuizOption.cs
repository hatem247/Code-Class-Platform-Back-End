using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CodeClassPlatform.BackEnd.Entities;

public class QuizOption
{
    [Key]
    public int Id { get; set; }

    public int QuestionId { get; set; }

    [MaxLength(500)]
    public string? OptionTextEn { get; set; }

    [MaxLength(500)]
    public string? OptionTextAr { get; set; }

    public bool IsCorrect { get; set; }
    public int Order { get; set; }

    [ForeignKey(nameof(QuestionId))]
    public QuizQuestion? Question { get; set; }
}
