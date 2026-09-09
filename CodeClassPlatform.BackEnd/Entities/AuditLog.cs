using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CodeClassPlatform.BackEnd.Entities;

public class AuditLog
{
    [Key]
    public int Id { get; set; }

    public int? ActorAccountId { get; set; }

    [MaxLength(200)]
    public string Action { get; set; } = string.Empty;

    [MaxLength(200)]
    public string TargetType { get; set; } = string.Empty;

    public int? TargetId { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    [MaxLength(200)]
    public string? IpAddress { get; set; }

    [MaxLength(500)]
    public string? UserAgent { get; set; }

    [Column(TypeName = "nvarchar(max)")]
    public string? Metadata { get; set; }

    [ForeignKey(nameof(ActorAccountId))]
    public Account? ActorAccount { get; set; }
}
