using CodeClassPlatform.BackEnd.Data;
using CodeClassPlatform.BackEnd.Entities;
using CodeClassPlatform.BackEnd.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CodeClassPlatform.BackEnd.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CoursesController : ControllerBase
{
    private readonly AppDbContext _context;

    public CoursesController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<PagedResult<CourseSummaryDto>>> ListCourses([FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] string? search = null, [FromQuery] bool? published = null, CancellationToken cancellationToken = default)
    {
        var query = _context.Courses.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();
            query = query.Where(c =>
                (c.NameEn != null && c.NameEn.Contains(term)) ||
                (c.NameAr != null && c.NameAr.Contains(term)) ||
                (c.DescriptionEn != null && c.DescriptionEn.Contains(term)) ||
                (c.DescriptionAr != null && c.DescriptionAr.Contains(term)));
        }

        if (published.HasValue)
        {
            query = query.Where(c => c.IsPublished == published.Value);
        }

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query.OrderBy(c => c.Ordering)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(c => new CourseSummaryDto
            {
                Id = c.Id,
                NameEn = c.NameEn,
                NameAr = c.NameAr,
                DescriptionEn = c.DescriptionEn,
                DescriptionAr = c.DescriptionAr,
                IsPublished = c.IsPublished,
                IsActive = c.IsActive,
                Ordering = c.Ordering,
                CreatedAt = c.CreatedAt
            })
            .ToListAsync(cancellationToken);

        var totalPages = (int)Math.Ceiling(totalCount / (double)Math.Max(pageSize, 1));

        return Ok(new PagedResult<CourseSummaryDto>
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
    public async Task<ActionResult<ApiResponse<object>>> GetCourse(int id, CancellationToken cancellationToken)
    {
        var course = await _context.Courses.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
        if (course == null)
        {
            return NotFound(new ApiResponse<object> { Success = false, StatusCode = 404, Message = "Course not found.", Errors = new List<string> { "Course not found." } });
        }

        return Ok(new ApiResponse<object>
        {
            Success = true,
            StatusCode = 200,
            Message = "Course loaded.",
            Data = new
            {
                course.Id,
                course.NameEn,
                course.NameAr,
                course.DescriptionEn,
                course.DescriptionAr,
                course.IsPublished,
                course.IsActive,
                course.Ordering,
                course.CreatedAt,
                course.UpdatedAt
            }
        });
    }

    [HttpPost]
    [Authorize(Roles = "ADMIN,TEACHER")]
    public async Task<ActionResult<ApiResponse<object>>> CreateCourse([FromBody] CreateCourseRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.NameEn) && string.IsNullOrWhiteSpace(request.NameAr))
        {
            return BadRequest(new ApiResponse<object> { Success = false, StatusCode = 400, Message = "Course name is required.", Errors = new List<string> { "Course name is required." } });
        }

        var course = new Course
        {
            NameEn = request.NameEn,
            NameAr = request.NameAr,
            DescriptionEn = request.DescriptionEn,
            DescriptionAr = request.DescriptionAr,
            IsPublished = request.IsPublished,
            Ordering = request.Ordering,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            IsActive = true
        };

        _context.Courses.Add(course);
        await _context.SaveChangesAsync(cancellationToken);

        return Ok(new ApiResponse<object>
        {
            Success = true,
            StatusCode = 200,
            Message = "Course created successfully.",
            Data = course
        });
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "ADMIN,TEACHER")]
    public async Task<ActionResult<ApiResponse<object>>> UpdateCourse(int id, [FromBody] UpdateCourseRequest request, CancellationToken cancellationToken)
    {
        var course = await _context.Courses.FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
        if (course == null)
        {
            return NotFound(new ApiResponse<object> { Success = false, StatusCode = 404, Message = "Course not found.", Errors = new List<string> { "Course not found." } });
        }

        if (!string.IsNullOrWhiteSpace(request.NameEn)) course.NameEn = request.NameEn;
        if (!string.IsNullOrWhiteSpace(request.NameAr)) course.NameAr = request.NameAr;
        if (!string.IsNullOrWhiteSpace(request.DescriptionEn)) course.DescriptionEn = request.DescriptionEn;
        if (!string.IsNullOrWhiteSpace(request.DescriptionAr)) course.DescriptionAr = request.DescriptionAr;
        if (request.IsPublished.HasValue) course.IsPublished = request.IsPublished.Value;
        if (request.Ordering.HasValue) course.Ordering = request.Ordering.Value;
        course.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);
        return Ok(new ApiResponse<object> { Success = true, StatusCode = 200, Message = "Course updated successfully.", Data = course });
    }

    [HttpPost("{id:int}/publish")]
    [Authorize(Roles = "ADMIN,TEACHER")]
    public async Task<ActionResult<ApiResponse<object>>> PublishCourse(int id, CancellationToken cancellationToken)
    {
        var course = await _context.Courses.FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
        if (course == null)
        {
            return NotFound(new ApiResponse<object> { Success = false, StatusCode = 404, Message = "Course not found.", Errors = new List<string> { "Course not found." } });
        }

        course.IsPublished = true;
        course.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync(cancellationToken);

        return Ok(new ApiResponse<object> { Success = true, StatusCode = 200, Message = "Course published.", Data = course });
    }

    [HttpPost("{id:int}/unpublish")]
    [Authorize(Roles = "ADMIN,TEACHER")]
    public async Task<ActionResult<ApiResponse<object>>> UnpublishCourse(int id, CancellationToken cancellationToken)
    {
        var course = await _context.Courses.FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
        if (course == null)
        {
            return NotFound(new ApiResponse<object> { Success = false, StatusCode = 404, Message = "Course not found.", Errors = new List<string> { "Course not found." } });
        }

        course.IsPublished = false;
        course.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync(cancellationToken);

        return Ok(new ApiResponse<object> { Success = true, StatusCode = 200, Message = "Course unpublished.", Data = course });
    }
}
