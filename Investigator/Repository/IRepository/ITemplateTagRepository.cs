using Investigator.Models;

namespace Investigator.Repository.IRepository
{
    public interface ITemplateTagRepository:IRepository<TemplateTag>
    {
        void Update(TemplateTag templateTag);
    }
}
