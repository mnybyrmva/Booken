using BookStore.Data;
using BookStore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace BookStore.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "SuperAdmin,Admin")]
    public class SettingController : Controller
    {

        private readonly DataContext _dataContext;

        public SettingController(DataContext dataContext)
        {
            _dataContext = dataContext;
        }
        public IActionResult Index()
        {
            List<Setting> setting = _dataContext.Settings.ToList();
            var query = setting.AsQueryable();
            return View(query.ToList());
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(Setting setting)
        {

            if (!ModelState.IsValid) return View("Error"); 
            _dataContext.Settings.Add(setting);
            _dataContext.SaveChanges();
            return RedirectToAction("index");
        }
        public IActionResult Update(int id)
        {

            Setting setting = _dataContext.Settings.FirstOrDefault(x => x.Id == id);
            if (setting == null)  return View("Error"); 
            return View(setting);
        }
        [HttpPost]
        public IActionResult Update(Setting setting)
        {
            Setting existsetting = _dataContext.Settings.FirstOrDefault(x => x.Id == setting.Id);
            if (existsetting == null) return View("Error");
            existsetting.Key = setting.Key;
            existsetting.Value = setting.Value;
            _dataContext.SaveChanges();
            return RedirectToAction("index");
        }
    }
}
