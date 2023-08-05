
using Bulky.DataAccess.Data;
using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models.Models;
using Microsoft.AspNetCore.Mvc;

namespace BulkyWeb.Controllers;

public class CategoryController : Controller
{
    private readonly ICategoryRepository _repository;

    public CategoryController(ICategoryRepository repository)
    {
        _repository = repository;
    }

    // GET
    public IActionResult Index()
    {
        List<Category> categories = _repository.GetAll().ToList();
        return View(categories);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Create(Category category)
    {
        if (category.Name == category.DisplayOrder.ToString())
        {
            ModelState.AddModelError(nameof(category.Name), "Name and Display Order cannot be the same.");
        }

        if (ModelState.IsValid)
        {
            _repository.Add(category);
            _repository.Save();
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

        var category = _repository.Get(x=>x.CategoryId == id);
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
            _repository.Update(category);
            _repository.Save();
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

        var category = _repository.Get(x=>x.CategoryId==id);
        if (category is null)
        {
            return NotFound();
        }

        return View(category);
    }

    [HttpPost,ActionName("Delete")]
    public IActionResult DeletePost(int? id)
    {
        var category = _repository.Get(x=>x.CategoryId==id);
        if (category is null)
        {
            return NotFound();
        }
        _repository.Remove(category);
        _repository.Save();
        TempData["success"] = "Category deleted successfully.";
        return RedirectToAction(nameof(Index));
    }
}