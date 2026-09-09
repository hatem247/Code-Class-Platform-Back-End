namespace CodeClassPlatform.BackEnd.Models;

public class CreateCourseRequest
{
    public string? NameEn { get; set; }
    public string? NameAr { get; set; }
    public string? DescriptionEn { get; set; }
    public string? DescriptionAr { get; set; }
    public bool IsPublished { get; set; }
    public int Ordering { get; set; }
}

public class UpdateCourseRequest
{
    public string? NameEn { get; set; }
    public string? NameAr { get; set; }
    public string? DescriptionEn { get; set; }
    public string? DescriptionAr { get; set; }
    public bool? IsPublished { get; set; }
    public int? Ordering { get; set; }
}

public class CourseSummaryDto
{
    public int Id { get; set; }
    public string? NameEn { get; set; }
    public string? NameAr { get; set; }
    public string? DescriptionEn { get; set; }
    public string? DescriptionAr { get; set; }
    public bool IsPublished { get; set; }
    public bool IsActive { get; set; }
    public int Ordering { get; set; }
    public DateTime CreatedAt { get; set; }
}
