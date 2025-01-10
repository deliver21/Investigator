using Google.Apis.Drive.v3.Data;
using Investigator.Data;
using Investigator.Models;
using Investigator.Repository.IRepository;
using Investigator.Services;
using Investigator.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Investigator.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.AdminRole)]
    [IsBlockedAuthorize]    
    public class UserController : Controller
    {
        private readonly IUnitOfWork _unit;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        public UserController(IUnitOfWork unit, SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager) 
        {
            _unit = unit;
            _signInManager = signInManager;
            _userManager = userManager;
        }
        [Authorize]
        [IsBlockedAuthorize]
        public IActionResult Index()
        {
            return View();
        }

        #region APIs Calls
        [HttpGet]
        public IActionResult GetAll()
        {
            IEnumerable<ApplicationUser> obj = _unit.ApplicationUser.GetAll();
            foreach (ApplicationUser user in obj)
            {
                var formatTime = DateTimeFormat.FormatString(user.LastSeen);
                user.Interval = $"Last seen {Interval.SetInterval(user.LastSeen)}";
                user.LastSeen = DateTime.Parse(formatTime);
                user.Role = _userManager.GetRolesAsync(user).GetAwaiter().GetResult().FirstOrDefault();
            }
            return Json(new { data = obj.OrderByDescending(u => u.LastSeen) });
        }
        [Authorize]
        [IsBlockedAuthorize]
        [HttpPut]
        public async Task<IActionResult> SwitchRole(string? id)
        {
            IdentityResult result;
            var userToUpdate = await _unit.ApplicationUser.Get(u => u.Id == id,null,true);
            if (userToUpdate == null)
            {
                return Json(new { success = false, message = "Error while changing role" });
            }
            var oldRole = _userManager.GetRolesAsync(userToUpdate).GetAwaiter().GetResult().FirstOrDefault();
            
            result = oldRole == SD.CustomerRole ? _userManager.AddToRoleAsync(userToUpdate, SD.AdminRole).GetAwaiter().GetResult() 
                : _userManager.AddToRoleAsync(userToUpdate, SD.CustomerRole).GetAwaiter().GetResult();

            if (!result.Succeeded)
            {
                return Json(new { success = false, message = $"An error occured while changing user {userToUpdate.UserName} role" });
            }
            if (!string.IsNullOrEmpty(oldRole))
            {
                result = _userManager.RemoveFromRoleAsync(userToUpdate, oldRole).GetAwaiter().GetResult();
            }
            if (!result.Succeeded)
            {
                return Json(new { success = false, message = $"An error occured while changing user {userToUpdate.UserName} role" });
            }
            return Json(new { success = true, message = $"a new role is assigned to {userToUpdate.UserName}" });
        }

        
        [Authorize(Roles = SD.AdminRole)]
        [IsBlockedAuthorize]
        [HttpPost]
        public async Task<IActionResult> BulkDelete([FromBody] List<string> userIds)
        {
            if (userIds == null || !userIds.Any())
            {
                return BadRequest(new { message = "No users selected for deletion." });
            }
            try
            {
                foreach (var userId in userIds)
                {
                    var user =  await _unit.ApplicationUser.Get(u => u.Id == userId);
                    if (user != null)
                    {
                        _unit.ApplicationUser.Remove(user);
                    }
                }
                _unit.Save();
                return Ok(new { message = $"{userIds.Count} user(s) deleted successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error occurred during deletion.", error = ex.Message });
            }
        }
        [Authorize(Roles = SD.AdminRole)]
        [IsBlockedAuthorize]

        [HttpPost]
        public async Task<IActionResult> BulkLock([FromBody] List<string> ids)
        {
            foreach (var id in ids)
            {
                var user = await _unit.ApplicationUser.Get(u => u.Id == id);
                if (user != null && !user.IsBlocked)
                {
                    user.IsBlocked = true; // Set blocked flag
                    _unit.ApplicationUser.Update(user);
                    _unit.Save();
                }
            }
            
            return Json(new { message = "Selected users have been blocked." });
        }
        [Authorize(Roles = SD.AdminRole)]
        [IsBlockedAuthorize]
        [HttpPost]
        public async Task<IActionResult> BulkUnlock([FromBody] List<string> ids)
        {
            foreach (var id in ids)
            {
                var user = await _unit.ApplicationUser.Get(u => u.Id == id);
                if (user != null && user.IsBlocked)
                {
                    user.IsBlocked = false; // Remove blocked flag
                    _unit.ApplicationUser.Update(user);
                }
            }
            _unit.Save();
            return Json(new { message = "Selected users have been unlocked." });
        }
        #endregion
    }
}
