using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Talabat.DAL.Identity
{
    public static class AppIdentityDbContextSeed
    {
        public static async Task SeedUserAsync(UserManager<AppUser> userManager)
        {
            if (!userManager.Users.Any())
            {
                var user = new AppUser()
                {
                    DisplayName = "Amr Ayman",
                    UserName = "amr",
                    Email = "amr@gmail.com",
                    PhoneNumber = "4618161668",
                    Address = new Address
                    {
                        FirstName = "Amr",
                        LastName = "Ayman",
                        Country = "Egypt",
                        City = "Cairo",
                        Street = "16 Tahrir street",
                        ZipCode = "896186"
                    }
                };

                await userManager.CreateAsync(user, "Pa$$");
            }
        }
    }
}
