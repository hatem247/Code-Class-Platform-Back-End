using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace CodeClassPlatform.BackEnd.Entities;

public class Account
{
    [Key]
    public int Id { get; set; }

    [MaxLength(200)]
    public string? Email { get; set; }

    [JsonIgnore]
    [MaxLength(500)]
    public string? PasswordHash { get; set; }

    [MaxLength(50)]
    public string? PhoneNumber { get; set; }

    [MaxLength(50)]
    public string Role { get; set; } = "STUDENT";

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public DateTime? LastLoginAt { get; set; }

    public int? ProfileImageAssetId { get; set; }
    public int? CreatedByAccountId { get; set; }
    public int? LastModifiedByAccountId { get; set; }

    [ForeignKey(nameof(ProfileImageAssetId))]
    public ExternalResource? ProfileImageAsset { get; set; }

    [JsonIgnore]
    public ICollection<StudentProfile> StudentProfiles { get; set; } = new List<StudentProfile>();

    [JsonIgnore]
    public ICollection<TeacherProfile> TeacherProfiles { get; set; } = new List<TeacherProfile>();

    [NotMapped]
    [JsonIgnore]
    public ICollection<Course> Courses { get; set; } = new List<Course>();

    [NotMapped]
    [JsonIgnore]
    public ICollection<Lecture> Lectures { get; set; } = new List<Lecture>();

    [NotMapped]
    [JsonIgnore]
    public ICollection<Quiz> Quizzes { get; set; } = new List<Quiz>();

    [NotMapped]
    [JsonIgnore]
    public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
}
