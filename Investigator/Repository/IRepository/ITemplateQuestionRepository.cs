using Investigator.Models;

namespace Investigator.Repository.IRepository
{
    public interface ITemplateQuestionRepository:IRepository<TemplateQuestion>
    {
        void Update(TemplateQuestion templateQuestion);
    }
}
