using Investigator.Models;

namespace Investigator.Repository.IRepository
{
    public interface ICommentRepository:IRepository<Comment>
    {
        void Update(Comment comment);
    }
}
