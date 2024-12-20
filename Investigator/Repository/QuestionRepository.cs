using Investigator.Data;
using Investigator.Models;
using Investigator.Repository.IRepository;

namespace Investigator.Repository
{
    public class QuestionRepository : Repository<Question>, IQuestionRepository
    {
        private readonly AppDbContext _context;
        public QuestionRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public void Update(Question question)
        {
            _context.Questions.Update(question);
        }
    }
}
