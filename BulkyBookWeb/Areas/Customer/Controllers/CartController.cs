using System.Security.Claims;
using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models.Models;
using BulkyBook.Models.ViewModels;
using BulkyBook.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe.Checkout;

namespace BulkyBookWeb.Areas.Customer.Controllers;

[Area("Customer")]
[Authorize]
public class CartController : Controller
{
    private readonly IUnitOfWork _unitOfWork;

    public CartController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    [BindProperty]
    public ShoppingCartVM ShoppingCartVm { get; set; }

    public IActionResult Index()
    {
        var claimsIdentity = (ClaimsIdentity)User.Identity;
        var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
        ShoppingCartVm = new ShoppingCartVM
        {
            ShoppingCartList = _unitOfWork.ShoppingCart.GetAll(
                x => x.ApplicationUserId == userId,
                includeProperties: "Product"
            ),
            OrderHeader = new()
        };
        foreach (var cart in ShoppingCartVm.ShoppingCartList)
        {
            cart.Price = GetPriceBasedOnQuantity(cart);
            ShoppingCartVm.OrderHeader.OrderTotal += cart.Price * cart.Count;
        }

        return View(ShoppingCartVm);
    }

    public IActionResult Plus(int cartId)
    {
        var shoppingCart = _unitOfWork.ShoppingCart.Get(x => x.Id == cartId);
        shoppingCart.Count += 1;
        _unitOfWork.ShoppingCart.Update(shoppingCart);
        _unitOfWork.Save();
        return RedirectToAction(nameof(Index));
    }

    public IActionResult Minus(int cartId)
    {
        var shoppingCart = _unitOfWork.ShoppingCart.Get(x => x.Id == cartId, tracked: true);
        if (shoppingCart.Count <= 1)
        {
            HttpContext.Session.SetInt32(
                SD.SessionCart,
                _unitOfWork.ShoppingCart
                    .GetAll(x => x.ApplicationUserId == shoppingCart.ApplicationUserId)
                    .Count() - 1
            );
            _unitOfWork.ShoppingCart.Remove(shoppingCart);
        }
        else
        {
            shoppingCart.Count -= 1;
            _unitOfWork.ShoppingCart.Update(shoppingCart);
        }

        _unitOfWork.Save();
        return RedirectToAction(nameof(Index));
    }

    public IActionResult Remove(int cartId)
    {
        var shoppingCart = _unitOfWork.ShoppingCart.Get(x => x.Id == cartId, tracked: true);
        _unitOfWork.ShoppingCart.Remove(shoppingCart);
        HttpContext.Session.SetInt32(
            SD.SessionCart,
            _unitOfWork.ShoppingCart
                .GetAll(x => x.ApplicationUserId == shoppingCart.ApplicationUserId)
                .Count() - 1
        );
        _unitOfWork.Save();
        TempData["Success"] = "Item removed from cart successfully";
        return RedirectToAction(nameof(Index));
    }

    public IActionResult Summary()
    {
        var claimsIdentity = (ClaimsIdentity)User.Identity;
        var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
        ShoppingCartVm = new ShoppingCartVM
        {
            ShoppingCartList = _unitOfWork.ShoppingCart.GetAll(
                x => x.ApplicationUserId == userId,
                includeProperties: "Product"
            ),
            OrderHeader = new()
        };
        ShoppingCartVm.OrderHeader.ApplicationUser = _unitOfWork.ApplicationUser.Get(
            x => x.Id == userId
        );
        ShoppingCartVm.OrderHeader.Name = ShoppingCartVm.OrderHeader.ApplicationUser.Name;
        ShoppingCartVm.OrderHeader.City = ShoppingCartVm.OrderHeader.ApplicationUser.City;
        ShoppingCartVm.OrderHeader.State = ShoppingCartVm.OrderHeader.ApplicationUser.State;
        ShoppingCartVm.OrderHeader.StreetAddress = ShoppingCartVm
            .OrderHeader
            .ApplicationUser
            .StreetAddress;
        ShoppingCartVm.OrderHeader.PhoneNumber = ShoppingCartVm
            .OrderHeader
            .ApplicationUser
            .PhoneNumber;
        ShoppingCartVm.OrderHeader.PostalCode = ShoppingCartVm
            .OrderHeader
            .ApplicationUser
            .PostalCode;

        foreach (var cart in ShoppingCartVm.ShoppingCartList)
        {
            cart.Price = GetPriceBasedOnQuantity(cart);
            ShoppingCartVm.OrderHeader.OrderTotal += cart.Price * cart.Count;
        }

        return View(ShoppingCartVm);
    }

    [HttpPost]
    [ActionName(nameof(Summary))]
    public IActionResult SummaryPost()
    {
        var claimsIdentity = (ClaimsIdentity)User.Identity;
        var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
        ShoppingCartVm.ShoppingCartList = _unitOfWork.ShoppingCart.GetAll(
            x => x.ApplicationUserId == userId,
            includeProperties: "Product"
        );
        ShoppingCartVm.OrderHeader.OrderDate = DateTime.UtcNow;
        ShoppingCartVm.OrderHeader.ApplicationUserId = userId;
        ApplicationUser applicationUser = _unitOfWork.ApplicationUser.Get(x => x.Id == userId);

        foreach (var cart in ShoppingCartVm.ShoppingCartList)
        {
            cart.Price = GetPriceBasedOnQuantity(cart);
            ShoppingCartVm.OrderHeader.OrderTotal += cart.Price * cart.Count;
        }

        if (applicationUser.CompanyId.GetValueOrDefault() == 0)
        {
            //regular customer
            ShoppingCartVm.OrderHeader.PaymentStatus = SD.PaymentStatusPending;
            ShoppingCartVm.OrderHeader.OrderStatus = SD.StatusPending;
        }
        else
        {
            ShoppingCartVm.OrderHeader.PaymentStatus = SD.PaymentStatusDelayedPayment;
            ShoppingCartVm.OrderHeader.OrderStatus = SD.StatusApproved;
        }

        _unitOfWork.OrderHeader.Add(ShoppingCartVm.OrderHeader);
        _unitOfWork.Save();
        foreach (var cart in ShoppingCartVm.ShoppingCartList)
        {
            OrderDetail orderDetail = new OrderDetail
            {
                ProductId = cart.ProductId,
                Count = cart.Count,
                OrderHeaderId = ShoppingCartVm.OrderHeader.Id,
                Price = cart.Price
            };
            _unitOfWork.OrderDetail.Add(orderDetail);
            _unitOfWork.Save();
        }

        if (applicationUser.CompanyId.GetValueOrDefault() == 0)
        {
            //regular customer
            var domain = Request.Scheme + "://"+Request.Host.Value+"/";
            var options = new SessionCreateOptions
            {
                SuccessUrl =
                    domain + $"customer/cart/OrderConfirmation?id={ShoppingCartVm.OrderHeader.Id}",
                CancelUrl = domain + $"customer/cart/index",
                LineItems = new List<SessionLineItemOptions>(),
                Mode = "payment",
            };
            foreach (var item in ShoppingCartVm.ShoppingCartList)
            {
                var sessionLineItem = new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        UnitAmount = (long)(item.Price * 100),
                        Currency = "usd",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = item.Product.Title
                        }
                    },
                    Quantity = item.Count
                };
                options.LineItems.Add(sessionLineItem);
            }

            var service = new SessionService();
            Session session = service.Create(options);
            _unitOfWork.OrderHeader.UpdateStripePaymentId(
                ShoppingCartVm.OrderHeader.Id,
                session.Id,
                session.PaymentIntentId
            );
            _unitOfWork.Save();
            Response.Headers.Add("Location", session.Url);
            return new StatusCodeResult(303);
        }

        return RedirectToAction(
            nameof(OrderConfirmation),
            new { id = ShoppingCartVm.OrderHeader.Id }
        );
    }

    public IActionResult OrderConfirmation(int id)
    {
        var orderHeader = _unitOfWork.OrderHeader.Get(
            x => x.Id == id,
            includeProperties: "ApplicationUser"
        );
        if (orderHeader.PaymentStatus != SD.PaymentStatusDelayedPayment)
        {
            // customer
            var service = new SessionService();
            var session = service.Get(orderHeader.SessionId);
            if (session.PaymentStatus.ToLower() == "paid")
            {
                _unitOfWork.OrderHeader.UpdateStripePaymentId(
                    id,
                    session.Id,
                    session.PaymentIntentId
                );
                _unitOfWork.OrderHeader.UpdateStatus(
                    id,
                    SD.StatusApproved,
                    SD.PaymentStatusApproved
                );
                _unitOfWork.Save();
            }
            HttpContext.Session.Clear();
            
        }

        var shoppingCarts = _unitOfWork.ShoppingCart
            .GetAll(x => x.ApplicationUserId == orderHeader.ApplicationUserId)
            .ToList();
        _unitOfWork.ShoppingCart.RemoveRange(shoppingCarts);
        _unitOfWork.Save();
        return View(id);
    }

    private double GetPriceBasedOnQuantity(ShoppingCart shoppingCart)
    {
        var count = shoppingCart.Count;
        switch (count)
        {
            case <= 50:
                return shoppingCart.Product.ListPrice;
            case <= 100:
                return shoppingCart.Product.ListPrice50;
            default:
                return shoppingCart.Product.ListPrice100;
        }
    }
}
