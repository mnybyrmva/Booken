using BookStore.Data;
using BookStore.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.Controllers
{
    public class ContactController : Controller
    {
        private DataContext _dataContext;
        public ContactController(DataContext dataContext)
        {
            _dataContext = dataContext;
        }
        public IActionResult Index()
        {
            HomeViewModel HomeVM = new HomeViewModel()
            {
                settings = _dataContext.Settings.ToList(),
            };
            return View(HomeVM);
        }
    }
}
