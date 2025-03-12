using Investigator.Repository.IRepository;
using Microsoft.AspNetCore.SignalR;

namespace Investigator.Hubs
{
    public class TemplateDetailsHub:Hub
    {
        private readonly IUnitOfWork _unit;
        public static List<string> TemplatesJoined { get; set; } = new List<string>();
        public static int TemplateLikes { get; set; } = 0;
        public TemplateDetailsHub(IUnitOfWork unit)
        {
            _unit = unit;
        }
        public async Task JoinTemplate(int templateId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, templateId.ToString());
            TemplateLikes = _unit.Like.GetAll(u => u.TemplateId == templateId).Count();
            await Clients.Group(templateId.ToString()).SendAsync("getLikes", TemplateLikes);
        }
    }
}
