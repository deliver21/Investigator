using Investigator.Data;
using Investigator.Models;
using Investigator.Repository.IRepository;

namespace Investigator.Repository
{
    public class CommentRepository : Repository<Comment>, ICommentRepository
    {
        private readonly AppDbContext _context;
        public CommentRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public void Update(Comment comment)
        {
            _context.Comments.Update(comment);
        }
    }
}
