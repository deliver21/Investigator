using Investigator.Data;
using Investigator.Models;
using Investigator.Repository.IRepository;

namespace Investigator.Repository
{
    public class FormRepository : Repository<Form>, IFormRepository
    {
        private readonly AppDbContext _context;
        public FormRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public void Update(Form form)
        {
            try
            {
                _context.Forms.Update(form);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
