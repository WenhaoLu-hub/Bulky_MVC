using BulkyBook.Models.Models;
using BulkyBook.DataAccess.Data;
using BulkyBook.DataAccess.Repository.IRepository;

namespace BulkyBook.DataAccess.Repository;

public class OrderDetailRepository :  Repository<OrderDetail>, IOrderDetailRepository
{
    private readonly ApplicationDbContext _context;
    public OrderDetailRepository(ApplicationDbContext context) : base(context)
    {
        _context = context;
    }


    public void Update(OrderDetail obj)
    {
        _context.Update(obj);
    }
}