using System.Collections;
using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models.Models;
using BulkyBook.Models.ViewModels;
using BulkyBook.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;


namespace BulkyBookWeb.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = SD.Role_Admin)]
public class CompanyController : Controller
{
    private readonly IUnitOfWork _unitOfWork;

    public CompanyController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public IActionResult Index()
    {
        var companies = _unitOfWork.Company.GetAll().ToList();
        return View(companies);
    }

    public IActionResult Upsert(int? id)
    {
       
        if (id == null || id == 0)
        {
            //create
            return View(new Company());
        }
        else
        {
            //update
            Company company = _unitOfWork.Company.Get(x => x.CompanyId == id);
            return View(company);
        }
    }

    [HttpPost]
    public IActionResult Upsert(Company company)
    {
        if (ModelState.IsValid)
        {
            if (company.CompanyId ==0)
            {
                _unitOfWork.Company.Add(company);
            }
            else
            {
                _unitOfWork.Company.Update(company);
            }
            _unitOfWork.Save();
            TempData["success"] = "Company created successfully.";
            return RedirectToAction(nameof(Index));
        }
        
        return View(company);
    }

    
    #region API CALLS
    [HttpGet]
    public IActionResult GetAll()
    {
        var companies = _unitOfWork.Company.GetAll().ToList();
        return Json(new { data = companies });
    }

    [HttpDelete]
    public IActionResult Delete(int? id)
    {
        var company = _unitOfWork.Company.Get(x => x.CompanyId == id);
        if (company == null)
        {
            return Json(new { success = false, message = "Error while deleting" });
        }
        _unitOfWork.Company.Remove(company);
        _unitOfWork.Save();
        return Json(new { success = true, message = "Delete Company Successfully" });
    }
    
    #endregion
    
}