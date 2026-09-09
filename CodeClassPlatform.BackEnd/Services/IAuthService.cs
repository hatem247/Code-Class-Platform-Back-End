using CodeClassPlatform.BackEnd.Entities;
using CodeClassPlatform.BackEnd.Models;

namespace CodeClassPlatform.BackEnd.Services;

public interface IAuthService
{
    Task<ApiResponse<object>> LoginAsync(string email, string password, CancellationToken cancellationToken = default);
    Task<ApiResponse<object>> RefreshAsync(string refreshToken, CancellationToken cancellationToken = default);
    Task<ApiResponse<object>> ChangePasswordAsync(int accountId, string currentPassword, string newPassword, CancellationToken cancellationToken = default);
    Task<Account?> GetCurrentAccountAsync(string email, CancellationToken cancellationToken = default);
    string GenerateJwtToken(Account account);
}
