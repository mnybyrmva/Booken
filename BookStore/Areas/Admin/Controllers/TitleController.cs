using BookStore.Data;
using BookStore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace BookStore.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "SuperAdmin,Admin")]
    public class TitleController : Controller
    {
        private readonly DataContext _dataContext;
        public TitleController(DataContext dataContext)
        {
            _dataContext = dataContext;
        }
        public IActionResult Index()
        {
            List<Title> titles = _dataContext.Titles.ToList();
            List<Title> titlesList = new List<Title>();
            foreach (Title title in titles)
            {
                if (title.IsDeleted == false)
                {
                    titlesList.Add(title);
                }
            }
            var query = titlesList.AsQueryable();
            return View(query.ToList());
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(Title title)
        {
            _dataContext.Add(title);
            _dataContext.SaveChanges();
            return RedirectToAction("index");
        }

        public IActionResult Edit(int id)
        {
            Title title = _dataContext.Titles.Find(id);
            return View(title);
        }
        [HttpPost]
        public IActionResult Edit(Title newtitle)
        {
            Title existtitle = _dataContext.Titles.Find(newtitle.Id);
            if (existtitle is null) return NotFound();
            existtitle.Name = newtitle.Name;
            existtitle.Description = newtitle.Description;
            existtitle.Text = newtitle.Text;
            _dataContext.SaveChanges();
            return RedirectToAction("index");
        }
       
        
    }
}
