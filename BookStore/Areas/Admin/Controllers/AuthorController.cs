using BookStore.Data;
using BookStore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace BookStore.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "SuperAdmin,Admin")]
    public class AuthorController : Controller
    {
        private readonly DataContext _dataContext;
        public AuthorController(DataContext dataContext)
        {
            _dataContext= dataContext;
        }
        public IActionResult Index()
        {
            List<Author> authors = _dataContext.Authors.ToList();
            List<Author> authorsList = new List<Author>();
            foreach (Author author in authors)
            {
                if (author.IsDeleted == false)
                {
                    authorsList.Add(author);
                }
            }
            var query = authorsList.AsQueryable();
            return View(query.ToList());
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(Author author)
        {
            _dataContext.Authors.Add(author);
            _dataContext.SaveChanges();
            return RedirectToAction("index");
        }
        public IActionResult Edit(int id)
        {
            Author author = _dataContext.Authors.Find(id);
            return View(author);
        }
        [HttpPost]
        public IActionResult Edit(Author newauthor)
        {
            Author existauthor = _dataContext.Authors.Find(newauthor.Id);
            if (existauthor is null) return NotFound();
            existauthor.FullName = newauthor.FullName;
            _dataContext.SaveChanges();
            return RedirectToAction("index");
        }
        public IActionResult HardDelete(int id)
        {
            Author author = _dataContext.Authors.Find(id);
            if (author is null) return NotFound();
            _dataContext.Authors.Remove(author);
            _dataContext.SaveChanges();
            return Ok();

        }
        public IActionResult SoftDelete(int id)
        {
            Author author = _dataContext.Authors.Find(id);
            if (author is null) return NotFound();
            author.IsDeleted = true;
            _dataContext.SaveChanges();
            return RedirectToAction("index");
        }
    }
}
