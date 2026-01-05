using Microsoft.AspNetCore.Mvc;
using Youtube_Entertainment_Project.DTOs;
using Youtube_Entertainment_Project.Services.Interfaces;

namespace Youtube_Entertainment_Project.Controllers
{
    public class TagController : Controller
    {
        private readonly ITagService _tagService;

        public TagController(ITagService tagService)
        {
            _tagService = tagService;
        }

        // GET: /Tag
        public async Task<IActionResult> Index()
        {
            var tags = await _tagService.GetAllTagsAsync();
            return View(tags);
        }

        // GET: /Tag/Details/5
        public async Task<IActionResult> Details(Guid id)
        {
            var tag = await _tagService.GetTagByIdAsync(id);
            if (tag == null) return NotFound();
            return View(tag);
        }

        // GET: /Tag/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: /Tag/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TagDto dto)
        {
            if (ModelState.IsValid)
            {
                await _tagService.CreateTagAsync(dto);
                return RedirectToAction(nameof(Index));
            }
            return View(dto);
        }

        // GET: /Tag/Edit/5
        public async Task<IActionResult> Edit(Guid id)
        {
            var tag = await _tagService.GetTagByIdAsync(id);
            if (tag == null) return NotFound();
            return View(tag);
        }

        // POST: /Tag/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, TagDto dto)
        {
            if (id != dto.TagId) return BadRequest();

            if (ModelState.IsValid)
            {
                await _tagService.UpdateTagAsync(id, dto);
                return RedirectToAction(nameof(Index));
            }
            return View(dto);
        }

        // GET: /Tag/Delete/5
        public async Task<IActionResult> Delete(Guid id)
        {
            var tag = await _tagService.GetTagByIdAsync(id);
            if (tag == null) return NotFound();
            return View(tag);
        }

        // POST: /Tag/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            await _tagService.DeleteTagAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
