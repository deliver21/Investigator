using AutoMapper;
using Investigator.Models;
using Investigator.Models.DTOs;
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
        public IActionResult Index(string ? status)
        {
            LocalizeFormTable();
            return View();
        }
        private void LocalizeFormTable()
        {
            ViewBag.ManageQuestions = _localizer["Managequestions"];
            ViewBag.EditFormHeader = _localizer["Editformheader"];
            ViewBag.DeleteForm = _localizer["Deleteform"];
        }
        [Authorize]
        [IsBlockedAuthorize]
        public async Task<IActionResult> FillForm(int formId)
        {
            var form = await _unit.Form.Get(u => u.FormId == formId);
            form.Template = await _unit.Template.Get(u => u.TemplateId == form.TemplateId);
            form.Questions = _unit.Question.GetAll(u => u.FormId == formId).ToList();
            foreach(var question in form.Questions)
            {
                if(question.Type == SD.checkBoxType)
                {
                    question.Options = _unit.QuestionOption.GetAll(u => u.QuestionId == question.QuestionId).ToList();
                }
            }
            if (form == null) return Redirect("/Customer/Home/Index");
            return View(form);
        }

        [Authorize]
        [IsBlockedAuthorize]
        public async Task<IActionResult> Generate(int? templateId, FormDto? formData)
        {
            formData = JsonConvert.DeserializeObject<FormDto>(Convert.ToString(TempData["formData"]));
            if (formData != null && formData.Template != null) return View(formData);
            var templateForm = await _unit.Template.Get(u => u.TemplateId == templateId);
            if (templateForm == null)
            {
                return Redirect($"/Customer/Home/Index");
            }
            templateForm.Questions = _unit.TemplateQuestion.GetAll(u => u.TemplateId == templateForm.TemplateId, null, true).ToList();
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            Form form = new()
            {
                Questions = new List<Question>(),
                TemplateId = templateForm.TemplateId,
                Template = templateForm,
                Title = templateForm.Title,
                Description = templateForm.Description,
                CreatorId = userId,
            };
            foreach (var question in templateForm.Questions)
            {
                var questionCopy = new Question()
                {
                    QuestionId = 0,
                    FormId = 0,
                    Text= question.Text,
                    Type = question.Type,
                    Order = question.Order,
                    IsOptional = question.IsOptional,                    
                };
                form.Questions.Add(questionCopy);
            }
            var formDto = _mapper.Map<FormDto>(form);
            return View(formDto);
        }
        #region API's Calls

        [HttpGet]
        [Authorize]
        [IsBlockedAuthorize]
        public async Task <IActionResult> GetAll(string status)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            List<Form> forms = new List<Form>();

            if (status == "allForm" && User.IsInRole(SD.AdminRole))
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
        public async Task<IActionResult> SaveForm([FromForm] FormDto form)
        {
            TempData["baseUrl"] = SD.AppBaseUrl;
            form.Description =  _unit.Template.Get(u => u.TemplateId == form.TemplateId).GetAwaiter().GetResult().Description;
            if (form == null) return BadRequest("Invalid form data.");

            if (form.TemplateId != 0)
            {
                await _unit.Form.Add(_mapper.Map<Form>(form));
                var template = await _unit.Template.Get(u => u.TemplateId == form.TemplateId);
                if (template != null)
                {
                    template.Point += 1;
                    _unit.Template.Update(template);
                }
                _unit.Save();
            }

            var createdFormId = _unit.Form.Get(u => u.CreatedDate == form.CreatedDate).Id;

            foreach (var questionDto in form.Questions)
            {
                if (questionDto.QuestionId == 0)
                {
                    questionDto.FormId = createdFormId;
                   await _unit.Question.Add(_mapper.Map<Question>(questionDto));
                }
                else
                {
                    _unit.Question.Update(_mapper.Map<Question>(questionDto));
                }
                _unit.Save();

                // Save Options
                if(questionDto.Type == SD.checkBoxType && questionDto.Options.Any())
                {
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
                                int questionId = _unit.Question.Get(u => u.Text == questionDto.Text && u.Order == questionDto.Order).
                                    GetAwaiter().GetResult().QuestionId;
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
                                await _unit.QuestionOption.Add(_mapper.Map<QuestionOption>(option));
                            }
                            else
                            {
                                _unit.QuestionOption.Update(_mapper.Map<QuestionOption>(option));
                            }
                        }
                        _unit.Save();
                    }
                }
                
            }

            _unit.Save();
            return Ok(new { message = "Form saved successfully." });
        }

        [Authorize]
        [IsBlockedAuthorize]
        [HttpDelete]
        public async Task<IActionResult> Delete(int? id)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            Form formToDelete = await _unit.Form.Get(u => u.FormId == id);
            if (formToDelete == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }
            if(formToDelete.CreatorId != userId || !User.IsInRole(SD.AdminRole))
            {
                return Json(new { success = false, message = "Error while deleting" });
            }
                _unit.Form.Remove(formToDelete);
                _unit.Save();
            return Json(new { success = true, message = "Deletion successfully performed" });
        }
    }
    #endregion
}
