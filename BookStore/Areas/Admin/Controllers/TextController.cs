using BookStore.Data;
using BookStore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace BookStore.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "SuperAdmin,Admin")]
    public class TextController : Controller
    {
        private readonly DataContext _dataContext;
        public TextController(DataContext dataContext)
        {
            _dataContext = dataContext;
        }
        public IActionResult Index()
        {
            List<Text> texts = _dataContext.Texts.ToList();
            List<Text> textsList = new List<Text>();
            foreach (Text text in texts)
            {
                if (text.IsDeleted == false)
                {
                    textsList.Add(text);
                }
            }
            var query = textsList.AsQueryable();
            return View(query.ToList());
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(Text text)
        {
            _dataContext.Add(text);
            _dataContext.SaveChanges();
            return RedirectToAction("index");
        }

        public IActionResult Edit(int id)
        {
            Text text = _dataContext.Texts.Find(id);
            return View(text);
        }
        [HttpPost]
        public IActionResult Edit(Text newtext)
        {
            Text existtext = _dataContext.Texts.Find(newtext.Id);
            if (existtext is null) return NotFound();
            existtext.Name = newtext.Name;
            existtext.Desc = newtext.Desc;
            existtext.VideoUrl = newtext.VideoUrl;
            _dataContext.SaveChanges();
            return RedirectToAction("index");
        }
        [HttpPost]
        public IActionResult HardDelete(int id)
        {
            Text text = _dataContext.Texts.Find(id);
            if (text is null) return NotFound();
            return View(text);
        }
        public IActionResult HardDelete(Text text)
        {
            _dataContext.Texts.Remove(text);
            _dataContext.SaveChanges();
            return Ok();
        }
        public IActionResult SoftDelete(int id)
        {
            Text text = _dataContext.Texts.Find(id);
            if (text is null) return NotFound();
            text.IsDeleted = true;
            _dataContext.SaveChanges();
            return RedirectToAction("index");
        }
    }
}
