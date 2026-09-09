using CodeClassPlatform.BackEnd.Data;
using CodeClassPlatform.BackEnd.Entities;
using CodeClassPlatform.BackEnd.Models;
using CodeClassPlatform.BackEnd.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CodeClassPlatform.BackEnd.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly AppDbContext _context;

    public AuthController(IAuthService authService, AppDbContext context)
    {
        _authService = authService;
        _context = context;
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<object>>> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        var result = await _authService.LoginAsync(request.Email, request.Password, cancellationToken);
        return StatusCode(result.StatusCode, result);
    }

    [HttpPost("logout")]
    [Authorize]
    public ActionResult<ApiResponse<object>> Logout()
    {
        return Ok(new ApiResponse<object> { Success = true, StatusCode = 200, Message = "Logout successful." });
    }

    [HttpPost("refresh")]
    [AllowAnonymous]
    public ActionResult<ApiResponse<object>> Refresh()
    {
        return StatusCode(501, new ApiResponse<object>
        {
            Success = false,
            StatusCode = 501,
            Message = "Refresh token flow is not implemented in this initial backend pass.",
            Errors = new List<string> { "Refresh token flow is not implemented in this initial backend pass." }
        });
    }

    [HttpGet("me")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<object>>> GetCurrentUser(CancellationToken cancellationToken)
    {
        var email = User.Identity?.Name ?? User.FindFirst("email")?.Value ?? User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;

        if (string.IsNullOrWhiteSpace(email))
        {
            return Unauthorized(new ApiResponse<object> { Success = false, StatusCode = 401, Message = "Unauthorized.", Errors = new List<string> { "Unauthorized." } });
        }

        var account = await _context.Accounts.FirstOrDefaultAsync(a => a.Email != null && a.Email.ToLower() == email.ToLower(), cancellationToken);
        if (account == null)
        {
            return NotFound(new ApiResponse<object> { Success = false, StatusCode = 404, Message = "Account not found.", Errors = new List<string> { "Account not found." } });
        }

        StudentProfile? studentProfile = null;
        TeacherProfile? teacherProfile = null;

        studentProfile = await _context.StudentProfiles.FirstOrDefaultAsync(s => s.AccountId == account.Id, cancellationToken);
        if (studentProfile == null)
        {
            teacherProfile = await _context.TeacherProfiles.FirstOrDefaultAsync(t => t.AccountId == account.Id, cancellationToken);
        }

        return Ok(new ApiResponse<object>
        {
            Success = true,
            StatusCode = 200,
            Message = "Current user loaded.",
            Data = new
            {
                account.Id,
                account.Email,
                account.Role,
                account.IsActive,
                account.PhoneNumber,
                account.CreatedAt,
                studentProfile,
                teacherProfile
            }
        });
    }

    [HttpPost("change-password")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<object>>> ChangePassword([FromBody] ChangePasswordRequest request, CancellationToken cancellationToken)
    {
        var userClaim = User.FindFirst("accountId");
        if (userClaim == null || !int.TryParse(userClaim.Value, out var accountId))
        {
            return Unauthorized(new ApiResponse<object> { Success = false, StatusCode = 401, Message = "Unauthorized.", Errors = new List<string> { "Unauthorized." } });
        }

        var result = await _authService.ChangePasswordAsync(accountId, request.CurrentPassword, request.NewPassword, cancellationToken);
        return StatusCode(result.StatusCode, result);
    }

    public class LoginRequest
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class ChangePasswordRequest
    {
        public string CurrentPassword { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
    }
}
