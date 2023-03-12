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
    public class TestimonialController : Controller
    {
        private readonly DataContext _dataContext;
        private readonly IWebHostEnvironment _env;

        public TestimonialController(DataContext dataContext, IWebHostEnvironment env)
        {
            _dataContext = dataContext;
            _env = env;
        }
        public IActionResult Index()
        {
            List<Testimonial> testimonials = _dataContext.Testimonials.ToList();
            List<Testimonial> testimonialsList = new List<Testimonial>();
            foreach (Testimonial testimonial in testimonials)
            {
                if (testimonial.IsDeleted == false)
                {
                    testimonialsList.Add(testimonial);
                }
            }
            var query = testimonialsList.AsQueryable();
            return View(query.ToList());
        }
        public IActionResult Create()
        {
            ViewBag.Profession = _dataContext.Positions.ToList();
            return View();
        }
        [HttpPost]
        public IActionResult Create(Testimonial testimonial)
        {
            ViewBag.Profession = _dataContext.Positions.ToList();
            if (testimonial.Image != null)
            {
                if (testimonial.Image.ContentType == "image/jpeg" && testimonial.Image.ContentType == "image/png")
                {
                    ModelState.AddModelError("ImageFile", "you can upload jpeg or png files");
                    return View();
                }
                if (testimonial.Image.Length > 2097152)
                {
                    ModelState.AddModelError("ImageFile", "you can upload lower than 2Mb");
                    return View();
                }
                testimonial.ImageUrl = FileManager.SaveFile(testimonial.Image, _env.WebRootPath, "uploads/testimonials");

            }
            else
            {
                ModelState.AddModelError("ImageFile", "Required");

            }
            if (!ModelState.IsValid)
            {
                return View(testimonial);
            }
            _dataContext.Testimonials.Add(testimonial);
            _dataContext.SaveChanges();
            return RedirectToAction("index");
        }
        public IActionResult Edit(int id)
        {
            ViewBag.Profession = _dataContext.Positions.ToList();
            Testimonial testimonial = _dataContext.Testimonials.Find(id);
            if (testimonial == null)
            {
                return NotFound();
            }
            return View(testimonial);
        }
        [HttpPost]
        public IActionResult Edit(Testimonial testimonial)
        {
            ViewBag.Profession = _dataContext.Positions.ToList();
            Testimonial existtestimonial = _dataContext.Testimonials.Find(testimonial.Id);
            if (existtestimonial == null)
            {
                return NotFound();
            }
            if (!ModelState.IsValid)
            {
                return View(testimonial);
            }
            if (testimonial.Image != null)
            {
                if (testimonial.Image.ContentType == "image/jpeg" && testimonial.Image.ContentType == "image/png")
                {
                    ModelState.AddModelError("ImageFile", "you can upload jpeg or png files");
                    return View();
                }
                if (testimonial.Image.Length > 2097152)
                {
                    ModelState.AddModelError("ImageFile", "you can upload lower than 2Mb");
                    return View();
                }
                string deletepath = Path.Combine(_env.WebRootPath, "uploads/testimonials", existtestimonial.ImageUrl);
                if (System.IO.File.Exists(deletepath))
                {
                    System.IO.File.Delete(deletepath);
                }
                existtestimonial.ImageUrl = FileManager.SaveFile(testimonial.Image, _env.WebRootPath, "uploads/testimonials");

            }
            existtestimonial.Name = testimonial.Name;
            existtestimonial.Desc = testimonial.Desc;
            _dataContext.SaveChanges();
            return RedirectToAction("index");


        }
        public IActionResult HardDelete(int id)
        {
            Testimonial testimonial = _dataContext.Testimonials.Find(id);
            if (testimonial is null) return NotFound();

            string deletepath = Path.Combine(_env.WebRootPath, "uploads/testimonials", testimonial.ImageUrl);
            if (System.IO.File.Exists(deletepath))
            {
                System.IO.File.Delete(deletepath);
            }
            _dataContext.Testimonials.Remove(testimonial);
            _dataContext.SaveChanges();
            return Ok();
        }
        public IActionResult SoftDelete(int id)
        {
            Testimonial testimonial = _dataContext.Testimonials.Find(id);
            if (testimonial is null) return NotFound();
            testimonial.IsDeleted = true;
            _dataContext.SaveChanges();
            return RedirectToAction("index");
        }
    }
}
