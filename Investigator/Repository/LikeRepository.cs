using Investigator.Data;
using Investigator.Models;
using Investigator.Repository.IRepository;

namespace Investigator.Repository
{
    public class LikeRepository : Repository<Like>, ILikeRepository
    {
        private readonly AppDbContext _context;
        public LikeRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public void Update(Like like)
        {
            _context.Likes.Update(like);
        }
    }
}
