using AutoMapper;
using Investigator.Models;
using Investigator.Models.DTO;
using Investigator.Repository.IRepository;
using Investigator.Services;
using Investigator.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Security.Claims;

namespace Investigator.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class FormController : Controller
    {
        private readonly IUnitOfWork _unit;
        private readonly IMapper _mapper;
        public FormController(IUnitOfWork unit, IMapper mapper)
        {
            _unit = unit;
            _mapper = mapper;
        }
        [Authorize]
        [IsBlockedAuthorize]
        public IActionResult Generate(int ? templateId, FormDto ? formData)
        {
            formData = JsonConvert.DeserializeObject<FormDto>(Convert.ToString(TempData["formData"]));
            if (formData != null && formData.Template != null) return View(formData);
            var templateForm = _unit.Template.Get(u => u.TemplateId == templateId);
            if(templateForm == null)
            {
                return Redirect($"/Customer/Home/Index");
            }
            templateForm.Questions = _unit.Question.GetAll(u => u.TemplateId == templateForm.TemplateId, null, true).ToList();
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            Form form = new()
            {
                Questions = templateForm.Questions,
                TemplateId = templateForm.TemplateId,
                Template = templateForm,
                Title = templateForm.Title,
                Description = templateForm.Description,
                CreatorId= userId,
            };
            foreach(var question in templateForm.Questions )
            {
                question.QuestionId = 0;
            }
            var formDto = _mapper.Map<FormDto>(form);
            return View(formDto);
        }
        [Authorize]
        [IsBlockedAuthorize]        
        public IActionResult EditHeader(string ? formData)
        {
            if(string.IsNullOrEmpty(formData)) Redirect($"/Customer/Home/Index"); 
            var formDataDto = JsonConvert.DeserializeObject<FormDto>(formData);
            return View(formDataDto);
        }

        [HttpPost]
        public IActionResult EditHeader(FormDto ? formData)
        {
            if (formData == null) Redirect($"/Customer/Home/Index");
            var questions = JsonConvert.DeserializeObject<List<QuestionDto>>(Convert.ToString(TempData["questiontemp"]));
            var template = JsonConvert.DeserializeObject<Template>(Convert.ToString(TempData["templatetemp"]));
            if (questions.Any()) formData.Questions = questions;
            if (template != null) formData.Template = template;
            TempData["formData"] = JsonConvert.SerializeObject(formData);
            return RedirectToAction(nameof(Generate));
        }
        #region API's Calls
        [Authorize]
        [IsBlockedAuthorize]
        [HttpPost("save")]

        public async Task<IActionResult> SaveForm([FromBody] FormDto form)
        {
            if (form == null) return BadRequest("Invalid form data.");
            // Save Form
            var newForm = _mapper.Map<Form>(form);

            _unit.Form.Add(newForm);
            _unit.Save();

            // Save Questions
            foreach (var questionDto in form.Questions)
            {
                var newQuestion = _mapper.Map<Question>(questionDto);

                _unit.Question.Add(newQuestion);
                _unit.Save();

                // Save Options
                foreach (var optionDto in questionDto.Options)
                {
                    var newOption = new QuestionOption
                    {
                        Text = optionDto.Text,
                        QuestionId = newQuestion.QuestionId
                    };
                    _unit.QuestionOption.Add(newOption);
                }
            }

            _unit.Save();
            return Ok(new { message = "Form saved successfully." });
        }
    }
    #endregion
}
