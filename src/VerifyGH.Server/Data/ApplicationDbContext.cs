using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using VerifyGH.Server.Models;

namespace VerifyGH.Server.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public DbSet<Project> Projects => Set<Project>();

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Project>(e =>
        {
            e.HasOne(p => p.Student)
             .WithMany()
             .HasForeignKey(p => p.StudentUserId)
             .OnDelete(DeleteBehavior.Restrict);

            e.HasOne(p => p.SupervisorLecturer)
             .WithMany()
             .HasForeignKey(p => p.SupervisorLecturerId)
             .OnDelete(DeleteBehavior.SetNull);
        });
    }
}
