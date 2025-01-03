using Investigator.Data;
using Investigator.Models;
using Investigator.Utilities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Investigator.DbInitializer
{
    public class DbInitializer:IDbInitializer
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly AppDbContext _context;
        public DbInitializer(UserManager<IdentityUser>userManager,RoleManager<IdentityRole>roleManager, AppDbContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
        }
        public void Initialize()
        {
            try
            {
                if (!_context.Database.GetPendingMigrations().Any())
                {
                    _context.Database.Migrate();
                }
            }
            catch { }

            if(!_roleManager.RoleExistsAsync(SD.CustomerRole).GetAwaiter().GetResult())
            {
                _roleManager.CreateAsync(new IdentityRole(SD.CustomerRole)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(SD.AdminRole)).GetAwaiter().GetResult();
            }

            if(!_context.ApplicationUsers.Any(u => u.Email == "matecode@gmail.com"))
            {
                var result = _userManager.CreateAsync(new ApplicationUser
                {
                    Email = "matecode@gmail.com",
                    UserName = "Mate Code"
                }, "Matecode1.").GetAwaiter().GetResult();

                if (result.Succeeded)
                {
                    var user = _context.ApplicationUsers.FirstOrDefault(u => u.Email == "matecode@gmail.com");
                    _userManager.AddToRoleAsync(user, SD.AdminRole);
                }
            }                      
        }
    }
}
