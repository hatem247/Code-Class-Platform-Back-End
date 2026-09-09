using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CodeClassPlatform.BackEnd.Data;
using CodeClassPlatform.BackEnd.Entities;
using CodeClassPlatform.BackEnd.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace CodeClassPlatform.BackEnd.Services;

public class AuthService : IAuthService
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _configuration;

    public AuthService(AppDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    public async Task<ApiResponse<object>> LoginAsync(string email, string password, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            return new ApiResponse<object> { Success = false, StatusCode = 400, Message = "Email and password are required.", Errors = new List<string> { "Email and password are required." } };
        }

        var normalizedEmail = email.Trim();
        var account = await _context.Accounts.FirstOrDefaultAsync(a => a.Email != null && a.Email.ToLower() == normalizedEmail.ToLower(), cancellationToken);

        if (account == null || !BCrypt.Net.BCrypt.Verify(password, account.PasswordHash ?? string.Empty))
        {
            return new ApiResponse<object> { Success = false, StatusCode = 401, Message = "Invalid credentials.", Errors = new List<string> { "Invalid credentials." } };
        }

        if (!account.IsActive)
        {
            return new ApiResponse<object> { Success = false, StatusCode = 401, Message = "Account is inactive.", Errors = new List<string> { "Account is inactive." } };
        }

        account.LastLoginAt = DateTime.UtcNow;
        await _context.SaveChangesAsync(cancellationToken);

        var token = GenerateJwtToken(account);
        var payload = new
        {
            token,
            email = account.Email,
            accountId = account.Id,
            role = account.Role
        };

        return new ApiResponse<object> { Success = true, StatusCode = 200, Message = "Login successful.", Data = payload };
    }

    public Task<ApiResponse<object>> RefreshAsync(string refreshToken, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(new ApiResponse<object>
        {
            Success = false,
            StatusCode = 501,
            Message = "Refresh token flow is not implemented in this initial backend pass.",
            Errors = new List<string> { "Refresh token flow is not implemented in this initial backend pass." }
        });
    }

    public async Task<ApiResponse<object>> ChangePasswordAsync(int accountId, string currentPassword, string newPassword, CancellationToken cancellationToken = default)
    {
        var account = await _context.Accounts.FirstOrDefaultAsync(a => a.Id == accountId, cancellationToken);
        if (account == null)
        {
            return new ApiResponse<object> { Success = false, StatusCode = 404, Message = "Account not found.", Errors = new List<string> { "Account not found." } };
        }

        if (string.IsNullOrWhiteSpace(currentPassword) || string.IsNullOrWhiteSpace(newPassword))
        {
            return new ApiResponse<object> { Success = false, StatusCode = 400, Message = "Current and new passwords are required.", Errors = new List<string> { "Current and new passwords are required." } };
        }

        if (!BCrypt.Net.BCrypt.Verify(currentPassword, account.PasswordHash ?? string.Empty))
        {
            return new ApiResponse<object> { Success = false, StatusCode = 401, Message = "Current password is incorrect.", Errors = new List<string> { "Current password is incorrect." } };
        }

        if (!IsValidPassword(newPassword))
        {
            return new ApiResponse<object>
            {
                Success = false,
                StatusCode = 400,
                Message = "New password must be at least 8 characters and include uppercase, lowercase, a number, and a special character.",
                Errors = new List<string> { "New password must be at least 8 characters and include uppercase, lowercase, a number, and a special character." }
            };
        }

        account.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
        account.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync(cancellationToken);

        return new ApiResponse<object> { Success = true, StatusCode = 200, Message = "Password changed successfully.", Data = new { accountId = account.Id } };
    }

    public async Task<Account?> GetCurrentAccountAsync(string email, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            return null;
        }

        return await _context.Accounts
            .FirstOrDefaultAsync(a => a.Email != null && a.Email.ToLower() == email.Trim().ToLower(), cancellationToken);
    }

    public string GenerateJwtToken(Account account)
    {
        var jwtKey = _configuration["JWT:Key"];
        if (string.IsNullOrWhiteSpace(jwtKey))
        {
            throw new InvalidOperationException("JWT:Key configuration is required.");
        }

        var issuer = _configuration["JWT:Issuer"];
        if (string.IsNullOrWhiteSpace(issuer))
        {
            throw new InvalidOperationException("JWT:Issuer configuration is required.");
        }

        var audience = _configuration["JWT:Audience"];
        if (string.IsNullOrWhiteSpace(audience))
        {
            throw new InvalidOperationException("JWT:Audience configuration is required.");
        }

        var key = Encoding.UTF8.GetBytes(jwtKey);
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, account.Email ?? string.Empty),
            new Claim("accountId", account.Id.ToString()),
            new Claim("role", account.Role),
            new Claim(ClaimTypes.Email, account.Email ?? string.Empty),
            new Claim(JwtRegisteredClaimNames.Email, account.Email ?? string.Empty)
        };

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(8),
            signingCredentials: new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256));

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private static bool IsValidPassword(string password)
    {
        if (string.IsNullOrWhiteSpace(password) || password.Length < 8)
        {
            return false;
        }

        var hasUpper = password.Any(char.IsUpper);
        var hasLower = password.Any(char.IsLower);
        var hasDigit = password.Any(char.IsDigit);
        var hasSymbol = password.Any(ch => !char.IsLetterOrDigit(ch));

        return hasUpper && hasLower && hasDigit && hasSymbol;
    }
}
