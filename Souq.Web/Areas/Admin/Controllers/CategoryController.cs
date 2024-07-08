using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Souq.Entities.Models;
using Souq.Entities.Repositories;
using Souq.Utilities;

namespace Souq.Web.Areas.Admin.Controllers
{
    [Area(SD.AdminRole)]
    [Authorize(Roles = SD.AdminRole)]
    public class CategoryController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public CategoryController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IActionResult> Index()
        {
            var categories = await _unitOfWork.Repository<Category>().GetAll();
            //var categories = await _unitOfWork.CategoryRepository.GetAll();

            return View(categories);
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Category category)
        {
            if (ModelState.IsValid)
            {
                await _unitOfWork.Repository<Category>().Add(category);
                await _unitOfWork.Complete();
                TempData["Create"] = "Data has created successfully.";
                return RedirectToAction("Index");
            }
            return View(category);
        }
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id is null || id == 0)
                return NotFound();
            var category = await _unitOfWork.Repository<Category>().GetFirstOrDefault(x => x.Id == id);
            if (category == null)
                return NotFound();
            return View(category);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Category category)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.Repository<Category>().Update(category);
                await _unitOfWork.Complete();
                TempData["Update"] = "Data has updated successfully.";
                return RedirectToAction("Index");
            }
            return View(category);
        }
        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id is null || id == 0)
                return NotFound();
            var category = await _unitOfWork.Repository<Category>().GetFirstOrDefault(x => x.Id == id);
            if (category == null)
                return NotFound();
            return View(category);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete([FromRoute] int id, Category category)
        {
            if (id != category.Id)
                return BadRequest("ID Mismatch");
            if (category == null)
                return NotFound();
            _unitOfWork.Repository<Category>().Remove(category);
            await _unitOfWork.Complete();
            TempData["Delete"] = "Data has deleted successfully.";
            return RedirectToAction("Index");
        }
    }
}
