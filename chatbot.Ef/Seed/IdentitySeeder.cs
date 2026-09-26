using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.Authorization.System;
using Microsoft.AspNetCore.Identity;

namespace chatbot.Ef.Seed
{
    public static class IdentitySeeder
    {
        public static async Task SeedRolesAsync(RoleManager<IdentityRole<Guid>> roleManager)
        {
            var roles = new[]
            {
            SystemRoles.User,
            SystemRoles.Admin,
            SystemRoles.SuperAdmin
            };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    var result =
                        await roleManager.CreateAsync(
                            new IdentityRole<Guid>(role));

                    if (!result.Succeeded)
                    {
                        throw new Exception(
                            $"Failed to create role: {role}");
                    }
                }
            }
        }
    }
}