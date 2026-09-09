using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace CodeClassPlatform.BackEnd.Entities;

public class TeacherProfile
{
    [Key]
    public int Id { get; set; }

    public int AccountId { get; set; }

    [MaxLength(200)]
    public string? FullNameEn { get; set; }

    [MaxLength(200)]
    public string? FullNameAr { get; set; }

    [MaxLength(1000)]
    public string? BioEn { get; set; }

    [MaxLength(1000)]
    public string? BioAr { get; set; }

    [MaxLength(50)]
    public string? PhoneNumber { get; set; }

    public int? ProfileImageAssetId { get; set; }

    [MaxLength(200)]
    public string? SpecializationEn { get; set; }

    [MaxLength(200)]
    public string? SpecializationAr { get; set; }

    public int? YearsOfExperience { get; set; }

    [MaxLength(300)]
    public string? QualificationEn { get; set; }

    [MaxLength(300)]
    public string? QualificationAr { get; set; }

    [ForeignKey(nameof(AccountId))]
    [JsonIgnore]
    public Account? Account { get; set; }

    [ForeignKey(nameof(ProfileImageAssetId))]
    [JsonIgnore]
    public ExternalResource? ProfileImageAsset { get; set; }
}
