using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Youtube_Entertainment_Project.DTOs;
using Youtube_Entertainment_Project.Services.Interfaces;

namespace Youtube_Entertainment_Project.Controllers
{
    [Authorize(Roles = "SuperAdmin")]
    public class CategoryController : Controller
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        // GET: /Category
        public async Task<IActionResult> Index()
        {
            var categories = await _categoryService.GetAllCategoriesAsync();
            ViewBag.AllCategories = categories;
            return View(categories);
        }

        // GET: /Category/Details/5
        public async Task<IActionResult> Details(Guid id)
        {
            var category = await _categoryService.GetCategoryByIdAsync(id);
            if (category == null) return NotFound();
            return View(category);
        }

        // GET: /Category/Create
        public async Task<IActionResult> Create()
        {
            ViewBag.Categories = new SelectList(await _categoryService.GetAllCategoriesAsync(), "CategoryId", "Name");
            return View();
        }


        // POST: /Category/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CategoryDto dto)
        {
            if (ModelState.IsValid)
            {
                // Call your service to create the category
                await _categoryService.CreateCategoryAsync(dto);

                return RedirectToAction(nameof(Index));
            }

            // If ModelState is invalid, reload parent categories
            ViewBag.Categories = new SelectList(await _categoryService.GetAllCategoriesAsync(), "CategoryId", "Name");
            return View(dto);
        }



        // GET: /Category/Edit/5
        public async Task<IActionResult> Edit(Guid id)
        {
            var category = await _categoryService.GetCategoryByIdAsync(id);
            if (category == null) return NotFound();

            var categories = await _categoryService.GetAllCategoriesAsync();

            // Exclude the current category to prevent circular reference
            ViewBag.Categories = categories.Where(c => c.CategoryId != id).ToList();
            return View(category);
        }

        // POST: /Category/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, CategoryDto dto)
        {
            if (id != dto.CategoryId) return BadRequest();

            if (ModelState.IsValid)
            {
                await _categoryService.UpdateCategoryAsync(id, dto);
                return RedirectToAction(nameof(Index));
            }

            var categories = await _categoryService.GetAllCategoriesAsync();
            ViewBag.Categories = categories.Where(c => c.CategoryId != id).ToList();
            return View(dto);
        }


        // GET: /Category/Delete/5
        public async Task<IActionResult> Delete(Guid id)
        {
            var category = await _categoryService.GetCategoryByIdAsync(id);
            if (category == null) return NotFound();

            var dto = new CategoryDto
            {
                CategoryId = category.CategoryId,
                Name = category.Name,
                ParentCategoryId = category.ParentCategoryId
            };

            return View(dto);
        }


        // POST: /Category/Delete/5
        [HttpPost, ActionName("DeleteConfirmed")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid categoryId)
        {
            try
            {
                await _categoryService.DeleteCategoryAsync(categoryId);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                // Optionally log the error
                return NotFound(ex.Message);
            }
        }


    }
}
