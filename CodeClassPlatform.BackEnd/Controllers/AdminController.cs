using CodeClassPlatform.BackEnd.Data;
using CodeClassPlatform.BackEnd.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CodeClassPlatform.BackEnd.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AdminController : ControllerBase
{
    private readonly AppDbContext _context;

    public AdminController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet("dashboard")]
    [Authorize(Roles = "ADMIN")]
    public async Task<ActionResult<ApiResponse<object>>> Dashboard(CancellationToken cancellationToken)
    {
        var totalStudents = await _context.Accounts.CountAsync(a => a.Role == "STUDENT", cancellationToken);
        var totalTeachers = await _context.Accounts.CountAsync(a => a.Role == "TEACHER", cancellationToken);
        var activeUsers = await _context.Accounts.CountAsync(a => a.IsActive, cancellationToken);
        var inactiveUsers = await _context.Accounts.CountAsync(a => !a.IsActive, cancellationToken);
        var totalCourses = await _context.Courses.CountAsync(cancellationToken);
        var totalLectures = await _context.Lectures.CountAsync(cancellationToken);
        var totalQuizzes = await _context.Quizzes.CountAsync(cancellationToken);

        var recentAccounts = await _context.Accounts
            .AsNoTracking()
            .OrderByDescending(a => a.CreatedAt)
            .Take(5)
            .Select(a => new { a.Id, a.Email, a.Role, a.IsActive, a.CreatedAt })
            .ToListAsync(cancellationToken);

        return Ok(new ApiResponse<object>
        {
            Success = true,
            StatusCode = 200,
            Message = "Dashboard metrics loaded.",
            Data = new
            {
                totalStudents,
                totalTeachers,
                activeUsers,
                inactiveUsers,
                totalCourses,
                totalLectures,
                totalQuizzes,
                recentAccounts
            }
        });
    }
}
