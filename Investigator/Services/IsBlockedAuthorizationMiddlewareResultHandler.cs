using Investigator.Data;
using Investigator.Repository;
using Investigator.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace Investigator.Services
{
    public class IsBlockedAuthorizationMiddlewareResultHandler : IAuthorizationMiddlewareResultHandler
    {
        private readonly AuthorizationMiddlewareResultHandler defaultHandler = new();
        private readonly AppDbContext _context;
        private IHttpContextAccessor _httpContext;
        private readonly SignInManager<IdentityUser> _signInManager;
        public IsBlockedAuthorizationMiddlewareResultHandler(AppDbContext context, IHttpContextAccessor httpContext,
            SignInManager<IdentityUser> signInManager) 
        {
            _httpContext = httpContext;
            _context = context;
            _signInManager = signInManager;
        }        
        public async Task HandleAsync(RequestDelegate next, HttpContext context, AuthorizationPolicy policy, PolicyAuthorizationResult authorizeResult)
        {
            var claimsIdentity = (ClaimsIdentity)_httpContext.HttpContext.User.Identity;
            if (!claimsIdentity.Claims.Any()) {
                context.Response.Redirect($"/Identity/Account/Login");
                return;
            }
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            var userToVerify = _context.ApplicationUsers.First(u => u.Id == userId);
            if (userToVerify == null || userToVerify.IsBlocked)
            {
                _signInManager.SignOutAsync().GetAwaiter().GetResult();
                context.Response.Redirect($"/Identity/Account/Login");
                return;
            }
            await defaultHandler.HandleAsync(next, context, policy, authorizeResult);
        }
    }
}
