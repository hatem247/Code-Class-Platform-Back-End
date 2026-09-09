using CodeClassPlatform.BackEnd.Data;
using CodeClassPlatform.BackEnd.Entities;
using CodeClassPlatform.BackEnd.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CodeClassPlatform.BackEnd.Controllers;

[ApiController]
[Route("api/[controller]")]
public class QuizzesController : ControllerBase
{
    private readonly AppDbContext _context;

    public QuizzesController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<PagedResult<object>>> ListQuizzes([FromQuery] int page = 1, [FromQuery] int pageSize = 10, CancellationToken cancellationToken = default)
    {
        var query = _context.Quizzes.AsNoTracking().AsQueryable();
        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query.OrderByDescending(q => q.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(q => new
            {
                q.Id,
                q.TitleEn,
                q.TitleAr,
                q.DescriptionEn,
                q.DescriptionAr,
                q.IsPublished,
                q.PassingScore,
                q.MaxAttempts,
                q.TimeLimitMinutes
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

    [HttpGet("{id:int}")]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<object>>> GetQuiz(int id, CancellationToken cancellationToken)
    {
        var quiz = await _context.Quizzes
            .Include(q => q.Questions)
            .ThenInclude(q => q.Options)
            .AsNoTracking()
            .FirstOrDefaultAsync(q => q.Id == id, cancellationToken);

        if (quiz == null)
        {
            return NotFound(new ApiResponse<object> { Success = false, StatusCode = 404, Message = "Quiz not found.", Errors = new List<string> { "Quiz not found." } });
        }

        return Ok(new ApiResponse<object> { Success = true, StatusCode = 200, Message = "Quiz loaded.", Data = quiz });
    }

    [HttpPost]
    [Authorize(Roles = "TEACHER,ADMIN")]
    public async Task<ActionResult<ApiResponse<object>>> CreateQuiz([FromBody] CreateQuizRequest request, CancellationToken cancellationToken)
    {
        var quiz = new Quiz
        {
            TitleEn = request.TitleEn,
            TitleAr = request.TitleAr,
            DescriptionEn = request.DescriptionEn,
            DescriptionAr = request.DescriptionAr,
            CourseId = request.CourseId,
            LectureId = request.LectureId,
            TeacherId = request.TeacherId,
            TimeLimitMinutes = request.TimeLimitMinutes,
            PassingScore = request.PassingScore,
            MaxAttempts = request.MaxAttempts,
            IsPublished = request.IsPublished,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Quizzes.Add(quiz);
        await _context.SaveChangesAsync(cancellationToken);
        return Ok(new ApiResponse<object> { Success = true, StatusCode = 200, Message = "Quiz created successfully.", Data = quiz });
    }

    [HttpPost("{id:int}/questions")]
    [Authorize(Roles = "TEACHER,ADMIN")]
    public async Task<ActionResult<ApiResponse<object>>> AddQuestion(int id, [FromBody] CreateQuestionRequest request, CancellationToken cancellationToken)
    {
        var quiz = await _context.Quizzes.FirstOrDefaultAsync(q => q.Id == id, cancellationToken);
        if (quiz == null)
        {
            return NotFound(new ApiResponse<object> { Success = false, StatusCode = 404, Message = "Quiz not found.", Errors = new List<string> { "Quiz not found." } });
        }

        var question = new QuizQuestion
        {
            QuizId = quiz.Id,
            QuestionTextEn = request.QuestionTextEn,
            QuestionTextAr = request.QuestionTextAr,
            ExplanationEn = request.ExplanationEn,
            ExplanationAr = request.ExplanationAr,
            QuestionType = request.QuestionType,
            Points = request.Points,
            Order = request.Order
        };

        _context.QuizQuestions.Add(question);
        await _context.SaveChangesAsync(cancellationToken);
        return Ok(new ApiResponse<object> { Success = true, StatusCode = 200, Message = "Question added.", Data = question });
    }

    [HttpPost("{id:int}/attempts/start")]
    [Authorize(Roles = "STUDENT")]
    public async Task<ActionResult<ApiResponse<object>>> StartAttempt(int id, CancellationToken cancellationToken)
    {
        var accountIdClaim = User.FindFirst("accountId");
        if (accountIdClaim == null || !int.TryParse(accountIdClaim.Value, out var accountId))
        {
            return Unauthorized(new ApiResponse<object> { Success = false, StatusCode = 401, Message = "Unauthorized.", Errors = new List<string> { "Unauthorized." } });
        }

        var quiz = await _context.Quizzes.FirstOrDefaultAsync(q => q.Id == id && q.IsPublished, cancellationToken);
        if (quiz == null)
        {
            return NotFound(new ApiResponse<object> { Success = false, StatusCode = 404, Message = "Quiz not available.", Errors = new List<string> { "Quiz not available." } });
        }

        var attemptCount = await _context.QuizAttempts.CountAsync(a => a.StudentId == accountId && a.QuizId == id, cancellationToken);
        var attempt = new QuizAttempt
        {
            StudentId = accountId,
            QuizId = id,
            StartedAt = DateTime.UtcNow,
            AttemptNumber = attemptCount + 1
        };

        _context.QuizAttempts.Add(attempt);
        await _context.SaveChangesAsync(cancellationToken);

        return Ok(new ApiResponse<object> { Success = true, StatusCode = 200, Message = "Quiz attempt started.", Data = attempt });
    }

    public class CreateQuizRequest
    {
        public string? TitleEn { get; set; }
        public string? TitleAr { get; set; }
        public string? DescriptionEn { get; set; }
        public string? DescriptionAr { get; set; }
        public int? CourseId { get; set; }
        public int? LectureId { get; set; }
        public int? TeacherId { get; set; }
        public int? TimeLimitMinutes { get; set; }
        public int? PassingScore { get; set; }
        public int? MaxAttempts { get; set; }
        public bool IsPublished { get; set; }
    }

    public class CreateQuestionRequest
    {
        public string? QuestionTextEn { get; set; }
        public string? QuestionTextAr { get; set; }
        public string? ExplanationEn { get; set; }
        public string? ExplanationAr { get; set; }
        public string QuestionType { get; set; } = "MULTIPLE_CHOICE";
        public int Points { get; set; } = 1;
        public int Order { get; set; }
    }
}
