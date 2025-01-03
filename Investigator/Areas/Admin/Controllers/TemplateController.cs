﻿using AutoMapper;
using Investigator.Models;
using Investigator.Models.DTO;
using Investigator.Repository.IRepository;
using Investigator.Services;
using Investigator.Services.IServices;
using Investigator.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;

namespace Investigator.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class TemplateController : Controller
    {
        private readonly IUnitOfWork _unit;
        private readonly IFileSaver _fileSaver;
        private IMapper _mapper;       
        public TemplateController(IUnitOfWork unit, IFileSaver fileSaver, IMapper mapper) 
        {
            _unit = unit;
            _fileSaver = fileSaver;
            _mapper = mapper;
        }
        [Authorize]
        [IsBlockedAuthorize]
        public IActionResult Index(string ? status)
        {   
            return View();
        }
        [Authorize]
        [IsBlockedAuthorize]
        public IActionResult Upsert(int ? id)
        {
            var topicList = new List<SelectListItem>()
            { new SelectListItem{Text=SD.EducationTopic,Value = SD.EducationTopic},
              new SelectListItem{Text=SD.PersonalTopic,Value = SD.PersonalTopic},
              new SelectListItem{Text=SD.ProfessionalTopic,Value = SD.ProfessionalTopic}
            };
            ViewBag.TopicList = topicList;
            Template template = new();
            if (id != 0)
            {
                template = _unit.Template.Get(u => u.TemplateId == id);
            }
            return View(template);
        }
        [Authorize]
        [HttpPost]
        public IActionResult Upsert(Template template, IFormFile ? file)
        {
            if (template.TemplateId == 0)
            {
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                template.CreatorId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
                template.CreatedDate = DateTime.Now;
                template.ImageId = file != null ? _fileSaver.UploadFilesToGoogleDrive(file) : "";
                _unit.Template.Add(template);
                TempData["success"] = "Template has successfully been created";
            }
            else
            {
                var previousPicture = _unit.Template.Get(u => u.TemplateId == template.TemplateId).ImageId;
                if (file != null)
                {
                    bool response = false;
                    
                    if (!string.IsNullOrEmpty(previousPicture))
                    {
                        response = _fileSaver.DeleteFileFromGoogleDrive(previousPicture).GetAwaiter().GetResult();
                    }
                    template.ImageId = response ? _fileSaver.UploadFilesToGoogleDrive(file) : "";
                }
                else
                {
                    template.ImageId = !string.IsNullOrEmpty(previousPicture) ? previousPicture : "";
                }
                template.ModifiedDate = DateTime.Now;
                _unit.Template.Update(template);
                TempData["success"] = "Template has successfully been updated";
            }
            _unit.Save();            
            return RedirectToAction("Index");
        }
        public IActionResult ManageQuestions(int id)
        {
            var template = _unit.Template.Get(u => u.TemplateId == id, includeProperties : "Questions");
            template.Questions = new List<Question>();
            template.Questions = _unit.Question.GetAll(u => u.TemplateId == template.TemplateId).ToList();
            return View(template);
        }

        #region Api's Calls
        
        [Authorize]
        [IsBlockedAuthorize]
        [HttpGet]
        public IActionResult GetAll(string status)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            List<Template> templates = new List<Template>();

            if(status == "allTemplate" && User.IsInRole(SD.AdminRole))
            {
                templates = _unit.Template.GetAll().ToList();
            }
            else
            {
                templates = _unit.Template.GetAll(u => u.CreatorId == userId, null).ToList();
            }            
            return Json(new { data = templates });
        }

        [HttpDelete]
        public IActionResult Delete(int ? id)
        {
            Template templateToDelete = _unit.Template.Get(u => u.TemplateId == id);
            if(templateToDelete == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }
            else
            {
                _unit.Template.Remove(templateToDelete);
                if(!string.IsNullOrEmpty(templateToDelete.ImageId))
                {
                    _fileSaver.DeleteFileFromGoogleDrive(templateToDelete.ImageId);
                }
                _unit.Save();
            }
            return Json(new { success = true, message = "Delete successfully performed" });
        }

        [HttpPost("SaveQuestions/{templateId:int}")]
        public IActionResult SaveQuestions(int templateId, [FromBody] IEnumerable<QuestionDto> questions)
        {
            if (_unit.Template.Get(t => t.TemplateId == templateId).TemplateId == 0)
                return NotFound(new { message = "Template not found." });
            try
            {
                foreach (var question in questions)
                {
                    if (question.QuestionId == 0)
                    {
                        var questionToSave = _mapper.Map<Question>(question);
                        questionToSave.TemplateId = templateId;
                        _unit.Question.Add(questionToSave);
                    }
                    else
                    {
                        if (_unit.Question.Get(u => u.QuestionId == question.QuestionId) == null) continue;
                        var questionToSave = _mapper.Map<Question>(question);
                        questionToSave.TemplateId = templateId;
                        _unit.Question.Update(questionToSave); ;
                    }
                }
                _unit.Save();
                return Ok(new { message = "Questions saved successfully!" });
            }
            catch
            {
                return NotFound();
            }
            
        }
        [HttpDelete("DeleteQuestion/{questionId:int}")]
        public IActionResult DeleteQuestion(int ? questionId)
        {
            if(questionId == null) return NotFound(new {message = "Question not found."});
            var question = _unit.Question.Get(u => u.QuestionId == questionId);
            if (question == null) return NotFound(new { message = "Question not found." });
            _unit.Question.Remove(question);
            _unit.Save();
            return Ok("Question is successfully deleted");
        }
        #endregion
    }
}
