using Investigator.Models;

namespace Investigator.Services.IServices
{
    public interface ISalesForceService
    {
        Task<bool> CreateUserToSalesForce(ApplicationUser user);
    }
}
