using BulkyBook.Models.Models;

namespace BulkyBook.Models.ViewModels;

public class OrderVM
{
    public OrderHeader OrderHeader { get; set; }
    public IEnumerable<OrderDetail>  OrderDetail { get; set; }
}