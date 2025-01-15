using Investigator.Models;

namespace Investigator.Services.IServices
{
    public interface IJiraService
    {
        Task<string> CreateTicketAsync(JiraTicket ticket, string reportedBy, string templateTitle);
    }
}
