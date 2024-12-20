using Investigator.Models;

namespace Investigator.Repository.IRepository
{
    public interface IQuestionOptionRepository:IRepository<QuestionOption>
    {
        void Update(QuestionOption questionOption);
    }
}
