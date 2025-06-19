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
using Investigator.Models.ViewModels;
using System.Text;

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
        private readonly IHmacGenerator _hmacGenerator;
        private readonly IEmailSender _emailSender;
        
        [BindProperty]
        public FormFillerVM FormFillers { get; set; }
        public FormController(IUnitOfWork unit, IMapper mapper, IHtmlLocalizer<FormController> localizer,
            IFileSaver fileSaver, IHmacGenerator hmacGenerator, IEmailSender emailSender)
        {
            _unit = unit;
            _mapper = mapper;
            _localizer = localizer;
            _fileSaver = fileSaver;
            _hmacGenerator = hmacGenerator;
            _emailSender = emailSender;
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
                TempData["error"] = _localizer["TheFormHasNotBeenFound"].Value;
                return RedirectToAction(nameof(Index));
            }
            else
            {
                var previousPicture = _unit.Form.Get(u => u.FormId == form.FormId,null,false).GetAwaiter().GetResult().ImageId;
                if (file != null)
                {
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
                TempData["success"] = _localizer["FormHasSuccessfullyBeenUpdated"];
            }
            _unit.Save();
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> ManageQuestions(int? id)
        {
            var form = await _unit.Form.Get(u => u.FormId == id, includeProperties: "Questions");
            if (form == null || form.FormId == 0)
            {
                TempData["success"] = _localizer["TheFormHasNotBeenFound"].Value;
                return RedirectToAction("Index");
            }
            form.Questions = _unit.Question.GetAll(u => u.FormId == form.FormId).ToList() ?? new List<Question>();
            foreach(Question question in form.Questions)
            {
                if(question.Type == SD.checkBoxType || question.Type == SD.radioBoxType)
                {
                    question.Options = _unit.QuestionOption.GetAll(u => u.QuestionId == question.QuestionId).ToList();
                }
            }
            TempData["baseUrl"] = SD.AppBaseUrl;
            var formDto = _mapper.Map<FormDto>(form);
            return View(formDto);
        }

        [Authorize]
        [IsBlockedAuthorize]
        public async Task<IActionResult> FillForm(string idHashed)
        {
            var form = await _unit.Form.Get(u => u.IdHashed == idHashed);
            var formId = form.FormId;
            form.Template = await _unit.Template.Get(u => u.TemplateId == form.TemplateId);
            form.Questions = _unit.Question.GetAll(u => u.FormId == formId).ToList();
            foreach(var question in form.Questions)
            {
                if(question.Type == SD.checkBoxType || question.Type == SD.radioBoxType)
                {
                    question.Options = _unit.QuestionOption.GetAll(u => u.QuestionId == question.QuestionId).ToList();
                }
            }
            if (form == null) return Redirect("/Customer/Home/Index");
            return View(form);
        }

        [Authorize]
        [IsBlockedAuthorize]
        public async Task<IActionResult> Generate(int? templateId)
        {            
            var templateForm = await _unit.Template.Get(u => u.TemplateId == templateId);
            if (templateForm == null)
            {
                return Redirect($"/Customer/Home/Index");
            }
            TempData["baseUrl"] = SD.AppBaseUrl;
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

        [Authorize]
        [IsBlockedAuthorize]
        public async Task<IActionResult> GetSubmissions(string? idHashed)
        {
            FormFillers = new();
            FormFillers.Form =  await _unit.Form.Get(u => u.IdHashed == idHashed);
            var formId = FormFillers.Form.FormId;
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            if (FormFillers.Form == null || FormFillers.Form.CreatorId != userId)
            {
                TempData["error"] = _localizer["ErrorWhileRetrievingData"].Value;
                RedirectToAction(nameof(Index));
            }
            FormFillers.Form.Creator = await _unit.ApplicationUser.Get(u => u.Id == FormFillers.Form.CreatorId);
            if(string.IsNullOrEmpty(FormFillers.Form.ImageId))
            {
                FormFillers.Form.ImageId = _unit.Template.Get(u => u.TemplateId == FormFillers.Form.TemplateId).GetAwaiter().GetResult().ImageId ?? "";
            }

            FormFillers.Questions = _unit.Question.GetAll(u => u.FormId == formId).OrderBy(u => u.QuestionId);            

            List<FormFiller> fillers = new List<FormFiller>();
            fillers = _unit.FormFiller.GetAll(u => u.FormId == formId).ToList();
            for (int i = 0; i < fillers.Count; i++)
            {
                fillers[i].Responses = _unit.Response.GetAll(u => u.Filler == fillers[i].Filler && u.FormId == formId).OrderBy(u => u.QuestionId).ToList();
                fillers[i].ApplicationUser = await _unit.ApplicationUser.Get(u => u.Id == fillers[i].Filler) ?? new();
            }
            FormFillers.FormFillers = fillers ?? new List<FormFiller>();
            TempData["baseUrl"] = SD.AppBaseUrl;
            return View(FormFillers);
        }

        [Authorize]
        [IsBlockedAuthorize]
        public async Task<IActionResult> GetSubmissionList()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            IEnumerable<FormFiller> formFillers = _unit.FormFiller.GetAll(u => u.Filler == userId).OrderByDescending(u => u.FormId);
            List<Form> forms = new List<Form>();
            foreach (var formFiller in formFillers)
            {
                var form = await _unit.Form.Get(u => u.FormId == formFiller.FormId);
                if(string.IsNullOrEmpty(form.ImageId))
                {
                    form.ImageId = _unit.Template.Get(u => u.TemplateId == form.TemplateId).GetAwaiter().GetResult().ImageId;
                }
                forms.Add(form);
            }
            return View(forms);
        }
        [Authorize]
        [IsBlockedAuthorize]
        public async Task<IActionResult> GetMySubmission(string ? idHashed)
        {
            FillerVM filler = new();
            filler.Form = await _unit.Form.Get(u => u.IdHashed == idHashed);
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            filler.FormFiller = await _unit.FormFiller.Get(u => u.Filler == userId && u.FormId == filler.Form.FormId);

            if (filler.Form == null || filler.FormFiller == null)
            {
                TempData["error"] = _localizer["ErrorWhileRetrievingData"].Value;
                RedirectToAction(nameof(GetSubmissionList));
            }
            var formId = filler.Form.FormId;

            if (string.IsNullOrEmpty(filler.Form.ImageId))
            {
                filler.Form.ImageId = _unit.Template.Get(u => u.TemplateId == filler.Form.TemplateId).GetAwaiter().GetResult().ImageId ?? "";
            }

            filler.Questions = _unit.Question.GetAll(u => u.FormId == formId).OrderBy(u => u.QuestionId);
            filler.FormFiller = await _unit.FormFiller.Get(u => u.FormId == formId && u.Filler == userId);
            filler.FormFiller.ApplicationUser = await _unit.ApplicationUser.Get(u => u.Id == userId);
            filler.FormFiller.Responses = _unit.Response.GetAll(u => u.Filler == filler.FormFiller.Filler && u.FormId == formId).OrderBy(u => u.QuestionId).ToList();
                
            return View(filler);
        }

        [HttpGet]
        public async Task<IActionResult> SubmissionConfirmation(int? formId)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            FormFiller filler = await _unit.FormFiller.Get(u => u.Filler == userId && u.FormId == formId);
            filler.Form = await _unit.Form.Get(u => u.FormId == filler.FormId);
            if (filler == null || filler.Form == null)
            {
                TempData["error"] = _localizer["ErrorWhileRetrievingData"].Value;
                return NotFound(new { message = _localizer["ErrorWhileRetrievingData"].Value });
            }

            if (string.IsNullOrEmpty(filler.Form.ImageId))
            {
                var templateImageId = _unit.Template.Get(u => u.TemplateId == filler.Form.TemplateId).GetAwaiter().GetResult().ImageId;
                if (!string.IsNullOrEmpty(templateImageId))
                {
                    filler.Form.ImageId = templateImageId;
                }
            }
            return View(filler);
        }

        #region API's Calls

        [HttpGet]
        [Authorize]
        [IsBlockedAuthorize]
        public IActionResult GetAll(string status)
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
        public IActionResult SubmitForm([FromForm] FormSubmissionDto submission)
        {
            if (submission == null || submission.ParsedAnswers == null)
                return BadRequest("Invalid submission");

            var checkedForm = _unit.Form.Get(u => u.FormId == submission.FormId).GetAwaiter().GetResult();
            if (checkedForm == null || checkedForm.Status == SD.InactiveStatus)
                return NotFound("Form is deleted or is no longer active");

            var responses = new List<Response>();
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value; // fallback if user not logged in

            if (_unit.FormFiller.Get(u => u.Filler == userId && u.FormId == submission.FormId).GetAwaiter().GetResult() != null)
            {
                return Unauthorized(new { message = "You have already submitted to this Form before :)" });
            }

            try
            {
                foreach (var answer in submission.ParsedAnswers)
                {
                    var response = new Response
                    {
                        FormId = submission.FormId,
                        QuestionId = answer.QuestionId,
                        Filler = userId,
                    };

                    var questionTypeToCheck = _unit.Question.Get(u => u.QuestionId == answer.QuestionId).GetAwaiter().GetResult().Type;

                    
                    if(questionTypeToCheck == SD.checkBoxType || questionTypeToCheck == SD.radioBoxType)
                    {
                        foreach (var optionId in answer.Answer.Split(','))
                        {
                            var option = _unit.QuestionOption.Get(u => u.OptionId == int.Parse(optionId)).GetAwaiter().GetResult().Text;
                            response.Answer += $"{option}\n";
                        }
                    }
                    else if(questionTypeToCheck == SD.file)
                    {
                        foreach (var file in submission.Files)
                        {
                            foreach (var fi in answer.Answer.Split("||"))
                            {
                                if(file.FileName == fi.Trim())
                                {
                                    var fileId = _fileSaver.UploadFilesToGoogleDrive(file);
                                    response.Answer += !String.IsNullOrEmpty(fileId) ? $"https://drive.google.com/file/d/{fileId}/view?usp=drivesdk\n" : "";
                                }
                            }                            
                        }
                    }
                    else
                    {
                        if (questionTypeToCheck == SD.phoneType)
                        {
                            response.Answer = answer.Answer.Length > 4 ? answer.Answer : "";
                        }
                        else
                        {
                            response.Answer = answer.Answer;
                        }
                    }
                    responses.Add(response);
                }

                // Save to DB
                foreach (var response in responses)
                {
                    _unit.Response.Add(response).GetAwaiter().GetResult();
                }
                _unit.Save();
                FormFiller filler = new()
                {
                    Filler = userId,
                    FormId = submission.FormId,
                    SubmissionDate = DateTime.Now,
                };
                _unit.FormFiller.Add(filler).GetAwaiter().GetResult();
                _unit.Save();
            }
            catch(Exception ex)
            {
                return BadRequest(new { message = ex.Message } );
            }

            var email = _unit.ApplicationUser.Get(u => u.Id == userId).GetAwaiter().GetResult().Email;
            if(!string.IsNullOrEmpty(email))
            {
                _emailSender.SendEmailAsync(email.ToLower(), SD.Subject,
                    $"{SD.Message}{SD.AppBaseUrl}/Admin/Form/GetMySubmission?idHashed={checkedForm.IdHashed}").GetAwaiter().GetResult();
            }            
            return Ok(new { message = "Form submitted successfully" });
        }

        [HttpPost]
        public IActionResult SaveForm([FromForm] FormDto form)
        {
            TempData["baseUrl"] = SD.AppBaseUrl;
            var template = _unit.Template.Get(u => u.TemplateId == form.TemplateId, null, true).GetAwaiter().GetResult();
            if (form == null) return BadRequest("Invalid form data.");
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            if (form.TemplateId != 0)
            {
                Form formToSave = new()
                {
                    FormId = form.FormId,
                    Title = form.Title,
                    Description = form.Description ?? "",
                    TemplateId = form.TemplateId,
                    CreatedDate = form.CreatedDate,
                    CreatorId = userId,
                    ImageId = !String.IsNullOrEmpty(template.ImageId) ? template.ImageId : ""
                };
                _unit.Form.Add(formToSave).GetAwaiter().GetResult();
                _unit.Save();

                 var createdForm = _unit.Form.Get(u => u.CreatedDate == form.CreatedDate && u.CreatorId == userId, null, true).GetAwaiter().GetResult();
                createdForm.IdHashed = _hmacGenerator.GenerateHmac(createdForm.FormId);
                
                if (template != null)
                {
                    template.Point += 1;
                    _unit.Template.Update(template);
                }

                _unit.Form.Update(createdForm);
                _unit.Save();
            }

            var createdFormId = _unit.Form.Get(u => u.CreatedDate == form.CreatedDate).GetAwaiter().GetResult().FormId;

            foreach (var questionDto in form.Questions)
            {
                Question questionToSave = new()
                {
                    QuestionId = questionDto.QuestionId,
                    Text = questionDto.Text,
                    Type = questionDto.Type,
                    Order = questionDto.Order,
                    IsOptional = questionDto.IsOptional,
                    FormId = createdFormId
                };

                if (questionDto.QuestionId == 0)
                {                    
                    _unit.Question.Add(questionToSave).GetAwaiter().GetResult();
                }
                else
                {
                    _unit.Question.Update(questionToSave);
                }
                _unit.Save();

                // Save Options
                if((questionDto.Type == SD.checkBoxType || questionDto.Type == SD.radioBoxType) && questionDto.Options.Any())
                {
                    foreach (var option in questionDto.Options)
                    {
                        QuestionOption questionOptionToSave = new()
                        {
                            OptionId = option.OptionId,
                            Text = option.Text,                            
                        };

                        if (option.QuestionId == 0)
                        {
                            if (questionDto.QuestionId != 0)
                            {
                                questionOptionToSave.QuestionId = questionDto.QuestionId;
                            }
                            else
                            {
                                int questionId = _unit.Question.Get(u => u.Text == questionDto.Text && u.Order == questionDto.Order).
                                    GetAwaiter().GetResult().QuestionId;
                                if (questionId > 0)
                                {
                                    questionOptionToSave.QuestionId = questionId;
                                }
                            }
                            
                            _unit.QuestionOption.Add(questionOptionToSave).GetAwaiter().GetResult();
                        }
                        else{
                                questionOptionToSave.QuestionId = option.QuestionId;
                                _unit.QuestionOption.Update(questionOptionToSave);
                         }                        
                        _unit.Save();
                    }
                }
                
            }

            _unit.Save();
            return Ok(new { message = "Form saved successfully." });
        }

        [HttpPost]
        public IActionResult UpdateForm([FromForm] FormDto form)
        {
            TempData["baseUrl"] = SD.AppBaseUrl;
            var formToUpdate = _unit.Form.Get(u => u.FormId == form.FormId).GetAwaiter().GetResult();
            if (formToUpdate == null)
            {
                return NotFound(new { Message = "Form is not found" });
            }
            formToUpdate.ModifiedDate = DateTime.Now;
            foreach (var questionDto in form.Questions)
            {
                Question questionToSave = new()
                {
                    QuestionId = questionDto.QuestionId,
                    Text = questionDto.Text,
                    Type = questionDto.Type,
                    Order = questionDto.Order,
                    IsOptional = questionDto.IsOptional,
                    FormId = formToUpdate.FormId
                };

                if (questionDto.QuestionId == 0)
                {
                    _unit.Question.Add(questionToSave).GetAwaiter().GetResult();
                } else {
                    _unit.Question.Update(questionToSave);
                }
                _unit.Save();

                if ((questionDto.Type == SD.checkBoxType || questionDto.Type == SD.radioBoxType) && questionDto.Options.Any())
                {
                    foreach (var option in questionDto.Options)
                    {
                        QuestionOption questionOptionToSave = new()
                        {
                            OptionId = option.OptionId,
                            Text = option.Text,
                        };

                        if (option.QuestionId == 0)
                        {
                            if (questionDto.QuestionId != 0)
                            {
                                questionOptionToSave.QuestionId = questionDto.QuestionId;
                            }
                            else
                            {
                                int questionId = _unit.Question.Get(u => u.Text == questionDto.Text && u.Order == questionDto.Order).
                                    GetAwaiter().GetResult().QuestionId;
                                if (questionId > 0)
                                {
                                    questionOptionToSave.QuestionId = questionId;
                                }
                            }

                            _unit.QuestionOption.Add(questionOptionToSave).GetAwaiter().GetResult();
                        }
                        else
                        {
                            questionOptionToSave.QuestionId = option.QuestionId;
                            if (option.OptionId == 0)
                            {                                
                                _unit.QuestionOption.Add(questionOptionToSave).GetAwaiter().GetResult();
                            }
                            else
                            {
                                _unit.QuestionOption.Update(questionOptionToSave);
                            }                           
                            
                        }
                        _unit.Save();
                    }
                }
            }
            _unit.Save();
            return Ok(new { message = "Form updated successfully." });
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

            return Ok(new { success = true, message = "Deletion successfully performed" });
        }

        [HttpDelete("{questionId:int}")]
        public async Task<IActionResult> DeleteQuestion(int? questionId)
        {
            var question = await _unit.Question.Get(u => u.QuestionId == questionId);
            if (question == null)
            {
                return NotFound(new { message = "Question is not found." });
            }
            _unit.Question.Remove(question);
            _unit.Save();
            return Ok(new { message = "Question is successfully deleted" });
        }

        [HttpDelete("{optionId:int}")]
        public async Task<IActionResult> DeleteQuestionOption(int? optionId)
        {
            var questionOption = await _unit.QuestionOption.Get(u => u.OptionId == optionId);
            if (questionOption == null)
            {
                return NotFound(new { message = "Option is not found." });
            }
            _unit.QuestionOption.Remove(questionOption);
            _unit.Save();
            return Ok(new { message = "Option is successfully deleted" });
        }

        [HttpPost]
        public async Task<IActionResult> ExportToCvs(int formId)
        {
            Form form = await _unit.Form.Get(u => u.FormId == formId);
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            if (form == null || form.CreatorId != userId)
            {
                TempData["error"] = "Error while retrieving data";
                return BadRequest();
            }

            FormFillers = new()
            {
                Questions = _unit.Question.GetAll(u => u.FormId == formId).OrderBy(u => u.QuestionId)
            };

            List<FormFiller> fillers = new List<FormFiller>();
            fillers = _unit.FormFiller.GetAll(u => u.FormId == formId).ToList();

            if(fillers.Count == 0)
            {
                return NotFound();
            }
            for (int i = 0; i < fillers.Count; i++)
            {
                fillers[i].Responses = _unit.Response.GetAll(u => u.Filler == fillers[i].Filler && u.FormId == formId).OrderBy(u => u.QuestionId).ToList();
                fillers[i].ApplicationUser = await _unit.ApplicationUser.Get(u => u.Id == fillers[i].Filler) ?? new();
            }
            FormFillers.FormFillers = fillers ?? new List<FormFiller>();
            var cvs = new StringBuilder();  
            var header = new StringBuilder();

            header.AppendJoin(',', "Contributor");
            foreach (var question in FormFillers.Questions)
            {
                header.AppendJoin(',', $",{question.Text}");
            }
            cvs.AppendLine(header.ToString().TrimStart(','));
            foreach(var filler in FormFillers.FormFillers) 
            {
                var body = new StringBuilder();
                body.AppendJoin(',', filler.ApplicationUser.Email);
                foreach (var response in filler.Responses)
                {
                    body.AppendJoin(',', $",{response.Answer}");
                }
                cvs.AppendLine(body.ToString().TrimStart(','));
            }
            return File(Encoding.UTF8.GetBytes(cvs.ToString()), "text/cvs", form.Title+"_submissions.cvs");
        }
    }   
    #endregion
}
