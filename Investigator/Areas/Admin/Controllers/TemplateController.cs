using AutoMapper;
using Investigator.Models;
using Investigator.Models.DTOs;
using Investigator.Models.ViewModels;
using Investigator.Repository.IRepository;
using Investigator.Services;
using Investigator.Services.IServices;
using Investigator.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;

namespace Investigator.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class TemplateController : Controller
    {
        private readonly IHtmlLocalizer<TemplateController> _localizer;
        private readonly IUnitOfWork _unit;
        private readonly IFileSaver _fileSaver;
        private IMapper _mapper;
        [BindProperty]
        public TemplateDetailsVM TemplateDetails { get; set; }
        public TemplateController(IUnitOfWork unit, IFileSaver fileSaver, IMapper mapper, IHtmlLocalizer<TemplateController> localizer) 
        {
            _unit = unit;
            _fileSaver = fileSaver;
            _mapper = mapper;
            _localizer = localizer;
        }
        [Authorize]
        [IsBlockedAuthorize]
        public IActionResult Index(string ? status)
        {
            LocalizeTemplateTable();
            return View();
        }
        private void LocalizeTemplateTable()
        {
            ViewBag.ManageQuestions = _localizer["Managequestions"];
            ViewBag.EditTemplateHeader = _localizer["Edittemplateheader"];
            ViewBag.DeleteTemplate = _localizer["Deletetemplate"];
        }
        [Authorize]
        [IsBlockedAuthorize]
        public async Task<IActionResult> Upsert(int ? id)
        {
            var topicList = new List<SelectListItem>()
            { new SelectListItem{Text = SD.EducationTopic, Value = SD.EducationTopic},
              new SelectListItem{Text = _localizer["Personal"].Value, Value = SD.PersonalTopic},
              new SelectListItem{Text = _localizer["Professional"].Value, Value = SD.ProfessionalTopic}
            };
            ViewBag.TopicList = topicList;
            Template template = new();
            if (id != 0)
            {
                template = await _unit.Template.Get(u => u.TemplateId == id);
            }
            return View(template);
        }
        [Authorize]
        [IsBlockedAuthorize]
        [HttpPost]
        public async Task<IActionResult> Upsert(Template template, IFormFile ? file)
        {
            if (template.TemplateId == 0)
            {
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                template.CreatorId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
                template.CreatedDate = DateTime.Now;
                template.ImageId = file != null ? _fileSaver.UploadFilesToGoogleDrive(file) : "";
                await _unit.Template.Add(template);
                TempData["success"] = "Template has successfully been created";
            }
            else
            {
                var previousPicture = _unit.Template.Get(u => u.TemplateId == template.TemplateId).GetAwaiter().GetResult().ImageId;
                if (file != null)
                {
                    bool response = false;
                    
                    if (!string.IsNullOrEmpty(previousPicture))
                    {
                        response = await _fileSaver.DeleteFileFromGoogleDrive(previousPicture);
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
        public async Task<IActionResult> ManageQuestions(int id)
        {
            var template = await _unit.Template.Get(u => u.TemplateId == id, includeProperties : "Questions");
            template.Questions = new List<Question>();
            template.Questions = _unit.Question.GetAll(u => u.TemplateId == template.TemplateId).ToList();
            TempData["baseUrl"] = SD.AppBaseUrl;
            return View(template);
        }

        public async Task<IActionResult> Details(int? templateId)
        {
            var template = await _unit.Template.Get(u => u.TemplateId == templateId);            
            if(template == null)
            {
                return Redirect($"/Customer/Home/Index");
            }
            
            template.Creator = await _unit.ApplicationUser.Get(u => u.Id == template.CreatorId);
            template.Questions = _unit.Question.GetAll(u => u.TemplateId == templateId, "QuestionOption").ToList();
            TemplateDetails = new()
            {
                Template = template,
                LikesCount = _unit.Like.GetAll(u => u.TemplateId == templateId).Count(),
                CommentsCount = _unit.Comment.GetAll(u => u.TemplateId == templateId).Count(),
                Likes = _unit.Like.GetAll(u => u.TemplateId == templateId),
                Comments = _unit.Comment.GetAll(u => u.TemplateId == templateId)
            };

            var claims = (ClaimsIdentity)User.Identity;
            if (claims.Claims.Any())
            {
                var userId = claims.FindFirst(ClaimTypes.NameIdentifier).Value;
            }
            return View(TemplateDetails);
        }

        [Authorize]
        [IsBlockedAuthorize]
        public async Task<IActionResult> LikeTemplate(int templateId)
        {
            var template = await _unit.Template.Get(u => u.TemplateId == templateId);
            if(template == null)
            {
                return Redirect($"/Customer/Home/Index");
            }
            var claims = (ClaimsIdentity) User.Identity;

            if(claims.Claims.Any())
            {
                var userId = claims.FindFirst(ClaimTypes.NameIdentifier).Value;
                var likeToDelete = await _unit.Like.Get(u => u.LikerId == userId);
                if (likeToDelete == null)
                {
                    Like like = new()
                    {
                        TemplateId = templateId,
                        LikerId = userId
                    };
                    await _unit.Like.Add(like);
                }
                else
                {
                    _unit.Like.Remove(likeToDelete);
                }
                _unit.Save();
            }
            int? passId = templateId;
            return RedirectToAction(nameof(Details), passId);
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
        public async Task <IActionResult> Delete(int ? id)
        {
            Template templateToDelete = await _unit.Template.Get(u => u.TemplateId == id);
            if(templateToDelete == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }
            else
            {
                _unit.Template.Remove(templateToDelete);
                if(!string.IsNullOrEmpty(templateToDelete.ImageId))
                {
                    await _fileSaver.DeleteFileFromGoogleDrive(templateToDelete.ImageId);
                }
                _unit.Save();
            }
            return Json(new { success = true, message = "Delete successfully performed" });
        }

        [HttpPost("SaveQuestions/{templateId:int}")]
        public async Task<IActionResult> SaveQuestions(int templateId, [FromBody] IEnumerable<QuestionDto> questions)
        {
            if (_unit.Template.Get(t => t.TemplateId == templateId).GetAwaiter().GetResult().TemplateId == 0)
                return NotFound(new { message = "Template not found." });
            try
            {
                foreach (var question in questions)
                {
                    if (question.QuestionId == 0)
                    {
                        var questionToSave = _mapper.Map<Question>(question);
                        questionToSave.TemplateId = templateId;
                        await _unit.Question.Add(questionToSave);
                    }
                    else
                    {
                        if (await _unit.Question.Get(u => u.QuestionId == question.QuestionId) == null) continue;
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
        public async Task<IActionResult> DeleteQuestion(int ? questionId)
        {
            if (questionId == null)
            {
                return NotFound(new { message = "Question not found." });
            }
            var question = await _unit.Question.Get(u => u.QuestionId == questionId);
            if (question == null)
            {
                return NotFound(new { message = "Question not found." });
            }
            _unit.Question.Remove(question);
            _unit.Save();
            return Ok("Question is successfully deleted");
        }
        #endregion
    }
}
