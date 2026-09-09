using CodeClassPlatform.BackEnd.Entities;
using Microsoft.EntityFrameworkCore;

namespace CodeClassPlatform.BackEnd.Data;

public static class SeedData
{
    public static async Task SeedAsync(AppDbContext context)
    {
        if (await context.Accounts.AnyAsync())
        {
            return;
        }

        var admin = new Account
        {
            Email = "admin@codeclass.local",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123!"),
            Role = "ADMIN",
            IsActive = true,
            PhoneNumber = "0000000000",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var teacher = new Account
        {
            Email = "teacher@codeclass.local",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Teacher123!"),
            Role = "TEACHER",
            IsActive = true,
            PhoneNumber = "1111111111",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var student = new Account
        {
            Email = "student@codeclass.local",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Student123!"),
            Role = "STUDENT",
            IsActive = true,
            PhoneNumber = "2222222222",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        context.Accounts.AddRange(admin, teacher, student);
        await context.SaveChangesAsync();

        context.TeacherProfiles.Add(new TeacherProfile
        {
            AccountId = teacher.Id,
            FullNameEn = "Default Teacher",
            FullNameAr = "معلم افتراضي",
            BioEn = "System teacher.",
            BioAr = "معلم النظام.",
            SpecializationEn = "Education",
            SpecializationAr = "تعليم"
        });

        context.StudentProfiles.Add(new StudentProfile
        {
            AccountId = student.Id,
            FullNameEn = "Default Student",
            FullNameAr = "طالب افتراضي",
            BioEn = "System student.",
            BioAr = "طالب النظام.",
            Major = "Computer Science"
        });

        await context.SaveChangesAsync();
    }
}
