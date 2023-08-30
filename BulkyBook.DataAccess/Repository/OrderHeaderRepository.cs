using BulkyBook.Models.Models;
using BulkyBook.DataAccess.Data;
using BulkyBook.DataAccess.Repository.IRepository;

namespace BulkyBook.DataAccess.Repository;

public class OrderHeaderRepository :  Repository<OrderHeader>, IOrderHeaderRepository
{
    private readonly ApplicationDbContext _context;
    public OrderHeaderRepository(ApplicationDbContext context) : base(context)
    {
        _context = context;
    }


    public void Update(OrderHeader obj)
    {
        _context.Update(obj);
    }
}