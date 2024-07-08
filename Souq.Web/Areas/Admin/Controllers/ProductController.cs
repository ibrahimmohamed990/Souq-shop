using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Souq.Entities.Models;
using Souq.Entities.Repositories;
using Souq.Entities.ViewModels;
using Souq.Utilities;
using Souq.Web.Services.Product;
using Stripe;
using Product = Souq.Entities.Models.Product;

namespace Souq.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.AdminRole)]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ProductController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IActionResult> Index()
        {
            IEnumerable<Product> products  = await _unitOfWork.Repository<Product>().GetAll();
            
            return View(products);
        }
        public async Task<IActionResult> GetData()
        {
            var products = await _unitOfWork.ProductRepository.GetAll(Include: "Category");

            return Json(new {data = products});
        }
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var categories = await _unitOfWork.CategoryRepository.GetAll();
            ProductVM productVM = new ProductVM()
            {
                Product = new Product(),
                CategoryList = categories.Select(x => new SelectListItem
                {
                    Text = x.Name,
                    Value = x.Id.ToString()
                })
            };

            return View(productVM);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductVM productVM, IFormFile file)
        {
            if (ModelState.IsValid)
            {
                if (file != null)
                {
                    productVM.Product.ImageUrl = PictureSetting.UploadFile(file, "products");
                }
                //var productToAdd = _mapper.Map<Product>(productVM.Product);
                await _unitOfWork.Repository<Product>().Add(productVM.Product);
                await _unitOfWork.Complete();
                TempData["Create"] = "Data has created successfully.";
                return RedirectToAction("Index");
            }
            var categories = await _unitOfWork.CategoryRepository.GetAll();
            productVM.CategoryList = categories.Select(x => new SelectListItem
            {
                Text = x.Name,
                Value = x.Id.ToString()
            });
            return View(productVM);
        }
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id is null || id == 0)
                return NotFound();

            var categories = await _unitOfWork.CategoryRepository.GetAll();
            var product = await _unitOfWork.ProductRepository.GetFirstOrDefault(x => x.Id == id);
            ProductVM productVM = new ProductVM()
            {
                Product = product,
                CategoryList = categories.Select(x => new SelectListItem
                {
                    Text = x.Name,
                    Value = x.Id.ToString()
                })
            };

            return View(productVM);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ProductVM productVM, IFormFile? file)
        {
            if (ModelState.IsValid)
            {
                if (file != null)
                {
                    if(productVM.Product.ImageUrl != null)
                        PictureSetting.DeleteFile("products", productVM.Product.ImageUrl);

                    productVM.Product.ImageUrl = PictureSetting.UploadFile(file, "products");
                }
                _unitOfWork.Repository<Product>().Update(productVM.Product);
                await _unitOfWork.Complete();
                TempData["Update"] = "Data has updated successfully.";
                return RedirectToAction("Index");
            }
            var categories = await _unitOfWork.CategoryRepository.GetAll();
            productVM.CategoryList = categories.Select(x => new SelectListItem
            {
                Text = x.Name,
                Value = x.Id.ToString()
            });
            return View(productVM);
        }
        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id is null || id == 0)
                return NotFound();
            var product = await _unitOfWork.Repository<Product>().GetFirstOrDefault(x => x.Id == id, Include:"Category");
            if (product == null)
                return NotFound();
            var deleteProduct = new ProductToDelete
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                CategoryName = product.Category.Name,
                Price = product.Price,
                ImageUrl = product.ImageUrl
            };
            return View(deleteProduct);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete([FromRoute] int id, Product product)
        {
            if (id != product.Id)
                return BadRequest("ID Mismatch");
            if (product == null)
                return NotFound();

            _unitOfWork.Repository<Product>().Remove(product);

            if (!string.IsNullOrEmpty(product.ImageUrl))
                PictureSetting.DeleteFile("products", product.ImageUrl);

            await _unitOfWork.Complete();
            TempData["Delete"] = "Data has deleted successfully.";

            return RedirectToAction("Index");
        }
    }
}
