using JustLearn1.Repository.IRepository;
using JustLearn1.Data;
using JustLearn1.Models;
using JustLearn1.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using JustLearn1.Models.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace JustLearn1Web.Areas.Admin.Controllers
{

    public class ProductsController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly UserManager<IdentityUser> _userManager;
        public ProductsController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment, UserManager<IdentityUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
            _userManager = userManager;
        }
        public IActionResult Admin()
        {
            return View();
        }
        public IActionResult Shop()
        {
            List<Product> objProductsList = _unitOfWork.Product.GetAll(includeProperties: "Category").ToList();

            return View(objProductsList);
        }

        public IActionResult Upsert(int? id)
        {


            var model = new ProductsVM();


            ProductsVM ProductsVM = new()
            {
                CategoryList = _unitOfWork.Category.GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                }),
                Product = new Product()
            };
            if (id == null || id == 0)
            {
                //create
                ProductsVM.IsTrending = false;
                return View(ProductsVM);
            }
            else
            {
                //update
                ProductsVM.Product = _unitOfWork.Product.Get(u => u.Id == id);
                ProductsVM.IsTrending = ProductsVM.Product.IsTrendingProduct;
                return View(ProductsVM);
            }

        }
        [HttpPost]
        public async Task<IActionResult> Upsert(ProductsVM ProductsVM, IFormFile? file)
        {
            if (ModelState.IsValid)
            {
                var currentUser = await _userManager.GetUserAsync(User);  


                if (currentUser != null)
                {
                    ProductsVM.Product.UserId = currentUser.Id;
                }
                else
                {

                    ModelState.AddModelError(string.Empty, "User is not logged in");
                    return View(ProductsVM);  // veya başka bir işlem
                }

                string wwwRootPath = _webHostEnvironment.WebRootPath;
                if (file != null)
                {
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    string ProductsPath = Path.Combine(wwwRootPath, @"images\Products");

                    if (!string.IsNullOrEmpty(ProductsVM.Product.ImageUrl))
                    {

                        var oldImagePath =
                            Path.Combine(wwwRootPath, ProductsVM.Product.ImageUrl.TrimStart('\\'));

                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }

                    using (var fileStream = new FileStream(Path.Combine(ProductsPath, fileName), FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }

                    ProductsVM.Product.ImageUrl = @"\images\Products\" + fileName;
                }

                if (ProductsVM.Product.Id == 0)
                {
                    _unitOfWork.Product.Add(ProductsVM.Product);
                }
                else
                {
                    _unitOfWork.Product.Update(ProductsVM.Product);
                }

                _unitOfWork.Save();
                TempData["success"] = "Products created successfully";
                return RedirectToAction("Shop");
            }
            else
            {
                ProductsVM.CategoryList = _unitOfWork.Category.GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                });
                return View(ProductsVM);
            }
        }


        public IActionResult Detail(int id)
        {
            var product = _unitOfWork.GetProductDetail(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }
        [Authorize(Roles = "Instructor")]
        public async Task<IActionResult> MyProducts()
        {
            var currentUser = await _userManager.GetUserAsync(User);

            if (currentUser == null)
            {
                return NotFound("User not found");
            }

            List<Product> objProductsList = _unitOfWork.Product.GetAll(includeProperties: "Category").ToList();


            return View(objProductsList);
        }


        #region API CALLS

        [HttpGet]
        public IActionResult GetAll()
        {
            List<Product> objProductsList = _unitOfWork.Product.GetAll(includeProperties: "Category").ToList();
            return Json(new { data = objProductsList });
        }


        [HttpPost]
        public IActionResult Delete(int? id)
        {
            var productToBeDeleted = _unitOfWork.Product.Get(u => u.Id == id);
            if (productToBeDeleted == null)
            {
                return RedirectToAction("MyProducts");
            }

            // ImageUrl null veya boş olup olmadığını kontrol et
            if (!string.IsNullOrEmpty(productToBeDeleted.ImageUrl))
            {
                var oldImagePath =
                    Path.Combine(_webHostEnvironment.WebRootPath, productToBeDeleted.ImageUrl.TrimStart('\\'));

                if (System.IO.File.Exists(oldImagePath))
                {
                    System.IO.File.Delete(oldImagePath);
                }
            }

            _unitOfWork.Product.Remove(productToBeDeleted);
            _unitOfWork.Save();

            return RedirectToAction("MyProducts");
        }




        #endregion
    }
}