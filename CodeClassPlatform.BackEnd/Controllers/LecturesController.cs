using CodeClassPlatform.BackEnd.Data;
using CodeClassPlatform.BackEnd.Entities;
using CodeClassPlatform.BackEnd.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CodeClassPlatform.BackEnd.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LecturesController : ControllerBase
{
    private readonly AppDbContext _context;

    public LecturesController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<PagedResult<Lecture>>> ListLectures([FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] int? courseId = null, CancellationToken cancellationToken = default)
    {
        var query = _context.Lectures.AsNoTracking().AsQueryable();
        if (courseId.HasValue)
        {
            query = query.Where(l => l.CourseId == courseId.Value);
        }

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query.OrderBy(l => l.Order)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var totalPages = (int)Math.Ceiling(totalCount / (double)Math.Max(pageSize, 1));

        return Ok(new PagedResult<Lecture>
        {
            Items = items,
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount,
            TotalPages = totalPages,
            HasNext = page < totalPages,
            HasPrevious = page > 1
        });
    }

    [HttpGet("{id:int}")]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<object>>> GetLecture(int id, CancellationToken cancellationToken)
    {
        var lecture = await _context.Lectures.AsNoTracking().FirstOrDefaultAsync(l => l.Id == id, cancellationToken);
        if (lecture == null)
        {
            return NotFound(new ApiResponse<object> { Success = false, StatusCode = 404, Message = "Lecture not found.", Errors = new List<string> { "Lecture not found." } });
        }

        return Ok(new ApiResponse<object> { Success = true, StatusCode = 200, Message = "Lecture loaded.", Data = lecture });
    }

    [HttpPost]
    [Authorize(Roles = "TEACHER,ADMIN")]
    public async Task<ActionResult<ApiResponse<object>>> CreateLecture([FromBody] CreateLectureRequest request, CancellationToken cancellationToken)
    {
        var lecture = new Lecture
        {
            CourseId = request.CourseId,
            TeacherId = request.TeacherId,
            TitleEn = request.TitleEn,
            TitleAr = request.TitleAr,
            DescriptionEn = request.DescriptionEn,
            DescriptionAr = request.DescriptionAr,
            DurationSeconds = request.DurationSeconds,
            Order = request.Order,
            IsPublished = request.IsPublished,
            IsFree = request.IsFree,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Lectures.Add(lecture);
        await _context.SaveChangesAsync(cancellationToken);
        return Ok(new ApiResponse<object> { Success = true, StatusCode = 200, Message = "Lecture created successfully.", Data = lecture });
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "TEACHER,ADMIN")]
    public async Task<ActionResult<ApiResponse<object>>> UpdateLecture(int id, [FromBody] UpdateLectureRequest request, CancellationToken cancellationToken)
    {
        var lecture = await _context.Lectures.FirstOrDefaultAsync(l => l.Id == id, cancellationToken);
        if (lecture == null)
        {
            return NotFound(new ApiResponse<object> { Success = false, StatusCode = 404, Message = "Lecture not found.", Errors = new List<string> { "Lecture not found." } });
        }

        if (!string.IsNullOrWhiteSpace(request.TitleEn)) lecture.TitleEn = request.TitleEn;
        if (!string.IsNullOrWhiteSpace(request.TitleAr)) lecture.TitleAr = request.TitleAr;
        if (!string.IsNullOrWhiteSpace(request.DescriptionEn)) lecture.DescriptionEn = request.DescriptionEn;
        if (!string.IsNullOrWhiteSpace(request.DescriptionAr)) lecture.DescriptionAr = request.DescriptionAr;
        if (request.DurationSeconds.HasValue) lecture.DurationSeconds = request.DurationSeconds.Value;
        if (request.Order.HasValue) lecture.Order = request.Order.Value;
        if (request.IsPublished.HasValue) lecture.IsPublished = request.IsPublished.Value;
        if (request.IsFree.HasValue) lecture.IsFree = request.IsFree.Value;
        lecture.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);
        return Ok(new ApiResponse<object> { Success = true, StatusCode = 200, Message = "Lecture updated successfully.", Data = lecture });
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "TEACHER,ADMIN")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteLecture(int id, CancellationToken cancellationToken)
    {
        var lecture = await _context.Lectures.FirstOrDefaultAsync(l => l.Id == id, cancellationToken);
        if (lecture == null)
        {
            return NotFound(new ApiResponse<object> { Success = false, StatusCode = 404, Message = "Lecture not found.", Errors = new List<string> { "Lecture not found." } });
        }

        _context.Lectures.Remove(lecture);
        await _context.SaveChangesAsync(cancellationToken);
        return Ok(new ApiResponse<object> { Success = true, StatusCode = 200, Message = "Lecture deleted successfully." });
    }

    [HttpPost("{id:int}/publish")]
    [Authorize(Roles = "TEACHER,ADMIN")]
    public async Task<ActionResult<ApiResponse<object>>> PublishLecture(int id, CancellationToken cancellationToken)
    {
        var lecture = await _context.Lectures.FirstOrDefaultAsync(l => l.Id == id, cancellationToken);
        if (lecture == null)
        {
            return NotFound(new ApiResponse<object> { Success = false, StatusCode = 404, Message = "Lecture not found.", Errors = new List<string> { "Lecture not found." } });
        }

        lecture.IsPublished = true;
        lecture.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync(cancellationToken);
        return Ok(new ApiResponse<object> { Success = true, StatusCode = 200, Message = "Lecture published.", Data = lecture });
    }

    [HttpPost("{id:int}/unpublish")]
    [Authorize(Roles = "TEACHER,ADMIN")]
    public async Task<ActionResult<ApiResponse<object>>> UnpublishLecture(int id, CancellationToken cancellationToken)
    {
        var lecture = await _context.Lectures.FirstOrDefaultAsync(l => l.Id == id, cancellationToken);
        if (lecture == null)
        {
            return NotFound(new ApiResponse<object> { Success = false, StatusCode = 404, Message = "Lecture not found.", Errors = new List<string> { "Lecture not found." } });
        }

        lecture.IsPublished = false;
        lecture.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync(cancellationToken);
        return Ok(new ApiResponse<object> { Success = true, StatusCode = 200, Message = "Lecture unpublished.", Data = lecture });
    }

    public class CreateLectureRequest
    {
        public int? CourseId { get; set; }
        public int? TeacherId { get; set; }
        public string? TitleEn { get; set; }
        public string? TitleAr { get; set; }
        public string? DescriptionEn { get; set; }
        public string? DescriptionAr { get; set; }
        public int? DurationSeconds { get; set; }
        public int Order { get; set; }
        public bool IsPublished { get; set; }
        public bool IsFree { get; set; }
    }

    public class UpdateLectureRequest
    {
        public string? TitleEn { get; set; }
        public string? TitleAr { get; set; }
        public string? DescriptionEn { get; set; }
        public string? DescriptionAr { get; set; }
        public int? DurationSeconds { get; set; }
        public int? Order { get; set; }
        public bool? IsPublished { get; set; }
        public bool? IsFree { get; set; }
    }
}
