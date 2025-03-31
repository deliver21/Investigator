using Investigator.Data;
using Investigator.Models;
using Investigator.Repository.IRepository;
using Salesforce.Chatter.Models;

namespace Investigator.Repository
{
    public class TemplateQuestionRepository : Repository<TemplateQuestion>, ITemplateQuestionRepository 
    {
        private readonly AppDbContext _context;
        public TemplateQuestionRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }
        public void Update(TemplateQuestion templateQuestion)
        {
            _context.TemplateQuestions.Update(templateQuestion);
        }
    }
}
