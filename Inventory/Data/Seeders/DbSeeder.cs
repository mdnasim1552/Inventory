
using Inventory.Extensions;
using Inventory.Models;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Data.Seeders
{
    public static class DbSeeder
    {
        public static async Task SeedAsync(ApplicationDbContext db)
        {
            // Ensure database is created
            await db.Database.EnsureCreatedAsync();

            // 1️⃣ Seed Roles
            if (!await db.Userroles.AnyAsync())
            {
                var roles = new List<Userrole>
                {
                    new Userrole { Role = Policies.HumanResource },
                    new Userrole { Role = Policies.Admin },
                    new Userrole { Role = Policies.Manager },
                    new Userrole { Role = Policies.User },
                    new Userrole { Role = Policies.Guest },
                    new Userrole { Role = Policies.Moderator },
                    new Userrole { Role = Policies.Auditor },
                    new Userrole { Role = Policies.Support }
                };

                db.Userroles.AddRange(roles);
                await db.SaveChangesAsync();
            }
            //// 2️⃣ Seed Users
            //if (!await db.Users.AnyAsync())
            //{
            //    var users = new List<User>
            //    {
            //        new User
            //        {
            //            FullName = "Super Admin",
            //            Email = "admin@example.com",
            //            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
            //            CreatedAt = DateTime.UtcNow
            //        },
            //        new User
            //        {
            //            FullName = "Manager User",
            //            Email = "manager@example.com",
            //            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Manager@123"),
            //            CreatedAt = DateTime.UtcNow
            //        },
            //        new User
            //        {
            //            FullName = "Editor User",
            //            Email = "editor@example.com",
            //            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Editor@123"),
            //            CreatedAt = DateTime.UtcNow
            //        }
            //    };

            //    db.Users.AddRange(users);
            //    await db.SaveChangesAsync();
            //}

            //// 3️⃣ Seed UserRoles
            //if (!await db.UserRoles.AnyAsync())
            //{
            //    var admin = await db.Users.FirstAsync(u => u.Email == "admin@example.com");
            //    var manager = await db.Users.FirstAsync(u => u.Email == "manager@example.com");
            //    var editor = await db.Users.FirstAsync(u => u.Email == "editor@example.com");

            //    var adminRole = await db.Roles.FirstAsync(r => r.Name == "Admin");
            //    var managerRole = await db.Roles.FirstAsync(r => r.Name == "Manager");
            //    var editorRole = await db.Roles.FirstAsync(r => r.Name == "Editor");

            //    var userRoles = new List<UserRole>
            //    {
            //        new UserRole { UserId = admin.Id, RoleId = adminRole.Id, AssignedDate = DateTime.UtcNow },
            //        new UserRole { UserId = manager.Id, RoleId = managerRole.Id, AssignedDate = DateTime.UtcNow },
            //        new UserRole { UserId = editor.Id, RoleId = editorRole.Id, AssignedDate = DateTime.UtcNow }
            //    };

            //    db.UserRoles.AddRange(userRoles);
            //    await db.SaveChangesAsync();
            //}
            if (!await db.Units.AnyAsync())
            {
                var units = new List<Unit> {
                    new Unit { Name = "Kilogram (kg)",ShortName="kg" },
                    new Unit { Name = "Gram (gm)",ShortName="gm"},
                    new Unit { Name = "Litre (L)",ShortName="L" },
                    new Unit { Name = "Milliliter (ml)",ShortName="ml" },
                    new Unit { Name = "Unit (pcs)",ShortName="Pcs" },
                    new Unit { Name = "Meter (m)",ShortName="m" },
                };
                db.Units.AddRange(units);
                await db.SaveChangesAsync();
            }
        }
    }
}
