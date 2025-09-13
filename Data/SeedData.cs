using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Burg_Storage.Models;

namespace Burg_Storage.Data
{
    /// <summary>
    /// Seeds initial application data like the default admin user.
    /// </summary>
    public static class SeedData
    {
        private const string DefaultEmail = "admin@admin.de";
        private const string DefaultPassword = "admin";

        public static async Task InitializeAsync(IServiceProvider services)
        {
            var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

            if (await userManager.FindByEmailAsync(DefaultEmail) == null)
            {
                var user = new ApplicationUser
                {
                    UserName = DefaultEmail,
                    Email = DefaultEmail,
                    EmailConfirmed = true
                };

                await userManager.CreateAsync(user, DefaultPassword);
            }
        }
    }
}
