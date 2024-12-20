using Investigator.Data;
using Investigator.Models;
using Investigator.Repository.IRepository;

namespace Investigator.Repository
{
    public class ResponseRepository : Repository<Response>, IResponseRepository
    {
        private readonly AppDbContext _context;
        public ResponseRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public void Update(Response response)
        {
            _context.Responses.Update(response);
        }
    }
}
