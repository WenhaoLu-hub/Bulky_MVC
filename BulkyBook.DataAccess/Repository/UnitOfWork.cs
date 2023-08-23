using BulkyBook.DataAccess.Data;
using BulkyBook.DataAccess.Repository.IRepository;

namespace BulkyBook.DataAccess.Repository;

public class UnitOfWork : IUnitOfWork
{
    
    public ICategoryRepository Category { get; private set; }
    public IProductRepository Product { get; private set; }
    
    public ICompanyRepository Company { get; private set; }
    
    public IShoppingCartRepository ShoppingCart { get; private set; }
    public IApplicationUserRepository ApplicationUser { get; }

    private readonly ApplicationDbContext _context;
    
    public UnitOfWork( ApplicationDbContext context)
    {
        _context = context;
        ApplicationUser = new ApplicationUserRepository(_context);
        ShoppingCart =  new ShoppingCartRepository(_context);
        Category = new CategoryRepository(_context);
        Product = new ProductRepository(_context);
        Company = new CompanyRepository(_context);
    }

   
    public void Save()
    {
        _context.SaveChanges();
    }
}