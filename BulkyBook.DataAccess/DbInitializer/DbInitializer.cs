using BulkyBook.DataAccess.Data;
using BulkyBook.Models.Models;
using BulkyBook.Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BulkyBook.DataAccess.DbInitializer;

public class DbInitializer: IDbInitializer
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly ApplicationDbContext _db;

    public DbInitializer(ApplicationDbContext db, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        _db = db;
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public void Initialize()
    {
        //migrations if they are not applied
        
        try
        {
            if (_db.Database.GetPendingMigrations().Count() >0)
            {
                _db.Database.Migrate();
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
        
        
        //create roles if they are not created
        if (!_roleManager.RoleExistsAsync(SD.Role_Customer).GetAwaiter().GetResult())
        {
            _roleManager
                .CreateAsync(new IdentityRole(SD.Role_Customer))
                .GetAwaiter()
                .GetResult();
            _roleManager.CreateAsync(new IdentityRole(SD.Role_Admin)).GetAwaiter().GetResult();
            _roleManager
                .CreateAsync(new IdentityRole(SD.Role_Company))
                .GetAwaiter()
                .GetResult();
            _roleManager
                .CreateAsync(new IdentityRole(SD.Role_Employee))
                .GetAwaiter()
                .GetResult();
            
            //if roles are not created, then we will create admin user as well
            _userManager.CreateAsync(new ApplicationUser
            {
                UserName = "admin@bulky.com",
                Email = "admin@bulky.com",
                Name = "Darwin",
                PhoneNumber = "88888888",
                StreetAddress = "123 Main Street",
                State = "SH",
                PostalCode = "1234",
                City = "SH",
            },"Admin123*").GetAwaiter().GetResult();
            
            
            var user = _db.ApplicationUsers.FirstOrDefault(x=>x.Email=="admin@bulky.com");
            _userManager.AddToRoleAsync(user, SD.Role_Admin).GetAwaiter().GetResult();
        }
        return;
        
    }
}