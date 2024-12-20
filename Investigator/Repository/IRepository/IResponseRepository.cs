using Investigator.Models;

namespace Investigator.Repository.IRepository
{
    public interface IResponseRepository:IRepository<Response>
    {
        void Update(Response response);
    }
}
