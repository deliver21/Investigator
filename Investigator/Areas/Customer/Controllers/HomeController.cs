﻿using Investigator.Models;
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
            IndexCulture();
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            string ? userId = null;
            if (claimsIdentity.Claims.Any())
            {
                userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            }
            TemplateVM = new()
            {
                Template = _unit.Template.GetAll(u => u.Visibility == 0, "Questions").Take(4),
                Form = userId != null ? _unit.Form.GetAll(u => u.CreatorId == userId) : new List<Form>()
            };
            return View(TemplateVM);
        }
        public IActionResult ExtendTemplates()
        {
            IEnumerable<Template> templates = _unit.Template.GetAll(u => u.Visibility == 0, "Questions");
            return View(templates);
        }
        private void IndexCulture()
        {
            ViewData["WelcomeTo"] = _localizer["WelcomeTo"];
            ViewData["ASoftwareToQuestion"] = _localizer["ASoftwareToQuestion"];
            ViewData["UsersOnTheInternet"] = _localizer["UsersOnTheInternet"];
            ViewData["Signin"] = _localizer["Signin"];
            ViewData["CreateTemplate"] = _localizer["CreateTemplate"];
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