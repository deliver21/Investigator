using Investigator.Data;
using Investigator.Models;
using Investigator.Repository.IRepository;

namespace Investigator.Repository
{
    public class TemplateTagRepository : Repository<TemplateTag>,ITemplateTagRepository
    {
        private readonly AppDbContext _context;
        public TemplateTagRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public void Update(TemplateTag templateTag)
        {
            _context.TemplateTags.Update(templateTag);
        }
    }
}
