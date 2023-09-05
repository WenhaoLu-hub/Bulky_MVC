using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models.Models;
using BulkyBook.Models.ViewModels;
using BulkyBook.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using Stripe.Checkout;

namespace BulkyBookWeb.Areas.Admin.Controllers;

[Area("admin")]
[Authorize]
public class OrderController : Controller
{
    private readonly IUnitOfWork _unitOfWork;

    [BindProperty]
    public OrderVM OrderVm { get; set; }

    public OrderController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Details(int orderId)
    {
        OrderVm = new OrderVM
        {
            OrderHeader = _unitOfWork.OrderHeader.Get(
                x => x.Id == orderId,
                includeProperties: "ApplicationUser"
            ),
            OrderDetail = _unitOfWork.OrderDetail.GetAll(
                x => x.OrderHeaderId == orderId,
                includeProperties: "Product"
            )
        };
        return View(OrderVm);
    }

    [HttpPost]
    [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
    public IActionResult UpdateOrderDetail()
    {
        var orderHeader = _unitOfWork.OrderHeader.Get(x => x.Id == OrderVm.OrderHeader.Id);
        orderHeader.Name = OrderVm.OrderHeader.Name;
        orderHeader.PhoneNumber = OrderVm.OrderHeader.PhoneNumber;
        orderHeader.City = OrderVm.OrderHeader.City;
        orderHeader.StreetAddress = OrderVm.OrderHeader.StreetAddress;
        orderHeader.State = OrderVm.OrderHeader.State;
        orderHeader.PostalCode = OrderVm.OrderHeader.PostalCode;
        if (!string.IsNullOrEmpty(OrderVm.OrderHeader.Carrier))
        {
            orderHeader.Carrier = OrderVm.OrderHeader.Carrier;
        }
        if (!string.IsNullOrEmpty(OrderVm.OrderHeader.TrackNumber))
        {
            orderHeader.TrackNumber = OrderVm.OrderHeader.TrackNumber;
        }
        _unitOfWork.OrderHeader.Update(orderHeader);
        _unitOfWork.Save();
        TempData["Success"] = "Order Details Updated Successfully";
        return RedirectToAction(nameof(Details), new { orderId = orderHeader.Id });
    }

    [HttpPost]
    [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
    public IActionResult StartProcessing()
    {
        _unitOfWork.OrderHeader.UpdateStatus(OrderVm.OrderHeader.Id, SD.StatusInProcess);
        _unitOfWork.Save();
        TempData["Success"] = "Order Status Update Successfully";
        return RedirectToAction(nameof(Details), new { orderId = OrderVm.OrderHeader.Id });
    }

    [HttpPost]
    [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
    public IActionResult ShipOrder()
    {
        var orderHeader = _unitOfWork.OrderHeader.Get(x => x.Id == OrderVm.OrderHeader.Id);
        orderHeader.TrackNumber = OrderVm.OrderHeader.TrackNumber;
        orderHeader.Carrier = OrderVm.OrderHeader.Carrier;
        orderHeader.OrderStatus = SD.StatusShipped;
        orderHeader.ShippingDate = DateTime.UtcNow;
        if (orderHeader.PaymentStatus == SD.PaymentStatusDelayedPayment)
        {
            orderHeader.PaymentDueDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(30));
        }
        _unitOfWork.OrderHeader.Update(orderHeader);
        _unitOfWork.Save();
        TempData["Success"] = "Order Status Update Successfully";
        return RedirectToAction(nameof(Details), new { orderId = OrderVm.OrderHeader.Id });
    }

    [HttpPost]
    [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
    public IActionResult CancelOrder()
    {
        var orderHeader = _unitOfWork.OrderHeader.Get(x => x.Id == OrderVm.OrderHeader.Id);

        if (orderHeader.PaymentStatus == SD.PaymentStatusApproved)
        {
            var options = new RefundCreateOptions
            {
                Reason = RefundReasons.RequestedByCustomer,
                PaymentIntent = orderHeader.PaymentIntentId
            };
            var refundService = new RefundService();
            var refund = refundService.Create(options);

            _unitOfWork.OrderHeader.UpdateStatus(
                orderHeader.Id,
                SD.StatusCancelled,
                SD.StatusRefunded
            );
        }
        else
        {
            _unitOfWork.OrderHeader.UpdateStatus(
                orderHeader.Id,
                SD.StatusCancelled,
                SD.StatusRefunded
            );
        }
        _unitOfWork.Save();
        TempData["Success"] = "Order Status Update Successfully";
        return RedirectToAction(nameof(Details), new { orderId = OrderVm.OrderHeader.Id });
    }

    [HttpPost]
    [ActionName("Details")]
    [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
    public IActionResult Details_PAY_NOW()
    {
        OrderVm.OrderHeader = _unitOfWork.OrderHeader.Get(
            x => x.Id == OrderVm.OrderHeader.Id,
            includeProperties: "ApplicationUser"
        );
        OrderVm.OrderDetail = _unitOfWork.OrderDetail.GetAll(
            x => x.OrderHeaderId == OrderVm.OrderHeader.Id,
            includeProperties: "Product"
        );

        //stripe logic
        
        var domain = Request.Scheme+"://"+Request.Host.Value+"/";
        var createOptions = new SessionCreateOptions
        {
            SuccessUrl =
                domain + $"Admin/Order/PaymentConfirmation?orderHeaderId={OrderVm.OrderHeader.Id}",
            CancelUrl = domain + $"Admin/Order/Details?orderId={OrderVm.OrderHeader.Id}",
            LineItems = new List<SessionLineItemOptions>(),
            Mode = "payment"
        };
        foreach (var item in OrderVm.OrderDetail)
        {
            var sessionLineItemOptions = new SessionLineItemOptions
            {
                PriceData = new SessionLineItemPriceDataOptions
                {
                    UnitAmount = (long)(item.Price * 100),
                    Currency = "usd",
                    ProductData = new SessionLineItemPriceDataProductDataOptions
                    {
                        Name = item.Product.Title
                    },
                },
                Quantity = item.Count
            };
            createOptions.LineItems.Add(sessionLineItemOptions);
        }

        var sessionService = new SessionService();
        var session = sessionService.Create(createOptions);
        _unitOfWork.OrderHeader.UpdateStripePaymentId(
            OrderVm.OrderHeader.Id,
            session.Id,
            session.PaymentIntentId
        );
        _unitOfWork.Save();
        Response.Headers.Add("Location", session.Url);
        return new StatusCodeResult(303);
    }

    public IActionResult PaymentConfirmation(int orderHeaderId)
    {
        var orderHeader = _unitOfWork.OrderHeader.Get(x => x.Id == orderHeaderId);
        if (orderHeader.PaymentStatus == SD.PaymentStatusDelayedPayment)
        {
            // customer
            var service = new SessionService();
            var session = service.Get(orderHeader.SessionId);
            if (session.PaymentStatus.ToLower() == "paid")
            {
                _unitOfWork.OrderHeader.UpdateStripePaymentId(
                    orderHeaderId,
                    session.Id,
                    session.PaymentIntentId
                );
                _unitOfWork.OrderHeader.UpdateStatus(
                    orderHeaderId,
                    orderHeader.OrderStatus,
                    SD.PaymentStatusApproved
                );
                _unitOfWork.Save();
            }
        }

        return View(orderHeaderId);
    }

    #region API CALLS

    [HttpGet]
    public IActionResult GetAll(string status)
    {
        IEnumerable<OrderHeader> orderHeaders;
        if (User.IsInRole(SD.Role_Admin) || User.IsInRole(SD.Role_Employee))
        {
            orderHeaders = _unitOfWork.OrderHeader
                .GetAll(includeProperties: "ApplicationUser")
                .ToList();
        }
        else
        {
            var userIdentity = (ClaimsIdentity)User.Identity;
            var userId = userIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            orderHeaders = _unitOfWork.OrderHeader
                .GetAll(x => x.ApplicationUserId == userId, includeProperties: "ApplicationUser")
                .ToList();
        }
        switch (status)
        {
            case "pending":
                orderHeaders = orderHeaders.Where(x => x.OrderStatus == SD.StatusPending).ToList();
                break;
            case "inprocess":
                orderHeaders = orderHeaders
                    .Where(x => x.OrderStatus == SD.StatusInProcess)
                    .ToList();
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
