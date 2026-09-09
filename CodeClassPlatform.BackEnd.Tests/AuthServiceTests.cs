using CodeClassPlatform.BackEnd.Data;
using CodeClassPlatform.BackEnd.Entities;
using CodeClassPlatform.BackEnd.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace CodeClassPlatform.BackEnd.Tests;

public class AuthServiceTests
{
    private static AppDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }

    [Fact]
    public async Task LoginAsync_ValidCredentials_ReturnsSuccess()
    {
        using var context = CreateContext();
        var config = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["JWT:Key"] = "this-is-a-very-long-secret-key-1234567890",
            ["JWT:Issuer"] = "CodeClassPlatform",
            ["JWT:Audience"] = "CodeClassPlatformClients"
        }).Build();

        var service = new AuthService(context, config);
        context.Accounts.Add(new Account
        {
            Email = "student@example.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Password123!"),
            Role = "STUDENT",
            IsActive = true
        });
        await context.SaveChangesAsync();

        var result = await service.LoginAsync("student@example.com", "Password123!");

        Assert.True(result.Success);
        Assert.Equal(200, result.StatusCode);
    }

    [Fact]
    public async Task LoginAsync_InactiveAccount_ReturnsUnauthorized()
    {
        using var context = CreateContext();
        var config = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["JWT:Key"] = "this-is-a-very-long-secret-key-1234567890",
            ["JWT:Issuer"] = "CodeClassPlatform",
            ["JWT:Audience"] = "CodeClassPlatformClients"
        }).Build();

        var service = new AuthService(context, config);
        context.Accounts.Add(new Account
        {
            Email = "inactive@example.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Password123!"),
            Role = "STUDENT",
            IsActive = false
        });
        await context.SaveChangesAsync();

        var result = await service.LoginAsync("inactive@example.com", "Password123!");

        Assert.False(result.Success);
        Assert.Equal(401, result.StatusCode);
    }

    [Fact]
    public async Task ChangePasswordAsync_ValidCurrentPassword_UpdatesHash()
    {
        using var context = CreateContext();
        var config = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["JWT:Key"] = "this-is-a-very-long-secret-key-1234567890",
            ["JWT:Issuer"] = "CodeClassPlatform",
            ["JWT:Audience"] = "CodeClassPlatformClients"
        }).Build();

        var service = new AuthService(context, config);
        var account = new Account
        {
            Email = "update@example.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("OldPassword123!"),
            Role = "STUDENT",
            IsActive = true
        };
        context.Accounts.Add(account);
        await context.SaveChangesAsync();

        var result = await service.ChangePasswordAsync(account.Id, "OldPassword123!", "NewPassword123!");

        Assert.True(result.Success);
        Assert.Equal(200, result.StatusCode);
    }

    [Fact]
    public async Task ChangePasswordAsync_RejectsWeakPassword()
    {
        using var context = CreateContext();
        var config = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["JWT:Key"] = "this-is-a-very-long-secret-key-1234567890",
            ["JWT:Issuer"] = "CodeClassPlatform",
            ["JWT:Audience"] = "CodeClassPlatformClients"
        }).Build();

        var service = new AuthService(context, config);
        var account = new Account
        {
            Email = "weakpass@example.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("OldPassword123!"),
            Role = "STUDENT",
            IsActive = true
        };
        context.Accounts.Add(account);
        await context.SaveChangesAsync();

        var result = await service.ChangePasswordAsync(account.Id, "OldPassword123!", "weak");

        Assert.False(result.Success);
        Assert.Equal(400, result.StatusCode);
    }

    [Fact]
    public void GenerateJwtToken_RequiresConfiguredJwtKey()
    {
        using var context = CreateContext();
        var config = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["JWT:Issuer"] = "CodeClassPlatform",
            ["JWT:Audience"] = "CodeClassPlatformClients"
        }).Build();

        var service = new AuthService(context, config);
        var account = new Account
        {
            Email = "token@example.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Password123!"),
            Role = "STUDENT",
            IsActive = true
        };

        var exception = Assert.Throws<InvalidOperationException>(() => service.GenerateJwtToken(account));
        Assert.Contains("JWT:Key", exception.Message);
    }
}
