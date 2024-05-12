using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities.Identity;

namespace Talabat.Repositotry._Identity
{
    public  class ApplicationIdentityDataSeed
    {
        public static async Task SeedUsersAsync(UserManager<ApplicationUser> userManager)
        {
            if (!userManager.Users.Any())
            {
                var user = new ApplicationUser()
                {

                    DisplayName = "Sara Mohamed",
                    Email = "saramohamed3738@gmail.com",
                    UserName = "saramohamed",
                    PhoneNumber = "0115034567"
                }; 
                    await userManager.CreateAsync(user, "P@ssw0rd");
            }
        }
    }
}
