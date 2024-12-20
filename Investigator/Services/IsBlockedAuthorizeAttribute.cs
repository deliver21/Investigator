using Microsoft.AspNetCore.Authorization;

namespace Investigator.Services
{
    public class IsBlockedAuthorizeAttribute : AuthorizeAttribute 
    {
        public IsBlockedAuthorizeAttribute() 
        { 
            Policy = "IsBlockPolicy"; 
        }
    }
}
