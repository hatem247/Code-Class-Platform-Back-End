using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace CodeClassPlatform.BackEnd.Entities;

public class StudentProfile
{
    [Key]
    public int Id { get; set; }

    public int AccountId { get; set; }

    [MaxLength(100)]
    public string? StudentNumber { get; set; }

    [MaxLength(200)]
    public string? FullNameEn { get; set; }

    [MaxLength(200)]
    public string? FullNameAr { get; set; }

    public DateTime? DateOfBirth { get; set; }

    [MaxLength(20)]
    public string? Gender { get; set; }

    [MaxLength(50)]
    public string? Phone { get; set; }

    [MaxLength(500)]
    public string? AddressEn { get; set; }

    [MaxLength(500)]
    public string? AddressAr { get; set; }

    [MaxLength(1000)]
    public string? BioEn { get; set; }

    [MaxLength(1000)]
    public string? BioAr { get; set; }

    public int? ProfileImageAssetId { get; set; }

    [MaxLength(200)]
    public string? Level { get; set; }

    [MaxLength(200)]
    public string? Grade { get; set; }

    [MaxLength(200)]
    public string? Major { get; set; }

    [MaxLength(200)]
    public string? EmergencyContactName { get; set; }

    [MaxLength(50)]
    public string? EmergencyContactPhone { get; set; }

    [ForeignKey(nameof(AccountId))]
    [JsonIgnore]
    public Account? Account { get; set; }

    [ForeignKey(nameof(ProfileImageAssetId))]
    [JsonIgnore]
    public ExternalResource? ProfileImageAsset { get; set; }
}
