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
    public class ImageController : Controller
    {
        private readonly DataContext _dataContext;
        private readonly IWebHostEnvironment _env;
        public ImageController(DataContext dataContext, IWebHostEnvironment env)
        {
            _dataContext = dataContext;
            _env = env;
        }
        public IActionResult Index()
        {
            List<Image> images = _dataContext.Images.ToList();
            List<Image> imagesList = new List<Image>();
            foreach (Image image in images)
            {
                if (image.IsDeleted == false)
                {
                    imagesList.Add(image);
                }
            }
            var query = imagesList.AsQueryable();
            return View(query.ToList());
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(Image image)
        {

            if (!ModelState.IsValid) { return NotFound(); }
            if (image.ImageFile != null)
            {
                if (image.ImageFile.ContentType == "image/jpeg" && image.ImageFile.ContentType == "image/png")
                {
                    ModelState.AddModelError("ImageFile", "you can upload jpeg or png files");
                    return View();
                }
                if (image.ImageFile.Length > 2097152)
                {
                    ModelState.AddModelError("ImageFile", "you can upload lower than 2Mb");
                    return View();
                }
                image.ImageUrl = FileManager.SaveFile(image.ImageFile, _env.WebRootPath, "uploads/images");

            }
            else
            {
                ModelState.AddModelError("ImageFile", "Required");

            }
            if (!ModelState.IsValid)
            {
                return View(image);
            }
            _dataContext.Images.Add(image); 
            _dataContext.SaveChanges();
            return RedirectToAction("index");
        }
        public IActionResult Edit(int id)
        {

            Image image = _dataContext.Images.FirstOrDefault(x => x.Id == id);
            if (image == null) return View("Error"); 
            return View(image);
        }
        [HttpPost]
        public IActionResult Edit(Image image)
        {
            Image existimage = _dataContext.Images.FirstOrDefault(x => x.Id == image.Id);
            if (existimage == null) return View("Error");
            if (!ModelState.IsValid)
            {
                return View(image);
            }
            if (image.ImageFile != null)
            {
                if (image.ImageFile.ContentType == "image/jpeg" && image.ImageFile.ContentType == "image/png")
                {
                    ModelState.AddModelError("ImageFile", "you can upload jpeg or png files");
                    return View();
                }
                if (image.ImageFile.Length > 2097152)
                {
                    ModelState.AddModelError("ImageFile", "you can upload lower than 2Mb");
                    return View();
                }
                string deletepath = Path.Combine(_env.WebRootPath, "uploads/images", existimage.ImageUrl);
                if (System.IO.File.Exists(deletepath))
                {
                    System.IO.File.Delete(deletepath);
                }
                existimage.ImageUrl = FileManager.SaveFile(image.ImageFile, _env.WebRootPath, "uploads/images");

            }
            existimage.Name = image.Name;
            _dataContext.SaveChanges();
            return RedirectToAction("index");
        }
    }
}
