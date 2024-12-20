using Investigator.Models;

namespace Investigator.Repository.IRepository
{
    public interface IFormRepository:IRepository<Form>
    {
        void Update(Form form);
    }
}
