using Investigator.Models;

namespace Investigator.Repository.IRepository
{
    public interface ITemplateRepository:IRepository<Template>
    {
        void Update(Template template);
    }
}
