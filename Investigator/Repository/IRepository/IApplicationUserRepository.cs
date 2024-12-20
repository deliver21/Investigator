using Investigator.Models;

namespace Investigator.Repository.IRepository
{
    public interface IApplicationUserRepository:IRepository<ApplicationUser>
    {
        void Update(ApplicationUser applicationUser);
    }
}
