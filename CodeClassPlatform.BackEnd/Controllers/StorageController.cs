using CodeClassPlatform.BackEnd.Data;
using CodeClassPlatform.BackEnd.Entities;
using CodeClassPlatform.BackEnd.Models;
using CodeClassPlatform.BackEnd.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CodeClassPlatform.BackEnd.Controllers;

[ApiController]
[Route("api/resources")]
public class ResourcesController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IGoogleDriveLinkService _googleDriveLinkService;

    public ResourcesController(AppDbContext context, IGoogleDriveLinkService googleDriveLinkService)
    {
        _context = context;
        _googleDriveLinkService = googleDriveLinkService;
    }

    [HttpGet("{id:int}")]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<object>>> GetResource(int id, CancellationToken cancellationToken)
    {
        var resource = await _context.ExternalResources.AsNoTracking().FirstOrDefaultAsync(r => r.Id == id, cancellationToken);
        if (resource == null)
        {
            return NotFound(new ApiResponse<object> { Success = false, StatusCode = 404, Message = "Resource not found.", Errors = new List<string> { "Resource not found." } });
        }

        return Ok(new ApiResponse<object> { Success = true, StatusCode = 200, Message = "Resource loaded.", Data = Map(resource) });
    }

    [HttpPost]
    [Authorize(Roles = "TEACHER,ADMIN")]
    public async Task<ActionResult<ApiResponse<object>>> CreateResource([FromBody] CreateExternalResourceRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Url))
        {
            return BadRequest(new ApiResponse<object> { Success = false, StatusCode = 400, Message = "A resource URL is required.", Errors = new List<string> { "A resource URL is required." } });
        }

        var validation = _googleDriveLinkService.Validate(request.Url);
        if (!validation.IsValid)
        {
            return BadRequest(new ApiResponse<object> { Success = false, StatusCode = 400, Message = validation.ErrorMessage ?? "Invalid resource URL.", Errors = new List<string> { validation.ErrorMessage ?? "Invalid resource URL." } });
        }

        var resource = new ExternalResource
        {
            Provider = "GoogleDrive",
            ResourceType = string.IsNullOrWhiteSpace(request.ResourceType) ? validation.ResourceType ?? "File" : request.ResourceType,
            OriginalUrl = request.Url.Trim(),
            CanonicalUrl = validation.CanonicalUrl ?? request.Url.Trim(),
            ExternalId = validation.FileId,
            FileName = request.FileName,
            ContentType = request.ContentType,
            DurationSeconds = request.DurationSeconds,
            ThumbnailUrl = request.ThumbnailUrl,
            TitleEn = request.TitleEn,
            TitleAr = request.TitleAr,
            DescriptionEn = request.DescriptionEn,
            DescriptionAr = request.DescriptionAr,
            IsActive = true,
            VerificationStatus = "Unknown",
            CreatedByAccountId = GetCurrentAccountId(),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.ExternalResources.Add(resource);
        await _context.SaveChangesAsync(cancellationToken);

        return Ok(new ApiResponse<object> { Success = true, StatusCode = 200, Message = "Resource registered successfully.", Data = Map(resource) });
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "TEACHER,ADMIN")]
    public async Task<ActionResult<ApiResponse<object>>> UpdateResource(int id, [FromBody] UpdateExternalResourceRequest request, CancellationToken cancellationToken)
    {
        var resource = await _context.ExternalResources.FirstOrDefaultAsync(r => r.Id == id, cancellationToken);
        if (resource == null)
        {
            return NotFound(new ApiResponse<object> { Success = false, StatusCode = 404, Message = "Resource not found.", Errors = new List<string> { "Resource not found." } });
        }

        if (!string.IsNullOrWhiteSpace(request.Url))
        {
            var validation = _googleDriveLinkService.Validate(request.Url);
            if (!validation.IsValid)
            {
                return BadRequest(new ApiResponse<object> { Success = false, StatusCode = 400, Message = validation.ErrorMessage ?? "Invalid resource URL.", Errors = new List<string> { validation.ErrorMessage ?? "Invalid resource URL." } });
            }

            resource.OriginalUrl = request.Url.Trim();
            resource.CanonicalUrl = validation.CanonicalUrl ?? request.Url.Trim();
            resource.ExternalId = validation.FileId;
            resource.Provider = "GoogleDrive";
        }

        if (!string.IsNullOrWhiteSpace(request.ResourceType)) resource.ResourceType = request.ResourceType;
        if (!string.IsNullOrWhiteSpace(request.TitleEn)) resource.TitleEn = request.TitleEn;
        if (!string.IsNullOrWhiteSpace(request.TitleAr)) resource.TitleAr = request.TitleAr;
        if (!string.IsNullOrWhiteSpace(request.DescriptionEn)) resource.DescriptionEn = request.DescriptionEn;
        if (!string.IsNullOrWhiteSpace(request.DescriptionAr)) resource.DescriptionAr = request.DescriptionAr;
        if (!string.IsNullOrWhiteSpace(request.FileName)) resource.FileName = request.FileName;
        if (!string.IsNullOrWhiteSpace(request.ContentType)) resource.ContentType = request.ContentType;
        if (!string.IsNullOrWhiteSpace(request.ThumbnailUrl)) resource.ThumbnailUrl = request.ThumbnailUrl;
        if (request.IsActive.HasValue) resource.IsActive = request.IsActive.Value;

        resource.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync(cancellationToken);

        return Ok(new ApiResponse<object> { Success = true, StatusCode = 200, Message = "Resource updated successfully.", Data = Map(resource) });
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "TEACHER,ADMIN")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteResource(int id, CancellationToken cancellationToken)
    {
        var resource = await _context.ExternalResources.FirstOrDefaultAsync(r => r.Id == id, cancellationToken);
        if (resource == null)
        {
            return NotFound(new ApiResponse<object> { Success = false, StatusCode = 404, Message = "Resource not found.", Errors = new List<string> { "Resource not found." } });
        }

        _context.ExternalResources.Remove(resource);
        await _context.SaveChangesAsync(cancellationToken);
        return Ok(new ApiResponse<object> { Success = true, StatusCode = 200, Message = "Resource deleted successfully." });
    }

    [HttpPost("{id:int}/verify")]
    [Authorize(Roles = "TEACHER,ADMIN")]
    public async Task<ActionResult<ApiResponse<object>>> VerifyResource(int id, CancellationToken cancellationToken)
    {
        var resource = await _context.ExternalResources.FirstOrDefaultAsync(r => r.Id == id, cancellationToken);
        if (resource == null)
        {
            return NotFound(new ApiResponse<object> { Success = false, StatusCode = 404, Message = "Resource not found.", Errors = new List<string> { "Resource not found." } });
        }

        var validation = _googleDriveLinkService.Validate(resource.OriginalUrl);
        var status = validation.IsValid ? "Verified" : "Failed";
        resource.VerificationStatus = status;
        resource.LastVerifiedAt = DateTime.UtcNow;
        resource.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        return Ok(new ApiResponse<object>
        {
            Success = true,
            StatusCode = 200,
            Message = validation.IsValid ? "Resource URL verified successfully." : "Resource URL could not be verified.",
            Data = new VerifyExternalResourceResponse
            {
                Id = resource.Id,
                VerificationStatus = status,
                LastVerifiedAt = resource.LastVerifiedAt,
                Message = validation.IsValid ? "Resource URL verified successfully." : "Resource URL could not be verified."
            }
        });
    }

    private static ExternalResourceResponse Map(ExternalResource resource)
    {
        return new ExternalResourceResponse
        {
            Id = resource.Id,
            Provider = resource.Provider,
            ResourceType = resource.ResourceType,
            OriginalUrl = resource.OriginalUrl,
            CanonicalUrl = resource.CanonicalUrl,
            ExternalId = resource.ExternalId,
            FileName = resource.FileName,
            ContentType = resource.ContentType,
            SizeBytes = resource.SizeBytes,
            DurationSeconds = resource.DurationSeconds,
            ThumbnailUrl = resource.ThumbnailUrl,
            TitleEn = resource.TitleEn,
            TitleAr = resource.TitleAr,
            DescriptionEn = resource.DescriptionEn,
            DescriptionAr = resource.DescriptionAr,
            IsActive = resource.IsActive,
            VerificationStatus = resource.VerificationStatus,
            LastVerifiedAt = resource.LastVerifiedAt,
            CreatedByAccountId = resource.CreatedByAccountId,
            CreatedAt = resource.CreatedAt
        };
    }

    private int? GetCurrentAccountId()
    {
        var accountIdClaim = User.FindFirst("accountId")?.Value ?? User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (int.TryParse(accountIdClaim, out var accountId))
        {
            return accountId;
        }

        return null;
    }
}
