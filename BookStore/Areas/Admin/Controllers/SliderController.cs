using BookStore.Data;
using BookStore.Helper;
using BookStore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace BookStore.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "SuperAdmin,Admin")]
    public class SliderController : Controller
    {
        private DataContext _dataContext;
        private IWebHostEnvironment _env;
        public SliderController(DataContext dataContext, IWebHostEnvironment env)
        {
            _dataContext = dataContext;
            _env = env;
        }
        public IActionResult Index()
        {
            List<Slider> sliders = _dataContext.Sliders.ToList();
            List<Slider> slidersList = new List<Slider>();
            foreach (Slider slider in sliders)
            {
                if (slider.IsDeleted == false)
                {
                    slidersList.Add(slider);
                }
            }
            var query = slidersList.AsQueryable();
            return View(query.ToList());
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(Slider slider)
        {
            if (slider.Image != null)
            {
                if (slider.Image.ContentType != "image/png" && slider.Image.ContentType != "image/jpeg")
                {
                    ModelState.AddModelError("Image", "You can only upload jpeg and png");
                    return View();
                }
                if (slider.Image.Length > 2097152)
                {
                    ModelState.AddModelError("Image", "You can only upload image size lower than 2 MB");
                    return View();
                }

                
            }
            else
            {
                ModelState.AddModelError("Image", "Required");
            }
            if (!ModelState.IsValid) return View();
            slider.ImageUrl = FileManager.SaveFile(slider.Image, _env.WebRootPath, "uploads/sliders");
            _dataContext.Sliders.Add(slider);
            _dataContext.SaveChanges();
            return RedirectToAction("index");
        }
        [HttpGet]
        public IActionResult Edit(int id)
        {
            Slider slider = _dataContext.Sliders.Find(id);
            if (slider == null) return View("Error");
            return View(slider);
        }
        [HttpPost]
        public IActionResult Edit(Slider newslider)
        {
            Slider existslider = _dataContext.Sliders.Find(newslider.Id);
            if (existslider is null) return View("Error");
            if (!ModelState.IsValid) return View();

            if (newslider.Image != null)
            {
                if (newslider.Image.ContentType != "image/png" && newslider.Image.ContentType != "image/jpeg")
                {
                    ModelState.AddModelError("Image", "You can only upload jpeg and png");
                    return View();
                }
                if (newslider.Image.Length > 2097152)
                {
                    ModelState.AddModelError("Image", "You can only upload image size lower than 2 MB");
                    return View();
                }
                string deletepath = Path.Combine(_env.WebRootPath, "uploads/sliders", existslider.ImageUrl);
                if (System.IO.File.Exists(deletepath))
                {
                    System.IO.File.Delete(deletepath);
                }

                existslider.ImageUrl = newslider.Image.SaveFile(_env.WebRootPath, "uploads/sliders");
            }
            existslider.Title1 = newslider.Title1;
            existslider.Title2 = newslider.Title2;
            existslider.Desc = newslider.Desc;
            existslider.RedirectUrlShop = newslider.RedirectUrlShop;
            existslider.RedirectUrlVideo = newslider.RedirectUrlVideo;
            _dataContext.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
        
        public IActionResult HardDelete(int id)
        {
            Slider slider = _dataContext.Sliders.Find(id);
            if (slider is null) return View("Error") ;

            string deletepath = Path.Combine(_env.WebRootPath, "uploads/sliders", slider.ImageUrl);
            if (System.IO.File.Exists(deletepath))
            {
                System.IO.File.Delete(deletepath);
            }
            _dataContext.Sliders.Remove(slider);
            _dataContext.SaveChanges();
            return Ok();
        }
        public IActionResult SoftDelete(int id)
        {
            Slider slider = _dataContext.Sliders.Find(id);
            if (slider is null) return View("Error");
            slider.IsDeleted = true;
            _dataContext.SaveChanges();
            return RedirectToAction("index");
        }
    }
}
