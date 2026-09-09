using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CodeClassPlatform.BackEnd.Entities;

public class Notification
{
    [Key]
    public int Id { get; set; }

    public int AccountId { get; set; }

    [MaxLength(100)]
    public string Type { get; set; } = "INFO";

    [MaxLength(200)]
    public string? TitleEn { get; set; }

    [MaxLength(200)]
    public string? TitleAr { get; set; }

    [MaxLength(1000)]
    public string? BodyEn { get; set; }

    [MaxLength(1000)]
    public string? BodyAr { get; set; }

    public bool IsRead { get; set; }
    public DateTime? ReadAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Column(TypeName = "nvarchar(max)")]
    public string? DataJson { get; set; }

    [ForeignKey(nameof(AccountId))]
    public Account? Account { get; set; }
}
