using BulkyBook.DataAccess.Data;
using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models.Models;

namespace BulkyBook.DataAccess.Repository;

public class ProductRepository : Repository<Product>, IProductRepository
{
    private readonly ApplicationDbContext _context;
    public ProductRepository(ApplicationDbContext context) : base(context)
    {
        _context = context;
    }

    public void Update(Product product)
    {
        _context.Update(product);
        var objFromDb = _context.Products.FirstOrDefault(x => x.ProductId == product.ProductId);
        if (objFromDb != null) 
        {
            objFromDb.Title = product.Title;
            objFromDb.Description = product.Description;
            objFromDb.Author = product.Author;
            objFromDb.ISBN = product.ISBN;
            objFromDb.ListPrice = product.ListPrice;
            objFromDb.ListPrice50 = product.ListPrice50;
            objFromDb.ListPrice100 = product.ListPrice100;
            objFromDb.CategoryId = product.CategoryId;
            if (product.ImageUrl != null)
            {
                objFromDb.ImageUrl = product.ImageUrl; 
            }
        }
    }
}