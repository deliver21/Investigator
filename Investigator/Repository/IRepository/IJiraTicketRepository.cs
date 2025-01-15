using Investigator.Models;

namespace Investigator.Repository.IRepository
{
    public interface IJiraTicketRepository:IRepository<JiraTicket>
    {
        void Update(JiraTicket jiraTicket);
    }
}
