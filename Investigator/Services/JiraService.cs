using Investigator.Models;
using Investigator.Repository.IRepository;
using Investigator.Services.IServices;
using Newtonsoft.Json;
using System.Text;

namespace Investigator.Services
{
    public class JiraService:IJiraService
    {

        private readonly IUnitOfWork _unit;
        private readonly string _baseUrl;
        private readonly string _apiToken;
        private readonly string _email;
        private readonly string _projectKey;
        public JiraService(IConfiguration config, IUnitOfWork unit)
        {
            _unit = unit;
            _baseUrl = config.GetSection("JiraDependecies").GetValue<string>("JiraDomain");
            _apiToken = config.GetSection("JiraDependecies").GetValue<string>("APIToken");
            _email = config.GetSection("JiraDependecies").GetValue<string>("Email");
            _projectKey = config.GetSection("JiraDependecies").GetValue<string>("JiraProjectKey");
        }
        public async Task<string> CreateTicketAsync(JiraTicket ticket, string reportedBy, string? templateTitle)
        {
            if(string.IsNullOrEmpty(templateTitle))
            {
                templateTitle = "N/A";
            }
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(
                "Basic", Convert.ToBase64String(Encoding.ASCII.GetBytes($"{_email}:{_apiToken}"))
            );

            var payload = new
            {
                fields = new
                {
                    summary = ticket.Summary,
                    priority = new { name = ticket.Priority },
                    description = $"Reported by: {reportedBy}\nTemplate: { templateTitle }\nPage Link: {ticket.PageLink}",
                    issuetype = new { name = "Task" },
                    project = new { key = _projectKey }
                }
            };

            var response = await client.PostAsync(
                $"{_baseUrl}/rest/api/2/issue",
                new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json")
            );

            //response.EnsureSuccessStatusCode();
            if(response.IsSuccessStatusCode)
            {
                var responseData = JsonConvert.DeserializeObject<dynamic>(await response.Content.ReadAsStringAsync());
                string jiraTicketKey = responseData.key;
                string jiraTicketUrl = $"{_baseUrl}/browse/{jiraTicketKey}";                

                ticket.JiraLink = jiraTicketUrl;
                ticket.Status = "Opened";
                ticket.CreatedByUserId = _unit.ApplicationUser.Get(u => u.UserName == reportedBy).GetAwaiter().GetResult().Id;
                ticket.CreatedDate = DateTime.Now;
                if(!string.IsNullOrEmpty(templateTitle) && !templateTitle.Contains("N/A"))
                {
                    ticket.TemplateTitle = templateTitle;
                }
                await _unit.JiraTicket.Add(ticket);
                _unit.Save();
                return jiraTicketUrl;
            }
            return string.Empty;
        }
    }
}
