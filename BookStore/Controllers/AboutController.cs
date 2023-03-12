using BookStore.Data;
using BookStore.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookStore.Controllers
{
    public class AboutController : Controller
    {
        private DataContext _dataContext;
        public AboutController(DataContext dataContext)
        {
            _dataContext = dataContext;
        }
        public IActionResult Index()
        {
            ViewBag.Authors = _dataContext.Authors.ToList();
            ViewBag.Categories = _dataContext.Categories.ToList();
            ViewBag.Settings = _dataContext.Settings.ToList();
            HomeViewModel HomeVM = new HomeViewModel
            {
                Sliders = _dataContext.Sliders.ToList(),
                FeaturedBooks = _dataContext.Books.Include(x => x.Author).Include(x => x.bookImages).Where(x => x.IsFeatured == true).ToList(),
                NewBooks = _dataContext.Books.Include(x => x.Author).Include(x => x.bookImages).Where(x => x.IsNew == true).ToList(),
                DiscountedBooks = _dataContext.Books.Include(x => x.Author).Include(x => x.bookImages).Where(x => x.DiscountPrice > 0).ToList(),
                categories = _dataContext.Categories.ToList(),
                authors = _dataContext.Authors.ToList(),
                bookImages = _dataContext.BookImages.ToList(),
                titles = _dataContext.Titles.ToList(),
                testimonials = _dataContext.Testimonials.ToList(),
                texts = _dataContext.Texts.ToList(),
                teams = _dataContext.Teams.ToList(),
                positions = _dataContext.Positions.ToList(),
                settings = _dataContext.Settings.ToList(),
                images = _dataContext.Images.ToList(),
                books = _dataContext.Books.Include(x => x.Author).Include(x => x.bookImages).ToList(),


            };
            return View(HomeVM);
        }
    }
}
