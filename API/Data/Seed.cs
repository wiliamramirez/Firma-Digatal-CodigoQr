using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using API.Entities;


namespace API.Data
{
    public class Seed
    {
        public static async Task SeedData(DataContext context)
        {
            if (!context.Users.Any() && !context.Roles.Any())
            {
                var roles = new List<Role>
                {
                    new Role
                    {
                        Id = Guid.NewGuid(),
                        Name = "admin"
                    },
                    new Role
                    {
                        Id = Guid.NewGuid(),
                        Name = "user"
                    },
                };

                await context.Roles.AddRangeAsync(roles);

                var user = new AppUser
                {
                    Dni = "12345678",
                    UserName = "admin",
                    Email = "admin@admin.com",
                    FullName = "admin",
                    LastName = "admon",
                    Id = Guid.NewGuid(),
                    Position = "admin",
                };


                using var hmac = new HMACSHA512();
                user.UserName = user.UserName.ToLower();
                user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes("P@ssword"));
                user.PasswordSalt = hmac.Key;
                context.Users.Add(user);

                await context.Users.AddAsync(user);

                var userRole = new List<UserRole>
                {
                    new UserRole
                    {
                        Role = roles[0],
                        AppUser = user
                    },
                    new UserRole
                    {
                        Role = roles[1],
                        AppUser = user
                    },
                };

                await context.UserRoles.AddRangeAsync(userRole);

                await context.SaveChangesAsync();
            }
        }
    }
}