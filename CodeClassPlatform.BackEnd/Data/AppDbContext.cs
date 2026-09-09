using CodeClassPlatform.BackEnd.Entities;
using Microsoft.EntityFrameworkCore;

namespace CodeClassPlatform.BackEnd.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Account> Accounts { get; set; }
    public DbSet<StudentProfile> StudentProfiles { get; set; }
    public DbSet<TeacherProfile> TeacherProfiles { get; set; }
    public DbSet<ExternalResource> ExternalResources { get; set; }
    public DbSet<Course> Courses { get; set; }
    public DbSet<Lecture> Lectures { get; set; }
    public DbSet<Quiz> Quizzes { get; set; }
    public DbSet<QuizQuestion> QuizQuestions { get; set; }
    public DbSet<QuizOption> QuizOptions { get; set; }
    public DbSet<QuizAttempt> QuizAttempts { get; set; }
    public DbSet<QuizAnswer> QuizAnswers { get; set; }
    public DbSet<StudentLectureProgress> StudentLectureProgress { get; set; }
    public DbSet<Notification> Notifications { get; set; }
    public DbSet<AuditLog> AuditLogs { get; set; }
    public DbSet<Enrollment> Enrollments { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Account>().HasIndex(a => a.Email).IsUnique();
        modelBuilder.Entity<StudentProfile>().HasIndex(s => s.AccountId).IsUnique();
        modelBuilder.Entity<TeacherProfile>().HasIndex(t => t.AccountId).IsUnique();
        modelBuilder.Entity<Enrollment>().HasIndex(e => new { e.StudentId, e.CourseId }).IsUnique();
        modelBuilder.Entity<StudentLectureProgress>().HasIndex(p => new { p.StudentId, p.LectureId }).IsUnique();
        modelBuilder.Entity<Notification>().HasIndex(n => new { n.AccountId, n.IsRead, n.CreatedAt });

        modelBuilder.Entity<Account>()
            .Property(a => a.Role)
            .HasMaxLength(50)
            .HasDefaultValue("STUDENT");

        modelBuilder.Entity<StudentProfile>()
            .HasOne(s => s.Account)
            .WithMany(a => a.StudentProfiles)
            .HasForeignKey(s => s.AccountId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<TeacherProfile>()
            .HasOne(t => t.Account)
            .WithMany(a => a.TeacherProfiles)
            .HasForeignKey(t => t.AccountId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Course>()
            .HasOne(c => c.CreatedByAccount)
            .WithMany()
            .HasForeignKey(c => c.CreatedBy)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<Course>()
            .HasOne(c => c.UpdatedByAccount)
            .WithMany()
            .HasForeignKey(c => c.UpdatedBy)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<Lecture>()
            .HasOne(l => l.Course)
            .WithMany(c => c.Lectures)
            .HasForeignKey(l => l.CourseId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Lecture>()
            .HasOne(l => l.Teacher)
            .WithMany()
            .HasForeignKey(l => l.TeacherId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Quiz>()
            .HasOne(q => q.Course)
            .WithMany(c => c.Quizzes)
            .HasForeignKey(q => q.CourseId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Quiz>()
            .HasOne(q => q.Teacher)
            .WithMany()
            .HasForeignKey(q => q.TeacherId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Enrollment>()
            .HasOne(e => e.Student)
            .WithMany()
            .HasForeignKey(e => e.StudentId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Enrollment>()
            .HasOne(e => e.Course)
            .WithMany(c => c.Enrollments)
            .HasForeignKey(e => e.CourseId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Enrollment>()
            .Property(e => e.ProgressPercentage)
            .HasPrecision(18, 2);

        modelBuilder.Entity<StudentLectureProgress>()
            .HasOne(p => p.Student)
            .WithMany()
            .HasForeignKey(p => p.StudentId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<StudentLectureProgress>()
            .HasOne(p => p.Lecture)
            .WithMany(l => l.Progress)
            .HasForeignKey(p => p.LectureId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Notification>()
            .HasOne(n => n.Account)
            .WithMany()
            .HasForeignKey(n => n.AccountId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<QuizAnswer>()
            .HasOne(qa => qa.Question)
            .WithMany(q => q.Answers)
            .HasForeignKey(qa => qa.QuestionId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<QuizAnswer>()
            .HasOne(qa => qa.Attempt)
            .WithMany(a => a.Answers)
            .HasForeignKey(qa => qa.AttemptId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<QuizAnswer>()
            .HasOne(qa => qa.SelectedOption)
            .WithMany()
            .HasForeignKey(qa => qa.SelectedOptionId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<QuizAttempt>()
            .Property(q => q.Percentage)
            .HasPrecision(18, 2);

        modelBuilder.Entity<ExternalResource>()
            .Property(a => a.Provider)
            .HasMaxLength(100);

        modelBuilder.Entity<Course>()
            .Property(c => c.NameEn)
            .HasMaxLength(200);

        modelBuilder.Entity<Course>()
            .Property(c => c.NameAr)
            .HasMaxLength(200);

        modelBuilder.Entity<Lecture>()
            .Property(l => l.TitleEn)
            .HasMaxLength(200);

        modelBuilder.Entity<Lecture>()
            .Property(l => l.TitleAr)
            .HasMaxLength(200);

        modelBuilder.Entity<Quiz>()
            .Property(q => q.TitleEn)
            .HasMaxLength(200);

        modelBuilder.Entity<Quiz>()
            .Property(q => q.TitleAr)
            .HasMaxLength(200);
    }
}
