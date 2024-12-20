using Investigator.Models;

namespace Investigator.Repository.IRepository
{
    public interface ILikeRepository:IRepository<Like>
    {
        void Update(Like like);   
    }
}
