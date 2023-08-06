using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace BulkyBookWeb.Areas.Admin.Controllers;

[Area("Admin")]
public class ProductController : Controller
{
    private readonly IUnitOfWork _unitOfWork;

    public ProductController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public IActionResult Index()
    {
        var products = _unitOfWork.Product.GetAll().ToList();
        return View(products);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Create(Product product)
    {
        if (ModelState.IsValid)
        {
            _unitOfWork.Product.Add(product);
            _unitOfWork.Save();
            TempData["success"] = "Product created successfully.";
            return RedirectToAction(nameof(Index));
        }

        return View();
    }

    public IActionResult Edit(int? id)
    {
        if (id ==0 || id == null)
        {
            return NotFound();
        }

        var product = _unitOfWork.Product.Get(x=>x.ProductId == id);
        if (product is null)
        {
            return NotFound();
        }
        return View(product);
    }

    [HttpPost]
    public IActionResult Edit(Product product)
    {
        if (ModelState.IsValid)
        {
            _unitOfWork.Product.Update(product);
            _unitOfWork.Save();
            TempData["success"] = "Product updated successfully.";
            return RedirectToAction(nameof(Index));
        }

        return View();
    }
    
    public IActionResult Delete(int? id)
    {
        if (id ==0 || id == null)
        {
            return NotFound();
        }

        var product = _unitOfWork.Product.Get(x=>x.ProductId == id);
        if (product is null)
        {
            return NotFound();
        }
        return View(product);
    }

    [HttpPost,ActionName("Delete")]
    public IActionResult DeletePost(int? id)
    {
        var product = _unitOfWork.Product.Get(x => x.ProductId == id);
        if (product is null)
        {
            return NotFound();
        }
        _unitOfWork.Product.Remove(product);
        _unitOfWork.Save();
        TempData["success"] = "Product deleted successfully.";
        return RedirectToAction(nameof(Index));
    }
    

}