using Investigator.Data;
using Investigator.Models;
using Investigator.Repository.IRepository;

namespace Investigator.Repository
{
    public class QuestionOptionRepository : Repository<QuestionOption>, IQuestionOptionRepository
    {
        private readonly AppDbContext _context;
        public QuestionOptionRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public void Update(QuestionOption questionOption)
        {
            _context.QuestionOptions.Update(questionOption);
        }
    }
}
