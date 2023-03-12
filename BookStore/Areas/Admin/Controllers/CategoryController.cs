using BookStore.Data;
using BookStore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace BookStore.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "SuperAdmin,Admin")]
    public class CategoryController : Controller
    {
        private readonly DataContext _dataContext;
        public CategoryController(DataContext dataContext)
        {
            _dataContext = dataContext;
        }
        public IActionResult Index()
        {
            List<Category> categorys = _dataContext.Categories.ToList();
            List<Category> categorysList = new List<Category>();
            foreach (Category category in categorys)
            {
                if (category.IsDeleted == false)
                {
                    categorysList.Add(category);
                }
            }
            var query = categorysList.AsQueryable();
            return View(query.ToList());
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(Category category)
        {
            _dataContext.Categories.Add(category);
            _dataContext.SaveChanges();
            return RedirectToAction("index");
        }
        
        public IActionResult Edit(int id)
        {
            Category category = _dataContext.Categories.Find(id);
            return View(category);
        }
        [HttpPost]
        public IActionResult Edit(Category newcategory)
        {
            Category existcategory = _dataContext.Categories.Find(newcategory.Id);
            if (existcategory is null) return NotFound();
            existcategory.Name = newcategory.Name;
            _dataContext.SaveChanges();
            return RedirectToAction("index");
        }
        [HttpPost]
        public IActionResult HardDelete(int id)
        {
            Category category = _dataContext.Categories.Find(id);
            if (category is null) return NotFound();
            return View(category);
        }
        public IActionResult HardDelete(Category category)
        {
            _dataContext.Categories.Remove(category);
            _dataContext.SaveChanges();
            return RedirectToAction("index");
        }
        public IActionResult SoftDelete(int id)
        {
            Category category = _dataContext.Categories.Find(id);
            if (category is null) return NotFound();
            category.IsDeleted = true;
            _dataContext.SaveChanges();
            return RedirectToAction("index");
        }
    }
}
