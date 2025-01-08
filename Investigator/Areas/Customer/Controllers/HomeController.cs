using Investigator.Models;
using Investigator.Utilities;
using Investigator.Repository.IRepository;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Localization;
using System.Diagnostics;
using Investigator.Models.ViewModels;
using System.Security.Claims;

namespace Investigator.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _unit;
        private readonly IHtmlLocalizer<HomeController> _localizer;
        [BindProperty]
        public TemplateVM TemplateVM { get; set; }
        public HomeController(ILogger<HomeController> logger, IUnitOfWork unit, IHtmlLocalizer<HomeController> localizer)
        {
            _logger = logger;
            _localizer= localizer;
            _unit = unit;
        } 

        public IActionResult Index()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            string ? userId = null;
            if (claimsIdentity.Claims.Any())
            {
                userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            }
            TemplateVM = new()
            {
                Template = _unit.Template.GetAll(u => u.Visibility == 0, "Questions").OrderByDescending(u => u.Point).Take(5),
                Form = userId != null ? _unit.Form.GetAll(u => u.CreatorId == userId, "Template").ToList() : new List<Form>()
            };
            if(TemplateVM.Form.Any())
            {
                for(int i = 0; i < TemplateVM.Form.Count(); i++)
                {
                    var id = TemplateVM.Form[i].TemplateId;
                    TemplateVM.Form[i].Template = _unit.Template.Get(u => u.TemplateId == id);  
                    if(TemplateVM.Form[i].Template == null)
                    {
                        TemplateVM.Form[i].Template = new();
                    }
                }
            }
            return View(TemplateVM);
        }
        public IActionResult ExtendTemplates(string? query)
        {
            IEnumerable<Template> templates = _unit.Template.GetAll(u => u.Visibility == 0, "Questions").OrderByDescending(u => u.Point);
            if (!string.IsNullOrEmpty(query))
            {
                templates = templates.Where(u => u.Title.ToUpper().Contains(query.ToUpper()) || u.Description.ToUpper().Contains(query.ToUpper()));
            }            
            return View(templates);
        }
        
        [HttpPost]
        public IActionResult CultureManagement(string culture, string returnUrl)
        {
            Response.Cookies.Append(CookieRequestCultureProvider.DefaultCookieName, CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions { Expires = DateTimeOffset.Now.AddDays(30) });
            return LocalRedirect(returnUrl);
        }
        public IActionResult ExtendTemplate()
        {
            IEnumerable<Template> templates = _unit.Template.GetAll(u => u.Visibility == 0, "Questions");
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}