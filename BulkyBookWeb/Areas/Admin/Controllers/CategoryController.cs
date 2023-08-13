using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models.Models;
using BulkyBook.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BulkyBookWeb.Areas.Admin.Controllers;
[Area("Admin")]
[Authorize(Roles = SD.Role_Admin)]
public class CategoryController : Controller
{
    private readonly IUnitOfWork _unitOfWork;
    public CategoryController( IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    // GET
    public IActionResult Index()
    {
        List<Category> categories = _unitOfWork.Category.GetAll().ToList();
        return View(categories);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Create(Category category)
    {
        if (ModelState.IsValid)
        {
            _unitOfWork.Category.Add(category);
            _unitOfWork.Save();
            TempData["success"] = "Category created successfully.";
            return RedirectToAction(nameof(Index));
        }

        return View();
    }

    public IActionResult Edit(int? id)
    {
        if (id == null || id == 0)
        {
            return NotFound();
        }

        var category = _unitOfWork.Category.Get(x=>x.CategoryId == id);
        if (category is null)
        {
            return NotFound();
        }

        return View(category);
    }

    [HttpPost]
    public IActionResult Edit(Category category)
    {
        if (ModelState.IsValid)
        {
            _unitOfWork.Category.Update(category);
            _unitOfWork.Save();
            TempData["success"] = "Category updated successfully.";
            return RedirectToAction(nameof(Index));
        }

        return View();
    }


    public IActionResult Delete(int? id)
    {
        if (id == null || id == 0)
        {
            return NotFound();
        }

        var category = _unitOfWork.Category.Get(x=>x.CategoryId==id);
        if (category is null)
        {
            return NotFound();
        }

        return View(category);
    }

    [HttpPost,ActionName("Delete")]
    public IActionResult DeletePost(int? id)
    {
        var category = _unitOfWork.Category.Get(x=>x.CategoryId==id);
        if (category is null)
        {
            return NotFound();
        }
        _unitOfWork.Category.Remove(category);
        _unitOfWork.Save();
        TempData["success"] = "Category deleted successfully.";
        return RedirectToAction(nameof(Index));
    }
}