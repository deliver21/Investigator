using Investigator.Data;
using Investigator.Models;
using Investigator.Repository.IRepository;

namespace Investigator.Repository
{
    public class ApplicationUserRepository:Repository<ApplicationUser>,IApplicationUserRepository
    {
        private readonly AppDbContext _context;
        public ApplicationUserRepository(AppDbContext context):base(context) 
        {
            _context = context;
        }
        public void Update(ApplicationUser user)
        {
            _context.ApplicationUsers.Update(user);
        }
    }
}
