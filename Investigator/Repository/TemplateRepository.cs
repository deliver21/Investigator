using Investigator.Data;
using Investigator.Models;
using Investigator.Repository.IRepository;

namespace Investigator.Repository
{
    public class TemplateRepository : Repository<Template>, ITemplateRepository
    {
        private readonly AppDbContext _context;
        public TemplateRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public void Update(Template template)
        {
            _context.Templates.Update(template);
        }
    }
}
