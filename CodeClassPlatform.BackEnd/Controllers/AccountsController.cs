using CodeClassPlatform.BackEnd.Data;
using CodeClassPlatform.BackEnd.Entities;
using CodeClassPlatform.BackEnd.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CodeClassPlatform.BackEnd.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AccountsController : ControllerBase
{
    private readonly AppDbContext _context;

    public AccountsController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    [Authorize(Roles = "ADMIN")]
    public async Task<ActionResult<PagedResult<object>>> GetAccounts([FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] string? search = null, [FromQuery] string? role = null, [FromQuery] bool? isActive = null, CancellationToken cancellationToken = default)
    {
        var query = _context.Accounts.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();
            query = query.Where(a => a.Email != null && a.Email.Contains(term));
        }

        if (!string.IsNullOrWhiteSpace(role))
        {
            query = query.Where(a => a.Role == role);
        }

        if (isActive.HasValue)
        {
            query = query.Where(a => a.IsActive == isActive.Value);
        }

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query.OrderByDescending(a => a.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(a => new
            {
                a.Id,
                a.Email,
                a.Role,
                a.IsActive,
                a.PhoneNumber,
                a.CreatedAt,
                a.LastLoginAt
            })
            .ToListAsync(cancellationToken);

        var totalPages = (int)Math.Ceiling(totalCount / (double)Math.Max(pageSize, 1));

        return Ok(new PagedResult<object>
        {
            Items = items.Cast<object>().ToList(),
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount,
            TotalPages = totalPages,
            HasNext = page < totalPages,
            HasPrevious = page > 1
        });
    }

    [HttpPost("students")]
    [Authorize(Roles = "ADMIN")]
    public async Task<ActionResult<ApiResponse<object>>> CreateStudent([FromBody] CreateStudentRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
        {
            return BadRequest(new ApiResponse<object> { Success = false, StatusCode = 400, Message = "Email and password are required.", Errors = new List<string> { "Email and password are required." } });
        }

        var exists = await _context.Accounts.AnyAsync(a => a.Email != null && a.Email.ToLower() == request.Email.Trim().ToLower(), cancellationToken);
        if (exists)
        {
            return Conflict(new ApiResponse<object> { Success = false, StatusCode = 409, Message = "Account already exists.", Errors = new List<string> { "Account already exists." } });
        }

        var account = new Account
        {
            Email = request.Email.Trim(),
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            Role = "STUDENT",
            IsActive = true,
            PhoneNumber = request.PhoneNumber,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Accounts.Add(account);
        await _context.SaveChangesAsync(cancellationToken);

        if (!string.IsNullOrWhiteSpace(request.FullNameEn) || !string.IsNullOrWhiteSpace(request.FullNameAr))
        {
            _context.StudentProfiles.Add(new StudentProfile
            {
                AccountId = account.Id,
                FullNameEn = request.FullNameEn,
                FullNameAr = request.FullNameAr,
                StudentNumber = request.StudentNumber,
                Phone = request.PhoneNumber,
                Level = request.Level,
                Grade = request.Grade,
                Major = request.Major
            });
            await _context.SaveChangesAsync(cancellationToken);
        }

        return Ok(new ApiResponse<object> { Success = true, StatusCode = 200, Message = "Student created successfully.", Data = new { account.Id, account.Email, account.Role } });
    }

    [HttpPost("teachers")]
    [Authorize(Roles = "ADMIN")]
    public async Task<ActionResult<ApiResponse<object>>> CreateTeacher([FromBody] CreateTeacherRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
        {
            return BadRequest(new ApiResponse<object> { Success = false, StatusCode = 400, Message = "Email and password are required.", Errors = new List<string> { "Email and password are required." } });
        }

        var exists = await _context.Accounts.AnyAsync(a => a.Email != null && a.Email.ToLower() == request.Email.Trim().ToLower(), cancellationToken);
        if (exists)
        {
            return Conflict(new ApiResponse<object> { Success = false, StatusCode = 409, Message = "Account already exists.", Errors = new List<string> { "Account already exists." } });
        }

        var account = new Account
        {
            Email = request.Email.Trim(),
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            Role = "TEACHER",
            IsActive = true,
            PhoneNumber = request.PhoneNumber,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Accounts.Add(account);
        await _context.SaveChangesAsync(cancellationToken);

        _context.TeacherProfiles.Add(new TeacherProfile
        {
            AccountId = account.Id,
            FullNameEn = request.FullNameEn,
            FullNameAr = request.FullNameAr,
            PhoneNumber = request.PhoneNumber,
            SpecializationEn = request.SpecializationEn,
            SpecializationAr = request.SpecializationAr,
            QualificationEn = request.QualificationEn,
            QualificationAr = request.QualificationAr,
            YearsOfExperience = request.YearsOfExperience
        });
        await _context.SaveChangesAsync(cancellationToken);

        return Ok(new ApiResponse<object> { Success = true, StatusCode = 200, Message = "Teacher created successfully.", Data = new { account.Id, account.Email, account.Role } });
    }

    [HttpGet("{id:int}")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<object>>> GetAccount(int id, CancellationToken cancellationToken)
    {
        var account = await _context.Accounts.AsNoTracking().FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
        if (account == null)
        {
            return NotFound(new ApiResponse<object> { Success = false, StatusCode = 404, Message = "Account not found.", Errors = new List<string> { "Account not found." } });
        }

        return Ok(new ApiResponse<object>
        {
            Success = true,
            StatusCode = 200,
            Message = "Account loaded.",
            Data = new
            {
                account.Id,
                account.Email,
                account.Role,
                account.IsActive,
                account.PhoneNumber,
                account.CreatedAt,
                account.LastLoginAt
            }
        });
    }

    public class CreateStudentRequest
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public string? FullNameEn { get; set; }
        public string? FullNameAr { get; set; }
        public string? StudentNumber { get; set; }
        public string? Level { get; set; }
        public string? Grade { get; set; }
        public string? Major { get; set; }
    }

    public class CreateTeacherRequest
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public string? FullNameEn { get; set; }
        public string? FullNameAr { get; set; }
        public string? SpecializationEn { get; set; }
        public string? SpecializationAr { get; set; }
        public string? QualificationEn { get; set; }
        public string? QualificationAr { get; set; }
        public int? YearsOfExperience { get; set; }
    }
}
