using BulkyBook.DataAccess.Data;
using BulkyBook.DataAccess.Repository.IRepository;

namespace BulkyBook.DataAccess.Repository;

public class UnitOfWork : IUnitOfWork
{
    
    public ICategoryRepository Category { get; private set; }
    public IProductRepository Product { get; private set; }
    
    public ICompanyRepository Company { get; private set; }
    
    private readonly ApplicationDbContext _context;
    
    public UnitOfWork( ApplicationDbContext context)
    {
        _context = context;
        Category = new CategoryRepository(_context);
        Product = new ProductRepository(_context);
        Company = new CompanyRepository(_context);
    }

   
    public void Save()
    {
        _context.SaveChanges();
    }
}