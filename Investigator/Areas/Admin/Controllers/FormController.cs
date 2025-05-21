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
using Investigator.Services.IServices;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Investigator.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin/[controller]/[action]")]
    public class FormController : Controller
    {
        private readonly IHtmlLocalizer<FormController> _localizer;
        private readonly IUnitOfWork _unit;
        private readonly IMapper _mapper;
        private readonly IFileSaver _fileSaver;
        public FormController(IUnitOfWork unit, IMapper mapper, IHtmlLocalizer<FormController> localizer, IFileSaver fileSaver)
        {
            _unit = unit;
            _mapper = mapper;
            _localizer = localizer;
            _fileSaver = fileSaver;
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
            ViewBag.FormAnswers = _localizer["Formanswers"];
        }

        [Authorize]
        [IsBlockedAuthorize]
        public async Task<IActionResult> Upsert(int? id)
        {
            Form form = new();
                form = await _unit.Form.Get(u => u.FormId == id);
            if (form == null) return RedirectToAction(nameof(Index));
            if(string.IsNullOrEmpty(form.ImageId))
            {
                form.ImageId = _unit.Template.Get(u => u.TemplateId == form.TemplateId).GetAwaiter().GetResult().ImageId;
            }
            
            return View(form);
        }
        [Authorize]
        [IsBlockedAuthorize]
        [HttpPost]
        public async Task<IActionResult> Upsert(Form form, IFormFile? file)
        {
            if (form.FormId == 0)
            {
                TempData["success"] = "Form has successfully been created";
                return RedirectToAction(nameof(Index));
            }
            else
            {
                var previousPicture = _unit.Form.Get(u => u.FormId == form.FormId,null,false).GetAwaiter().GetResult().ImageId;
                if (file != null)
                {
                    bool response = false;
                    var templateWithImage = _unit.Template.GetAll(u => u.ImageId == previousPicture).Count();
                    if (templateWithImage == 0 && !string.IsNullOrEmpty(previousPicture))
                    {
                        await _fileSaver.DeleteFileFromGoogleDrive(previousPicture);
                    }
                    form.ImageId = _fileSaver.UploadFilesToGoogleDrive(file);
                }
                else
                {
                    form.ImageId = !string.IsNullOrEmpty(previousPicture) ? previousPicture : "";
                }
                form.ModifiedDate = DateTime.Now;
                _unit.Form.Update(form);
                TempData["success"] = "Form has successfully been updated";
            }
            _unit.Save();
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> ManageQuestions(int? id)
        {
            var form = await _unit.Form.Get(u => u.TemplateId == id, includeProperties: "Questions");
            if(form == null || form.FormId == 0) 
            {
                TempData["success"] = "Form not found";
                return RedirectToAction("Index");
            }
            form.Questions = new List<Question>();
            form.Questions = _unit.Question.GetAll(u => u.FormId == form.FormId).ToList();
            TempData["baseUrl"] = SD.AppBaseUrl;
            return View(form);
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
                var questionToCopy = new Question()
                {
                    QuestionId = 0,
                    FormId = 0,
                    Text = question.Text,
                    Type = question.Type,
                    Order = question.Order,
                    IsOptional = question.IsOptional,
                };
                form.Questions.Add(questionToCopy);
            }
            var formDto = _mapper.Map<FormDto>(form);
            return View(formDto);
        }

        public async Task<IActionResult> ManageSubmissions(int? id)
        {
            Form form = new();
            form = await _unit.Form.Get(u => u.FormId == id);
            if (form == null) return RedirectToAction(nameof(Index));
            IEnumerable<Question> questions = _unit.Question.GetAll(u => u.FormId == form.FormId).OrderBy(u => u.QuestionId).ToList();
            return View(questions);
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

        [HttpPost]
        public async Task<IActionResult> SubmitForm([FromForm] FormSubmissionDto submission)
        {
            if (submission == null || submission.ParsedAnswers == null)
                return BadRequest("Invalid submission");            

            var responses = new List<Response>();
            var userName = User.Identity?.Name ?? submission.Filler; // fallback if user not logged in

            if (await _unit.FormFiller.Get(u => u.Filler == userName && u.FormId == submission.FormId) != null)
            {
                return Unauthorized("You have already submitted to this Form before :)");
            }

            try
            {
                foreach (var answer in submission.ParsedAnswers)
                {
                    var response = new Response
                    {
                        FormId = submission.FormId,
                        QuestionId = answer.QuestionId,
                        Filler = userName,
                    };

                    if (_unit.Question.Get(u => u.QuestionId == answer.QuestionId).GetAwaiter().GetResult().Type != SD.file)
                    {
                        response.Answer = answer.Answer;
                    }
                    else
                    {
                        foreach (var file in submission.Files)
                        {
                            var fileId = _fileSaver.UploadFilesToGoogleDrive(file);
                            response.Answer += !String.IsNullOrEmpty(fileId) ? $"https://drive.google.com/thumbnail?id={fileId}\n" : "";
                        }
                    }
                    responses.Add(response);
                }

                // Handle files separately
                foreach (var file in submission.Files)
                {
                    var questionIdStr = file.Name.Replace("files_", ""); // assuming field name is files_123
                    if (int.TryParse(questionIdStr, out var questionId))
                    {
                        var fileId = _fileSaver.UploadFilesToGoogleDrive(file);
                    }
                }

                // Save to DB
                foreach (var response in responses)
                {
                    await _unit.Response.Add(response);
                }
                _unit.Save();
                FormFiller filler = new()
                {
                    Filler = userName,
                    FormId = submission.FormId,
                    SubmissionDate = DateTime.Now,
                };
                await _unit.FormFiller.Add(filler);
                _unit.Save();
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }            

            return Ok(new { message = "Form submitted successfully" });
        }


        [HttpGet]
        [Authorize]
        [IsBlockedAuthorize]
        public async Task<IActionResult> GetSubmissions(int ? formId)
        {
            Form form = new();
            form = await _unit.Form.Get(u => u.FormId == formId);
            if (form == null) return Json(new { success = false, message = "Error while retrieving data" });
            List<FormFiller> fillers = new List<FormFiller>();
            fillers = _unit.FormFiller.GetAll(u => u.FormId == form.FormId).ToList();
            for(int i = 0; i < fillers.Count; i++)
            {
                fillers[i].Responses = _unit.Response.GetAll(u => u.Filler == fillers[i].Filler).OrderBy(u => u.QuestionId).ToList();
            }
            return Json(new {data = fillers});
        }
        [HttpPost]
        public async Task<IActionResult> SaveForm([FromForm] FormDto form)
        {
            TempData["baseUrl"] = SD.AppBaseUrl;
            form.Description = _unit.Template.Get(u => u.TemplateId == form.TemplateId).GetAwaiter().GetResult().Description;
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
            var templateWithImage = _unit.Template.GetAll(u => u.ImageId == formToDelete.ImageId).Count();
            if(templateWithImage == 0 && !string.IsNullOrEmpty(formToDelete.ImageId))
            {
                await _fileSaver.DeleteFileFromGoogleDrive(formToDelete.ImageId);
            }
                _unit.Save();

            return Json(new { success = true, message = "Deletion successfully performed" });
        }


        [HttpPost("SaveQuestions/{formId:int}")]
        public async Task<IActionResult> SaveQuestions(int formId, [FromBody] IEnumerable<QuestionDto> questions)
        {
            if (_unit.Form.Get(t => t.FormId == formId).GetAwaiter().GetResult().FormId == 0)
                return NotFound(new { message = "Form not found." });
            try
            {
                foreach (var question in questions)
                {
                    if (question.QuestionId == 0)
                    {
                        var questionToSave = new Question();
                        questionToSave.Type = question.Type;
                        questionToSave.Text = question.Text;
                        questionToSave.Order = question.Order;
                        questionToSave.IsOptional = question.IsOptional;
                        questionToSave.FormId = formId;

                        await _unit.Question.Add(questionToSave);
                    }
                    else
                    {
                        if (_unit.Question.Get(u => u.QuestionId == question.QuestionId).GetAwaiter().GetResult() != null)
                        {
                            var questionToSave = new Question();
                            questionToSave.Type = question.Type;
                            questionToSave.Text = question.Text;
                            questionToSave.Order = question.Order;
                            questionToSave.IsOptional = question.IsOptional;
                            questionToSave.FormId = formId;

                            _unit.Question.Update(questionToSave);
                        }
                    }
                    _unit.Save();
                }

                return Ok(new { message = "Questions are successfully saved!" });
            }
            catch
            {
                return NotFound(new { message = "An error occurred while saving questions." });
            }

        }
        [HttpDelete("DeleteQuestion/{questionId:int}")]
        public async Task<IActionResult> DeleteQuestion(int? questionId)
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
    }    
    #endregion
}
