using BookStore.Data;
using BookStore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Data;

namespace BookStore.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "SuperAdmin,Admin")]
    public class OrderController : Controller
    {
            private readonly DataContext _dataContext;

            public OrderController(DataContext dataContext)
            {
            _dataContext = dataContext;
            }

            public IActionResult Index()
            {
                List<Order> orders = _dataContext.Orders.ToList();
                return View(orders);
            }

            public IActionResult Detail(int id)
            {
                Order order = _dataContext.Orders.Include(x => x.OrderItems).FirstOrDefault(x => x.Id == id);
                if (order == null) return View("Error");


                return View(order);
            }

            public IActionResult Accept(int id)
            {
                Order order = _dataContext.Orders.FirstOrDefault(x => x.Id == id);
                if (order == null) return View("Error");

                order.OrderStatus = Enums.OrderStatus.Accepted;

            _dataContext.SaveChanges();

                return RedirectToAction("index");
            }

            public IActionResult Reject(int id)
            {
                Order order = _dataContext.Orders.FirstOrDefault(x => x.Id == id);
                if (order == null) return View("Error");

                order.OrderStatus = Enums.OrderStatus.Rejected;

            _dataContext.SaveChanges();

                return RedirectToAction("index");
            }
        public IActionResult Delivery(int id)
        {
            Order order = _dataContext.Orders.FirstOrDefault(x => x.Id == id);
            if (order == null) return View("Error");

            order.OrderStatus = Enums.OrderStatus.Delivery;

            _dataContext.SaveChanges();

            return RedirectToAction("index");
        }
        public IActionResult Completed(int id)
        {
            Order order = _dataContext.Orders.FirstOrDefault(x => x.Id == id);
            if (order == null) return View("Error");

            order.OrderStatus = Enums.OrderStatus.Completed;

            _dataContext.SaveChanges();

            return RedirectToAction("index");
        }
        public IActionResult AllOrders()
        {
            List<Order> orders = _dataContext.Orders.ToList();
            return View(orders);
        }
    }
    }

