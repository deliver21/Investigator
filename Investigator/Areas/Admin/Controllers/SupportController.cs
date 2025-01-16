using AutoMapper;
using Investigator.Models;
using Investigator.Models.DTOs;
using Investigator.Repository.IRepository;
using Investigator.Services;
using Investigator.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Investigator.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SupportController : Controller
    {
        private readonly IJiraService _jiraService;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IUnitOfWork _unit;
        private readonly IMapper _mapper;
        public SupportController(IJiraService jiraService, UserManager<IdentityUser> userManager, IUnitOfWork unit, IMapper mapper)
        {
            _jiraService = jiraService;
            _userManager = userManager;
            _unit = unit;
            _mapper = mapper;
        }
        public IActionResult Index()
        {
            return View();
        }

        [Authorize]
        [IsBlockedAuthorize]
        [HttpGet]
        public IActionResult CreateTicket(string link)
        {
            return View(new JiraTicketDto { PageLink = link });
        }

        [Authorize]
        [IsBlockedAuthorize]
        [HttpPost]
        public async Task<IActionResult> CreateTicket(JiraTicketDto modelDto)
        {
            if (!ModelState.IsValid)
                return View(modelDto);

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Unauthorized();

            string? templateTitle = Convert.ToString(TempData["TemplateTitle"]);
            var model = _mapper.Map<JiraTicket>(modelDto);
            var ticketUrl = await _jiraService.CreateTicketAsync(model, user.Email, templateTitle);
            if(!string.IsNullOrEmpty(ticketUrl))
            {
                ViewBag.TicketUrl = ticketUrl;
                TempData["success"] = "A new ticket is successfully created";

                return View("TicketCreated");
            }
            TempData["error"] = "An error occured while creating the ticket in jira.";
            return View();
        }
        #region API's Calls
        [Authorize]
        [IsBlockedAuthorize]
        [HttpGet]
        public IActionResult GetAll()
        {
            IEnumerable<JiraTicket> tickets = new List<JiraTicket>();
            var claims = (ClaimsIdentity) User.Identity;
            if(claims.Claims.Any())
            {
                var userId = claims.FindFirst(ClaimTypes.NameIdentifier).Value;
                tickets = _unit.JiraTicket.GetAll(u => u.CreatedByUserId == userId).ToList();
            }
            return Json(new { data = tickets.OrderByDescending(u => u.CreatedDate)});
        }
        #endregion
    }
}
