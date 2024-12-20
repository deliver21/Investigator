using Investigator.Models;

namespace Investigator.Repository.IRepository
{
    public interface IQuestionRepository:IRepository<Question>
    {
        void Update(Question question);
    }
}
