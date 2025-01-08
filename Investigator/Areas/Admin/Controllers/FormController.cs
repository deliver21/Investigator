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
using System.Collections.Generic;
using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc.Localization;

namespace Investigator.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class FormController : Controller
    {
        private readonly IHtmlLocalizer<FormController> _localizer;
        private readonly IUnitOfWork _unit;
        private readonly IMapper _mapper;
        public FormController(IUnitOfWork unit, IMapper mapper, IHtmlLocalizer<FormController> localizer)
        {
            _unit = unit;
            _mapper = mapper;
            _localizer = localizer;
        }

        [Authorize]
        [IsBlockedAuthorize]
        public IActionResult Index(string? status)
        {
            LocalizeFormTable();
            return View();
        }
        private void LocalizeFormTable()
        {
            ViewBag.ManageQuestions = _localizer["Managequestions"];
            ViewBag.EditFormHeader = _localizer["Editformheader"];
            ViewBag.DeleteTemplate = _localizer["Deleteform"];
        }

        [Authorize]
        [IsBlockedAuthorize]
        public IActionResult Generate(int? templateId, FormDto? formData)
        {
            formData = JsonConvert.DeserializeObject<FormDto>(Convert.ToString(TempData["formData"]));
            if (formData != null && formData.Template != null) return View(formData);
            var templateForm = _unit.Template.Get(u => u.TemplateId == templateId);
            if (templateForm == null)
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
                CreatorId = userId,
            };
            foreach (var question in templateForm.Questions)
            {
                question.QuestionId = 0;
            }
            var formDto = _mapper.Map<FormDto>(form);
            return View(formDto);
        }
        [Authorize]
        [IsBlockedAuthorize]
        public IActionResult EditHeader(string? formData)
        {
            if (string.IsNullOrEmpty(formData)) Redirect($"/Customer/Home/Index");
            var formDataDto = JsonConvert.DeserializeObject<FormDto>(formData);
            return View(formDataDto);
        }

        [HttpPost]
        public IActionResult EditHeader(FormDto? formData)
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
        [HttpGet]
        public IActionResult GetAll(string status)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            List<Form> forms = new List<Form>();

            if (status == "allForms" && User.IsInRole(SD.AdminRole))
            {
                forms = _unit.Form.GetAll().ToList();
            }
            else
            {
                forms = _unit.Form.GetAll(u => u.CreatorId == userId, null).ToList();
            }
            return Json(new { data = forms });
        }

        [Authorize]
        [IsBlockedAuthorize]
        [HttpPost("save")]
        public IActionResult SaveForm([FromForm] FormDto form)
        {
            form.Description = _unit.Template.Get(u => u.TemplateId == form.TemplateId).Description;
            if (form == null) return BadRequest("Invalid form data.");

            var newForm = _mapper.Map<Form>(form);
            if (form.TemplateId != 0)
            {
                _unit.Form.Add(newForm);
                var template = _unit.Template.Get(u => u.TemplateId == form.TemplateId);
                if (template != null)
                {
                    template.Point += 1;
                    _unit.Template.Update(template);
                }
                _unit.Save();
            }

            foreach (var questionDto in form.Questions)
            {
                var newQuestion = _mapper.Map<Question>(questionDto);
                if (newQuestion.QuestionId == 0)
                {
                    _unit.Question.Add(newQuestion);
                }
                else
                {
                    _unit.Question.Update(newQuestion);
                }
                _unit.Save();

                // Save Options
                foreach (var option in questionDto.Options)
                {
                    if (option.QuestionId == 0)
                    {
                        if (questionDto.QuestionId != 0)
                        {
                            option.QuestionId = questionDto.QuestionId;
                        }
                        else
                        {
                            int questionId = _unit.Question.Get(u => u.Text == questionDto.Text && u.Order == questionDto.Order).QuestionId;
                            if (questionId > 0)
                            {
                                option.QuestionId = questionId;
                            }
                        }
                    }
                    if (option.QuestionId != 0)
                    {
                        if (option.OptionId == 0)
                        {
                            _unit.QuestionOption.Add(option);
                        }
                        else
                        {
                            _unit.QuestionOption.Update(option);
                        }
                    }
                    _unit.Save();
                }
            }

            _unit.Save();
            return Ok(new { message = "Form saved successfully." });
        }

        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            Form formToDelete = _unit.Form.Get(u => u.FormId == id);
            if (formToDelete == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }
            else
            {
                _unit.Form.Remove(formToDelete);
                _unit.Save();
            }
            return Json(new { success = true, message = "Delete successfully performed" });
        }
    }
    #endregion
}
