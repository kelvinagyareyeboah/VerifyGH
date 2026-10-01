using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using VerifyGH.Server.Models;

namespace VerifyGH.Server.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }

    // ── DbSets ────────────────────────────────────────────────────────────────

    public DbSet<Project>            Projects            => Set<Project>();
    public DbSet<VerificationReview> VerificationReviews => Set<VerificationReview>();
    public DbSet<Skill>              Skills              => Set<Skill>();
    public DbSet<ProjectSkill>       ProjectSkills       => Set<ProjectSkill>();

    // ── Model Configuration ───────────────────────────────────────────────────

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // ── Project ───────────────────────────────────────────────────────────

        builder.Entity<Project>(e =>
        {
            e.HasKey(p => p.Id);

            e.Property(p => p.Title)
             .IsRequired()
             .HasMaxLength(200);

            e.Property(p => p.Description)
             .IsRequired();

            e.Property(p => p.SkillsTags)
             .HasMaxLength(1000)
             .HasDefaultValue(string.Empty);

            e.Property(p => p.Status)
             .HasConversion<string>()
             .HasMaxLength(30);

            // Student FK — restrict delete so we keep audit trail
            e.HasOne(p => p.Student)
             .WithMany()
             .HasForeignKey(p => p.StudentUserId)
             .OnDelete(DeleteBehavior.Restrict);

            // Supervisor FK — nullable, set null when lecturer deleted
            e.HasOne(p => p.SupervisorLecturer)
             .WithMany()
             .HasForeignKey(p => p.SupervisorLecturerId)
             .OnDelete(DeleteBehavior.SetNull)
             .IsRequired(false);
        });

        // ── VerificationReview ────────────────────────────────────────────────

        builder.Entity<VerificationReview>(e =>
        {
            e.HasKey(r => r.Id);

            e.Property(r => r.Status)
             .HasConversion<string>()
             .HasMaxLength(30);

            e.HasOne(r => r.Project)
             .WithMany(p => p.Reviews)
             .HasForeignKey(r => r.ProjectId)
             .OnDelete(DeleteBehavior.Cascade);

            e.HasOne(r => r.Lecturer)
             .WithMany()
             .HasForeignKey(r => r.LecturerId)
             .OnDelete(DeleteBehavior.Restrict);
        });

        // ── Skill ─────────────────────────────────────────────────────────────

        builder.Entity<Skill>(e =>
        {
            e.HasKey(s => s.Id);

            e.Property(s => s.Name)
             .IsRequired()
             .HasMaxLength(100);

            e.Property(s => s.Category)
             .HasMaxLength(50)
             .HasDefaultValue("General");

            e.HasIndex(s => s.Name).IsUnique();
        });

        // ── ProjectSkill (many-to-many join) ──────────────────────────────────

        builder.Entity<ProjectSkill>(e =>
        {
            e.HasKey(ps => new { ps.ProjectId, ps.SkillId });

            e.HasOne(ps => ps.Project)
             .WithMany(p => p.ProjectSkills)
             .HasForeignKey(ps => ps.ProjectId)
             .OnDelete(DeleteBehavior.Cascade);

            e.HasOne(ps => ps.Skill)
             .WithMany(s => s.ProjectSkills)
             .HasForeignKey(ps => ps.SkillId)
             .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
