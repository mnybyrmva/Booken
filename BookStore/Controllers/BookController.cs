using BookStore.Data;
using BookStore.Models;
using BookStore.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace BookStore.Controllers
{
    public class BookController : Controller
    {
        private DataContext _datacontext;
        private readonly UserManager<AppUser> _userManager;

        public BookController(DataContext dataContext, UserManager<AppUser> userManager)
        {
            _datacontext = dataContext;
            _userManager = userManager;
        }
		public async Task<IActionResult> Card()
		{
            AppUser member = null;
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                member = await _userManager.FindByNameAsync(HttpContext.User.Identity.Name);
            }
            List<BasketItem> basketItems = null;

            if (member == null)
            {
                string basketItemsStr = HttpContext.Request.Cookies["BasketItems"];
                if (basketItemsStr != null)
                {
                    basketItems = JsonConvert.DeserializeObject<List<BasketItem>>(basketItemsStr);
                }
                foreach (BasketItem basketItem in basketItems)
                {
                    basketItem.Book = _datacontext.Books.Include(x=>x.bookImages).Include(x => x.Author).Include(x => x.Category).FirstOrDefault(x => x.Id == basketItem.BookId);
                }
            }
            else
            {
                basketItems = _datacontext.BasketItems.Include(x => x.Book).Where(x => x.AppUserId == member.Id).ToList();
            }
            return View(basketItems);
		}
        public IActionResult Detail(int id)
        {
            Book book = _datacontext.Books
                            .Include(x => x.Author)
                            .Include(x => x.Category)
                            .Include(x => x.bookImages)
                            .FirstOrDefault(x => x.Id == id);

            if (book == null) return View("Error");

            BookViewModel bookVM = new BookViewModel
            {
                Book = book,
                Books = _datacontext.Books
                            .Include(x => x.Author)
                            .Include(x => x.Category)
                            .Include(x => x.bookImages)
                            .Where(x => x.DiscountPrice > 0).ToList()
            };

            return View(bookVM);
        }

        public async Task<IActionResult> AddtoBasket(int bookId)
        {
            if (!_datacontext.Books.Any(x => x.Id == bookId)) return View("Error");
            List<BasketViewModel> basketitems = new List<BasketViewModel>();
            BasketViewModel basket = null;
            AppUser member = null;
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                member = await _userManager.FindByNameAsync(HttpContext.User.Identity.Name);
            }
            string basketItemsStr = HttpContext.Request.Cookies["BasketItems"];
            if (member == null)
            {
                if (basketItemsStr != null)
                {
                    basketitems = JsonConvert.DeserializeObject<List<BasketViewModel>>(basketItemsStr);
                    basket = basketitems.FirstOrDefault(x => x.BookId == bookId);
                    if (basket != null) basket.count++;
                    else
                    {
                        basket = new BasketViewModel
                        {
                            BookId = bookId,
                            count = 1

                        };
                        basketitems.Add(basket);
                    }
                }
                else
                {
                    basket = new BasketViewModel
                    {
                        BookId = bookId,
                        count = 1
                    };
                    basketitems.Add(basket);
                }
                basketItemsStr = JsonConvert.SerializeObject(basketitems);
                HttpContext.Response.Cookies.Append("BasketItems", basketItemsStr);
            }
            else
            {
                BasketItem memberBasketItem = _datacontext.BasketItems.FirstOrDefault(x => x.AppUserId == member.Id && x.BookId == bookId);
                if (memberBasketItem != null)
                {
                    memberBasketItem.Count++;
                }
                else
                {
                    memberBasketItem = new BasketItem
                    {
                        AppUserId = member.Id,
                        BookId = bookId,
                        Count = 1
                    };
                    _datacontext.BasketItems.Add(memberBasketItem);
                }
                await _datacontext.SaveChangesAsync();
            }


            return Ok();
        }
        public IActionResult GetBasket()
        {
            List<BasketViewModel> basketitems = new List<BasketViewModel>();
            string basketItemsStr = HttpContext.Request.Cookies["BasketItems"];
            if (basketItemsStr != null)
            {
                basketitems = JsonConvert.DeserializeObject<List<BasketViewModel>>(basketItemsStr);
            }
            return Json(basketitems);
        }
		public async Task<IActionResult> Checkout()
		{
			AppUser member = null;
			if (HttpContext.User.Identity.IsAuthenticated)
			{
				member = await _userManager.FindByNameAsync(HttpContext.User.Identity.Name);
			}
			List<BasketViewModel> basketItems = new List<BasketViewModel>();
			List<CheckoutItemViewModel> checkoutItems = new List<CheckoutItemViewModel>();
			CheckoutItemViewModel checkoutItem = null;
			List<BasketItem> memberBasketItems = null;
			OrderViewModel orderViewModel = null;
			string basketItemsStr = HttpContext.Request.Cookies["BasketItems"];

			if (member == null)
			{
				if (basketItemsStr != null)
				{
					basketItems = JsonConvert.DeserializeObject<List<BasketViewModel>>(basketItemsStr);

					foreach (var item in basketItems)
					{
						checkoutItem = new CheckoutItemViewModel
						{
							Book = _datacontext.Books.FirstOrDefault(x => x.Id == item.BookId),
							Count = item.count
						};
						checkoutItems.Add(checkoutItem);
						
					}
				}
			}
			else
			{
				memberBasketItems = _datacontext.BasketItems.Include(x => x.Book).Where(x => x.AppUserId == member.Id).ToList();

				foreach (var item in memberBasketItems)
				{
					if (!item.IsDeleted)
					{
						checkoutItem = new CheckoutItemViewModel
						{
							Book = item.Book,
							Count = item.Count
						};
						checkoutItems.Add(checkoutItem);
					}
				}

			}

			orderViewModel = new OrderViewModel
			{
				CheckoutItemViewModels = checkoutItems,
				FullName = member?.FullName,
				Email = member?.Email,
				Phone = member?.PhoneNumber
			};
			
				
            return View(orderViewModel);
		}
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Order(OrderViewModel orderVM)
		{
			List<BasketViewModel> basketItems = new List<BasketViewModel>();
			List<CheckoutItemViewModel> checkoutItems = new List<CheckoutItemViewModel>();
			CheckoutItemViewModel checkoutItem = null;
			List<BasketItem> memberBasketItems = null;
			OrderItem orderItem = null;
			double totalPrice = 0;
			string basketItemsStr = HttpContext.Request.Cookies["BasketItems"];
            if (!ModelState.IsValid) return View("Error");
            AppUser member = null;
			if (HttpContext.User.Identity.IsAuthenticated)
			{
				member = await _userManager.FindByNameAsync(HttpContext.User.Identity.Name);
			}
			Order order = null;

			order = new Order
			{
				FullName = orderVM.FullName,
				Country = orderVM.Country,
				Adress = orderVM.Adress,
				City = orderVM.City,
				Email = orderVM.Email,
				Note = orderVM.Note,
				Phone = orderVM.Phone,
				ZipCode = orderVM.ZipCode,
				OrderStatus = Enums.OrderStatus.Pending,
				CreateDate = DateTime.UtcNow.AddHours(4),
				AppUserId = member?.Id
			};

			if (member == null)
			{
				if (basketItemsStr != null)
				{
					basketItems = JsonConvert.DeserializeObject<List<BasketViewModel>>(basketItemsStr);

					foreach (var item in basketItems)
					{
						Book book = _datacontext.Books.FirstOrDefault(x => x.Id == item.BookId);
						orderItem = new OrderItem
						{
							Book = book,
							BookName = book.Name,
							CostPrice = book.CostPrice,
							DiscountPrice = book.DiscountPrice,
							SalePrice = (book.SalePrice * (1 - (book.DiscountPrice / 100))),
							Count = item.count,
							Order = order
						};
						totalPrice += orderItem.SalePrice * orderItem.Count;
						order.OrderItems.Add(orderItem);
                        

                    }
					

                }
                if (Request.Cookies["BasketItems"] != null)
                {
                    Response.Cookies.Delete("BasketItems");
                }
            }
			else
			{
				memberBasketItems = _datacontext.BasketItems.Include(x => x.Book).Where(x => x.AppUserId == member.Id).ToList();

				foreach (var item in memberBasketItems)
				{
					Book book = _datacontext.Books.FirstOrDefault(x => x.Id == item.BookId);
					orderItem = new OrderItem
					{
						Book = book,
						BookName = book.Name,
						CostPrice = book.CostPrice,
						DiscountPrice = book.DiscountPrice,
						SalePrice = (book.SalePrice * (1 - (book.DiscountPrice / 100))),
						Count = item.Count,
						Order = order,
						
					};
					totalPrice += orderItem.SalePrice * orderItem.Count;
					order.OrderItems.Add(orderItem);
				}
				foreach(var item in memberBasketItems)
				{
				_datacontext.BasketItems.Remove(item);
				}
				_datacontext.SaveChanges();

            }
			order.TotalPrice = totalPrice;
			
			
			_datacontext.Orders.Add(order);
			_datacontext.SaveChanges();
            return RedirectToAction("index", "Home");
		}
		public IActionResult HardDelete(int id)
		{
			BasketItem basketItem  = _datacontext.BasketItems.Find(id);
			if (basketItem is null) return View("Error");
            _datacontext.BasketItems.Remove(basketItem);
			_datacontext.SaveChanges();
			return RedirectToAction("Card","Book");
		}
	}
}
