using System;
using System.Collections;
using System.Linq;
using BulkyBook.DataAccess.Data;
using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models.Models;
using BulkyBook.Models.ViewModels;
using BulkyBook.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace BulkyBookWeb.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = SD.Role_Admin)]
public class UserController : Controller
{
    private readonly ApplicationDbContext _db;
    private readonly UserManager<IdentityUser> _userManager;

    public UserController(ApplicationDbContext db, UserManager<IdentityUser> userManager)
    {
        _db = db;
        _userManager = userManager;
    }


    public IActionResult Index()
    {
        return View();
    }

    public IActionResult RoleManagement(string userId)
    {
        var roleId = _db.UserRoles.FirstOrDefault(x => x.UserId == userId).RoleId;
        var roleManagementVm = new RoleManagementVm
        {
            ApplicationUser =_db.ApplicationUsers.Include(x=>x.Company).FirstOrDefault(x=>x.Id==userId),
            RoleList = _db.Roles.Select( x=>new SelectListItem
            {
                Text = x.Name,
                Value = x.Name
            }),
            CompanyList = _db.Companies.Select(x=> new SelectListItem
            {
                Text = x.Name,
                Value = x.CompanyId.ToString()
            })
        };

        roleManagementVm.ApplicationUser.Role = _db.Roles.FirstOrDefault(x => x.Id == roleId).Name;
        
        return View(roleManagementVm);
    }
    [HttpPost]
    public IActionResult RoleManagement(RoleManagementVm roleManagementVm)
    {
        var identityUserRoles = _db.UserRoles.ToList();
        var roleId = _db.UserRoles.FirstOrDefault(x => x.UserId == roleManagementVm.ApplicationUser.Id).RoleId;
        string oldRole = _db.Roles.FirstOrDefault(x=>x.Id==roleId).Name;
        if (roleManagementVm.ApplicationUser.Role != oldRole )
        {
            var user = _db.ApplicationUsers.FirstOrDefault(x => x.Id == roleManagementVm.ApplicationUser.Id);
            if (roleManagementVm.ApplicationUser.Role == SD.Role_Company)
            {
                user.CompanyId = roleManagementVm.ApplicationUser.CompanyId;
            }
            if (oldRole == SD.Role_Company)
            {
                user.CompanyId = null;
            }
            _db.SaveChanges();

            _userManager.RemoveFromRoleAsync(user, oldRole).GetAwaiter().GetResult();
            _userManager.AddToRoleAsync(user, roleManagementVm.ApplicationUser.Role).GetAwaiter().GetResult();

        }
        
        return RedirectToAction(nameof(Index));
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
            var roleId = userRoles.FirstOrDefault(x => x.UserId == user.Id).RoleId;
            user.Role = roles.FirstOrDefault(x => x.Id == roleId).Name;
            if (user.Company == null)
            {
                
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
