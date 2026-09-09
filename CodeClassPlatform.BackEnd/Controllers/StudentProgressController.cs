using CodeClassPlatform.BackEnd.Data;
using CodeClassPlatform.BackEnd.Entities;
using CodeClassPlatform.BackEnd.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CodeClassPlatform.BackEnd.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StudentProgressController : ControllerBase
{
    private readonly AppDbContext _context;

    public StudentProgressController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet("lecture-progress")]
    [Authorize(Roles = "STUDENT")]
    public async Task<ActionResult<ApiResponse<object>>> GetLectureProgress(CancellationToken cancellationToken)
    {
        var accountIdClaim = User.FindFirst("accountId");
        if (accountIdClaim == null || !int.TryParse(accountIdClaim.Value, out var accountId))
        {
            return Unauthorized(new ApiResponse<object> { Success = false, StatusCode = 401, Message = "Unauthorized.", Errors = new List<string> { "Unauthorized." } });
        }

        var progress = await _context.StudentLectureProgress
            .Where(p => p.StudentId == accountId)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return Ok(new ApiResponse<object> { Success = true, StatusCode = 200, Message = "Progress loaded.", Data = progress });
    }

    [HttpPost("lecture-progress")]
    [Authorize(Roles = "STUDENT")]
    public async Task<ActionResult<ApiResponse<object>>> UpsertLectureProgress([FromBody] UpdateProgressRequest request, CancellationToken cancellationToken)
    {
        var accountIdClaim = User.FindFirst("accountId");
        if (accountIdClaim == null || !int.TryParse(accountIdClaim.Value, out var accountId))
        {
            return Unauthorized(new ApiResponse<object> { Success = false, StatusCode = 401, Message = "Unauthorized.", Errors = new List<string> { "Unauthorized." } });
        }

        var progress = await _context.StudentLectureProgress
            .FirstOrDefaultAsync(p => p.StudentId == accountId && p.LectureId == request.LectureId, cancellationToken);

        if (progress == null)
        {
            progress = new StudentLectureProgress
            {
                StudentId = accountId,
                LectureId = request.LectureId,
                FirstOpenedAt = DateTime.UtcNow,
                LastWatchedAt = DateTime.UtcNow
            };
            _context.StudentLectureProgress.Add(progress);
        }

        progress.WatchedSeconds = request.WatchedSeconds;
        progress.LastPositionSeconds = request.LastPositionSeconds;
        progress.CompletionPercentage = request.CompletionPercentage;
        progress.IsCompleted = request.IsCompleted;
        progress.LastWatchedAt = DateTime.UtcNow;

        if (request.IsCompleted)
        {
            progress.IsCompleted = true;
            progress.CompletionPercentage = 100;
        }

        await _context.SaveChangesAsync(cancellationToken);

        return Ok(new ApiResponse<object> { Success = true, StatusCode = 200, Message = "Progress updated.", Data = progress });
    }

    public class UpdateProgressRequest
    {
        public int LectureId { get; set; }
        public int WatchedSeconds { get; set; }
        public int CompletionPercentage { get; set; }
        public bool IsCompleted { get; set; }
        public int LastPositionSeconds { get; set; }
    }
}
