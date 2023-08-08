using System.Collections;
using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models.Models;
using BulkyBook.Models.VIewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace BulkyBookWeb.Areas.Admin.Controllers;

[Area("Admin")]
public class ProductController : Controller
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IWebHostEnvironment _hostEnvironment;

    public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment hostEnvironment)
    {
        _unitOfWork = unitOfWork;
        _hostEnvironment = hostEnvironment;
    }

    public IActionResult Index()
    {
        var products = _unitOfWork.Product.GetAll(includeProperties:"Category").ToList();
        return View(products);
    }

    public IActionResult Upsert(int? id)
    {
        var productVm = new ProductVM
        {
            Product = new Product(),
            CategoryList = _unitOfWork.Category.GetAll().Select(
                x => new SelectListItem
                {
                    Text = x.Name,
                    Value = x.CategoryId.ToString()
                })
        };
        if (id == null || id == 0)
        {
            //create
            return View(productVm);
        }
        else
        {
            //update
            productVm.Product = _unitOfWork.Product.Get(x => x.ProductId == id);
            return View(productVm);
        }
    }

    [HttpPost]
    public IActionResult Upsert(ProductVM productVm, IFormFile? file)
    {
        if (ModelState.IsValid)
        {
            string wwwRootPath = _hostEnvironment.WebRootPath;
            if (file != null)
            {
                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                string productPath = Path.Combine(wwwRootPath, @"images/product");
                if (!string.IsNullOrEmpty(productVm.Product.ImageUrl))
                {
                    //delete old file
                    var oldImagePath = Path.Combine(wwwRootPath, productVm.Product.ImageUrl.TrimStart('/'));
                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                }
                using (var fileStream = new FileStream(Path.Combine(productPath, fileName), FileMode.Create))
                {
                    file.CopyTo(fileStream);
                }
                
                productVm.Product.ImageUrl=@"/images/product/"+fileName;
            }

            if (productVm.Product.ProductId ==0)
            {
                _unitOfWork.Product.Add(productVm.Product);
            }
            else
            {
                _unitOfWork.Product.Update(productVm.Product);
            }
            _unitOfWork.Save();
            TempData["success"] = "Product created successfully.";
            return RedirectToAction(nameof(Index));
        }
        else
        {
            productVm.CategoryList = _unitOfWork.Category.GetAll().Select(
                x => new SelectListItem
                {
                    Text = x.Name,
                    Value = x.CategoryId.ToString()
                });
        }

        return View(productVm);
    }


    // public IActionResult Delete(int? id)
    // {
    //     if (id == 0 || id == null)
    //     {
    //         return NotFound();
    //     }
    //
    //     var product = _unitOfWork.Product.Get(x => x.ProductId == id);
    //     if (product is null)
    //     {
    //         return NotFound();
    //     }
    //
    //     return View(product);
    // }
    //
    // [HttpPost, ActionName("Delete")]
    // public IActionResult DeletePost(int? id)
    // {
    //     var product = _unitOfWork.Product.Get(x => x.ProductId == id);
    //     if (product is null)
    //     {
    //         return NotFound();
    //     }
    //
    //     _unitOfWork.Product.Remove(product);
    //     _unitOfWork.Save();
    //     TempData["success"] = "Product deleted successfully.";
    //     return RedirectToAction(nameof(Index));
    // }
    #region API CALLS
    [HttpGet]
    public IActionResult GetAll()
    {
        var products = _unitOfWork.Product.GetAll(includeProperties: "Category").ToList();
        return Json(new { data = products });
    }

    [HttpDelete]
    public IActionResult Delete(int? id)
    {
        var product = _unitOfWork.Product.Get(x => x.ProductId == id);
        if (product == null)
        {
            return Json(new { success = false, message = "Error while deleting" });
        }
        var oldImagePath = Path.Combine(_hostEnvironment.WebRootPath, product.ImageUrl.TrimStart('/'));
        if (System.IO.File.Exists(oldImagePath))
        {
            System.IO.File.Delete(oldImagePath);
        }
        _unitOfWork.Product.Remove(product);
        _unitOfWork.Save();
        return Json(new { success = true, message = "Delete Product Successfully" });
    }
    
    #endregion
    
}