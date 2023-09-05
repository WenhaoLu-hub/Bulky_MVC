using System.Collections;
using BulkyBook.DataAccess.Data;
using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models.Models;
using BulkyBook.Models.ViewModels;
using BulkyBook.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace BulkyBookWeb.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = SD.Role_Admin)]
public class UserController : Controller
{
    private readonly ApplicationDbContext _db;

    public UserController(ApplicationDbContext db)
    {
        _db = db;
    }


    public IActionResult Index()
    {
        return View();
    }

    

    #region API CALLS
    [HttpGet]
    public IActionResult GetAll()
    {
        var users = _db.ApplicationUsers.Include(x=>x.Company).ToList();
        var roles = _db.Roles.ToList();
        var userRoles = _db.UserRoles.ToList();
        foreach (var user in users)
        {
            if (user.Company == null)
            {
                var roleId = userRoles.FirstOrDefault(x => x.UserId == user.Id).RoleId;
                user.Role = roles.FirstOrDefault(x => x.Id == roleId).Name;
                user.Company = new Company
                {
                    Name = ""
                };
            }
        }
        return Json(new { data = users });
    }

    [HttpPost]
    public IActionResult LockUnlock([FromBody]string id)
    {
        var user = _db.ApplicationUsers.FirstOrDefault(x => x.Id == id);
        if (user == null)
        {
            return Json(new
            {
                success = false,
                message = "Error while Locking/Unlocking"
            });
        }

        if (user.LockoutEnd != null && user.LockoutEnd > DateTime.UtcNow)
        {
            user.LockoutEnd = DateTime.UtcNow;
        }
        else
        {
            user.LockoutEnd = DateTime.UtcNow.AddYears(100);
        }

        _db.SaveChanges();
        return Json(new { success = true, message = "Operation Successfully" });
    }

    #endregion
}
