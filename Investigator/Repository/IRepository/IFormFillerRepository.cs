using Investigator.Models;

namespace Investigator.Repository.IRepository
{
    public interface IFormFillerRepository:IRepository<FormFiller>
    {
        void Update(FormFiller formFiller);
    }
}
