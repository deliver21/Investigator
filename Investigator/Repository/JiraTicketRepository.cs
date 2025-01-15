using Investigator.Data;
using Investigator.Models;
using Investigator.Repository.IRepository;

namespace Investigator.Repository
{
    public class JiraTicketRepository : Repository<JiraTicket>, IJiraTicketRepository
    {
        private readonly AppDbContext _context;
        public JiraTicketRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public void Update(JiraTicket jiraTicket)
        {
            _context.JiraTickets.Update(jiraTicket);
        }
    }
}
