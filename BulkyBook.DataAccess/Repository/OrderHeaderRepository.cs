using BulkyBook.Models.Models;
using BulkyBook.DataAccess.Data;
using BulkyBook.DataAccess.Repository.IRepository;

namespace BulkyBook.DataAccess.Repository;

public class OrderHeaderRepository : Repository<OrderHeader>, IOrderHeaderRepository
{
    private readonly ApplicationDbContext _context;

    public OrderHeaderRepository(ApplicationDbContext context)
        : base(context)
    {
        _context = context;
    }

    public void Update(OrderHeader obj)
    {
        _context.Update(obj);
    }

    public void UpdateStatus(int id, string orderStatus, string? paymentStatus = null)
    {
        var orderHeader = _context.OrderHeaders.FirstOrDefault(x => x.Id == id);
        if (orderHeader != null)
        {
            orderHeader.OrderStatus = orderStatus;
            if (!string.IsNullOrEmpty(paymentStatus))
            {
                orderHeader.PaymentStatus = paymentStatus;
            }
        }
    }

    public void UpdateStripePaymentId(int id, string sessionId, string paymentIntentId)
    {
        var orderHeader = _context.OrderHeaders.FirstOrDefault(x => x.Id == id);
        if (!string.IsNullOrEmpty(sessionId))
        {
            orderHeader.SessionId = sessionId;
        }
        if (!string.IsNullOrEmpty(paymentIntentId))
        {
            orderHeader.PaymentIntentId = paymentIntentId;
            orderHeader.PaymentDate = DateTime.UtcNow;
        }
    }
}
