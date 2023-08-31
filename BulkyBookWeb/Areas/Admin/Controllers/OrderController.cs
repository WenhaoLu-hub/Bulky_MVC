using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models.Models;
using BulkyBook.Models.ViewModels;
using BulkyBook.Utility;
using Microsoft.AspNetCore.Mvc;

namespace BulkyBookWeb.Areas.Admin.Controllers;

[Area("admin")]
public class OrderController : Controller
{
    private readonly IUnitOfWork _unitOfWork;

    public OrderController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public IActionResult Index()
    {
        return View();
    }

    // public IActionResult Details(int orderId)
    // {
    //     new OrderVM
    //     {
    //         OrderHeader = _unitOfWork.OrderHeader.Get(x=>x.Id==orderId, includeProperties:"ApplicationUser"),
    //         OrderDetail = _unitOfWork.OrderDetail.GetAll(x=>x.OrderHeader.Id == orderId, includeProperties:"Product")
    //         
    //     };
    //     return View();
    // }

    #region API CALLS

    [HttpGet]
    public IActionResult GetAll(string status)
    {
        IEnumerable<OrderHeader> orderHeaders = _unitOfWork.OrderHeader.GetAll(includeProperties:"ApplicationUser").ToList();
        switch (status)
        {
            case "pending":
                orderHeaders = orderHeaders.Where(x => x.OrderStatus == SD.StatusPending  ).ToList();
                break;
            case "inprocess":
                orderHeaders = orderHeaders.Where(x => x.OrderStatus == SD.StatusInProcess).ToList();
                break;
            case "completed":
                orderHeaders = orderHeaders.Where(x => x.OrderStatus == SD.StatusShipped).ToList();
                break;
            case "approved":
                orderHeaders = orderHeaders.Where(x => x.OrderStatus == SD.StatusApproved).ToList();
                break;
            default:
                break;
        }
        return Json(new { data = orderHeaders });
    }
    

    #endregion
}