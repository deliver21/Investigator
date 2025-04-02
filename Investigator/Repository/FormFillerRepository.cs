using Investigator.Data;
using Investigator.Models;
using Investigator.Repository.IRepository;

namespace Investigator.Repository
{
    public class FormFillerRepository : Repository<FormFiller>, IFormFillerRepository
    {
        private readonly AppDbContext _context;
        public FormFillerRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public void Update(FormFiller formFiller)
        {
            _context.FormFillers.Update(formFiller);
        }
    }
}
