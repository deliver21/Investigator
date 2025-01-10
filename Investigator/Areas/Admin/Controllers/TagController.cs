using Investigator.Models;
using Investigator.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;

namespace Investigator.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class TagController : Controller
    {
        private readonly IUnitOfWork _unit;
        public TagController(IUnitOfWork unit)
        {
            _unit = unit;
        }
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Upsert(int ? tagId)
        {
            var tag = tagId != 0 ? await _unit.TemplateTag.Get(u => u.TagId == tagId) : new Models.TemplateTag();
            return View(tag);
        }

        [HttpPost]
        public IActionResult Upsert(TemplateTag tag)
        {
            if (tag.TagId == 0)
            {
                _unit.TemplateTag.Add(tag);
            }
            else
            {
                _unit.TemplateTag.Update(tag);
            }
            _unit.Save();
            return RedirectToAction(nameof(Index));
        }

        #region APIs Calls

        [HttpGet]
        public IActionResult GetAll()
        {
            IEnumerable<TemplateTag> templateTags = _unit.TemplateTag.GetAll();
            return Json(new {data = templateTags});
        }
        [HttpDelete]
        public async Task <IActionResult> Delete(int? id)
        {
            TemplateTag tagToDelete = await _unit.TemplateTag.Get(u => u.TagId == id);
            if (tagToDelete == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }
            else
            {
                _unit.TemplateTag.Remove(tagToDelete);
                _unit.Save();
            }
            return Json(new { success = true, message = "Delete successfully performed" });
        }
        #endregion
    }
}
