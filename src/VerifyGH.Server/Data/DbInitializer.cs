using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using VerifyGH.Server.Models;
using VerifyGH.Shared.Enums;

namespace VerifyGH.Server.Data;

public static class DbInitializer
{
    public static async Task SeedRolesAndAdminAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var context     = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var logger      = scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("DbInitializer");

        // ── 1. Ensure database is up to date ──────────────────────────────────
        if (context.Database.IsSqlite())
        {
            await context.Database.EnsureCreatedAsync();
        }
        else
        {
            await context.Database.MigrateAsync();
        }

        // ── 2. Seed Roles ─────────────────────────────────────────────────────
        foreach (var role in Enum.GetNames<UserRole>())
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                var result = await roleManager.CreateAsync(new IdentityRole(role));
                if (result.Succeeded)
                    logger.LogInformation("Seeded role: {Role}", role);
            }
        }

        // ── 3. Seed Admin ─────────────────────────────────────────────────────
        var adminUser = await EnsureUser(userManager, logger,
            email: "admin@verifygh.edu.gh",
            fullName: "System Administrator",
            institution: "VerifyGH National Verification Portal",
            department: "ICT & Administration",
            role: UserRole.Admin,
            password: "Admin@123456");

        // ── 4. Seed Sample Lecturers ──────────────────────────────────────────
        var lecturer1 = await EnsureUser(userManager, logger,
            email: "msoli@ug.edu.gh",
            fullName: "Dr. Michael Soli",
            institution: "University of Ghana",
            department: "Department of Computer Science",
            role: UserRole.Lecturer,
            password: "Lecturer@123");

        var lecturer2 = await EnsureUser(userManager, logger,
            email: "iwiafe@ug.edu.gh",
            fullName: "Dr. Isaac Wiafe",
            institution: "University of Ghana",
            department: "Department of Computer Science",
            role: UserRole.Lecturer,
            password: "Lecturer@123");

        var lecturer3 = await EnsureUser(userManager, logger,
            email: "eowusu@ug.edu.gh",
            fullName: "Dr. Ebenezer Owusu",
            institution: "University of Ghana",
            department: "Department of Computer Science",
            role: UserRole.Lecturer,
            password: "Lecturer@123");

        var lecturer4 = await EnsureUser(userManager, logger,
            email: "jabdulai@ug.edu.gh",
            fullName: "Dr. Jamal-Deen Abdulai",
            institution: "University of Ghana",
            department: "Department of Computer Science",
            role: UserRole.Lecturer,
            password: "Lecturer@123");

        var lecturer5 = await EnsureUser(userManager, logger,
            email: "pokae@ug.edu.gh",
            fullName: "Dr. Percy Okae",
            institution: "University of Ghana",
            department: "Department of Computer Science",
            role: UserRole.Lecturer,
            password: "Lecturer@123");

        // ── 5. Seed Sample Students ───────────────────────────────────────────
        var student1 = await EnsureUser(userManager, logger,
            email: "onlykelvin06@gmail.com",
            fullName: "Kelvin Agyare Yeboah",
            institution: "University of Ghana",
            department: "BSc Computer Science",
            role: UserRole.Student,
            password: "Student@123",
            studentId: "22159683");

        var student2 = await EnsureUser(userManager, logger,
            email: "abena@st.ug.edu.gh",
            fullName: "Abena Mensah",
            institution: "KNUST",
            department: "BSc Software Engineering",
            role: UserRole.Student,
            password: "Student@123",
            studentId: "22045612");

        // ── 6. Seed Sample Employer ───────────────────────────────────────────
        await EnsureUser(userManager, logger,
            email: "talent@hubtel.com",
            fullName: "Kwadwo Mensah",
            institution: "Hubtel Ghana",
            department: "Talent Acquisition",
            role: UserRole.Employer,
            password: "Employer@123");

        // ── 7. Seed Skills ────────────────────────────────────────────────────
        if (!await context.Skills.AnyAsync())
        {
            var skills = new[]
            {
                new Skill { Name = "C#",           Category = "Backend"  },
                new Skill { Name = "ASP.NET Core", Category = "Backend"  },
                new Skill { Name = "Blazor",       Category = "Frontend" },
                new Skill { Name = "SQL",          Category = "Database" },
                new Skill { Name = "Entity Framework", Category = "Database" },
                new Skill { Name = "React",        Category = "Frontend" },
                new Skill { Name = "TypeScript",   Category = "Frontend" },
                new Skill { Name = "Python",       Category = "Backend"  },
                new Skill { Name = "Machine Learning", Category = "AI/ML" },
                new Skill { Name = "Docker",       Category = "DevOps"   },
                new Skill { Name = "SignalR",      Category = "Backend"  },
                new Skill { Name = "REST API",     Category = "Backend"  },
                new Skill { Name = "Git",          Category = "DevOps"   },
            };
            context.Skills.AddRange(skills);
            await context.SaveChangesAsync();
            logger.LogInformation("Seeded {Count} skills.", skills.Length);
        }

        // ── 8. Seed Sample Projects ───────────────────────────────────────────
        if (!await context.Projects.AnyAsync() && student1 is not null && lecturer1 is not null)
        {
            var projects = new[]
            {
                new Project
                {
                    Title                = "Blockchain Transcript Verification System",
                    Description          = "A tamper-proof academic transcript verification platform using distributed ledger technology for Ghanaian universities.",
                    RepositoryUrl        = "https://github.com/kelvin-yeboah/blockchain-transcript",
                    LiveDemoUrl          = "https://blockchain-transcript.vercel.app",
                    SkillsTags           = "C#|Blazor|SQL|ASP.NET Core",
                    Status               = VerificationStatus.Approved,
                    LecturerFeedback     = "Excellent work. The smart contract integration is well implemented.",
                    VerifiedAt           = DateTime.UtcNow.AddDays(-5),
                    CreatedAt            = DateTime.UtcNow.AddDays(-20),
                    StudentUserId        = student1.Id,
                    SupervisorLecturerId = lecturer1.Id,
                },
                new Project
                {
                    Title                = "AI Lecture Scheduler",
                    Description          = "Machine learning model that auto-schedules university lectures minimising room conflicts and lecturer fatigue.",
                    RepositoryUrl        = "https://github.com/kelvin-yeboah/ai-scheduler",
                    SkillsTags           = "Python|Machine Learning|REST API",
                    Status               = VerificationStatus.Pending,
                    CreatedAt            = DateTime.UtcNow.AddDays(-3),
                    StudentUserId        = student1.Id,
                    SupervisorLecturerId = lecturer2.Id,
                },
                new Project
                {
                    Title                = "Campus Bus Tracker",
                    Description          = "Real-time GPS bus tracking for UG campus shuttles with SignalR live updates.",
                    RepositoryUrl        = "https://github.com/abena-mensah/bus-tracker",
                    LiveDemoUrl          = "https://ug-bus-tracker.netlify.app",
                    SkillsTags           = "React|TypeScript|SignalR",
                    Status               = VerificationStatus.NeedsRevision,
                    LecturerFeedback     = "Good concept. Please add error handling for GPS signal loss and improve the mobile UI.",
                    CreatedAt            = DateTime.UtcNow.AddDays(-10),
                    StudentUserId        = student2 is not null ? student2.Id : student1.Id,
                    SupervisorLecturerId = lecturer1.Id,
                },
            };

            context.Projects.AddRange(projects);
            await context.SaveChangesAsync();
            logger.LogInformation("Seeded {Count} sample projects.", projects.Length);
        }
    }

    // ── Helper ─────────────────────────────────────────────────────────────────

    private static async Task<ApplicationUser?> EnsureUser(
        UserManager<ApplicationUser> userManager,
        ILogger logger,
        string email, string fullName, string institution,
        string department, UserRole role, string password,
        string? studentId = null)
    {
        var existing = await userManager.FindByEmailAsync(email);
        if (existing is not null) return existing;

        var user = new ApplicationUser
        {
            UserName       = email,
            Email          = email,
            EmailConfirmed = true,
            FullName       = fullName,
            Institution    = institution,
            Department     = department,
            StudentId      = studentId,
            Role           = role,
            CreatedAt      = DateTime.UtcNow
        };

        var result = await userManager.CreateAsync(user, password);
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(user, role.ToString());
            logger.LogInformation("Seeded user: {Email} ({Role})", email, role);
            return user;
        }

        logger.LogWarning("Failed to seed user {Email}: {Errors}",
            email, string.Join(", ", result.Errors.Select(e => e.Description)));
        return null;
    }
}
