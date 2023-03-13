using BookStore.Data;
using BookStore.Helper;
using BookStore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace BookStore.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "SuperAdmin,Admin")]
    public class BookController : Controller
    {
        private readonly IWebHostEnvironment _env;
        private readonly DataContext _dataContext;
        public BookController(DataContext dataContext,IWebHostEnvironment env)
        {
            _dataContext = dataContext;
            _env = env;
        }
        public IActionResult Index()
        {
            List<Book> books = _dataContext.Books.ToList();
            List<Book> booksList = new List<Book>();
            foreach (Book book in books)
            {
                if (book.IsDeleted == false)
                {
                    booksList.Add(book);
                }
            }
            var query = booksList.AsQueryable();
            return View(query.ToList());
        }
        public IActionResult Create()
        {
            ViewBag.Authors = _dataContext.Authors.ToList();
            ViewBag.Categories = _dataContext.Categories.ToList();
            return View();
        }
        [HttpPost]
        public IActionResult Create(Book book)
        {
            ViewBag.Authors = _dataContext.Authors.ToList();
            ViewBag.Categories = _dataContext.Categories.ToList();
            if (ModelState.IsValid) return View(book);
            if (book.PosterImageFile != null)
            {
                if (book.PosterImageFile.ContentType != "image/png" && book.PosterImageFile.ContentType != "image/jpeg")
                {
                    ModelState.AddModelError("PosterImageFile", "You can only upload jpeg and png");
                    return View(book);
                }
                if (book.PosterImageFile.Length > 2097152)
                {
                    ModelState.AddModelError("PosterImageFile", "You can only upload image size lower than 2 MB");
                    return View(book);
                }
                BookImage bookImage = new BookImage
                {
                    book = book,
                    ImageUrl = book.PosterImageFile.SaveFile(_env.WebRootPath, "uploads/books"),
                    IsPoster = true,

                };
                _dataContext.BookImages.Add(bookImage);
            }
            if (book.ImageFiles != null)
            {
                foreach (IFormFile imageFile in book.ImageFiles)
                {
                    if (imageFile.ContentType != "image/png" && imageFile.ContentType != "image/jpeg")
                    {
                        ModelState.AddModelError("ImageFiles", "You can only upload jpeg and png");
                        return View(book);
                    }
                    if (imageFile.Length > 2097152)
                    {
                        ModelState.AddModelError("ImageFiles", "You can only upload image size lower than 2 MB");
                        return View(book);
                    }
                    BookImage bookImage = new BookImage
                    {
                        book = book,
                        ImageUrl = imageFile.SaveFile(_env.WebRootPath, "uploads/books"),
                        IsPoster = false,

                    };
                    _dataContext.BookImages.Add(bookImage);
                }
            }
            else
            {
                ModelState.AddModelError("Image", "Required");
            }

            _dataContext.Books.Add(book);

            _dataContext.SaveChanges();
            return RedirectToAction("Index");
        }
        
        public IActionResult Edit(int id)
        {
            ViewBag.Authors = _dataContext.Authors.ToList();
            ViewBag.Categories = _dataContext.Categories.ToList();
            Book book = _dataContext.Books.Include(x => x.bookImages).FirstOrDefault(x => x.Id == id);
            if (book == null) { return View("Error"); }
            return View(book);
        }

        [HttpPost]
        public IActionResult Edit(Book book)
        {
            ViewBag.Authors = _dataContext.Authors.ToList();
            ViewBag.Categories = _dataContext.Categories.ToList();
            Book existBook = _dataContext.Books.Include(x => x.bookImages).FirstOrDefault(x => x.Id == book.Id);
            if (existBook is null) return NotFound();
            foreach (var bookImage in existBook.bookImages.FindAll(bi => !book.BookImageIds.Contains(bi.Id) && bi.IsPoster == false))
            {
                string deletepath = Path.Combine(_env.WebRootPath, "uploads/books", bookImage.ImageUrl);
                if (System.IO.File.Exists(deletepath))
                {
                    System.IO.File.Delete(deletepath);
                }
            }
            existBook.bookImages.RemoveAll(bi => !book.BookImageIds.Contains(bi.Id) && bi.IsPoster == false);


            if (book.PosterImageFile is not null)
            {
                if (book.PosterImageFile.ContentType != "image/jpeg" && book.PosterImageFile.ContentType != "image/png")
                {
                    ModelState.AddModelError("PosterImageFile", "You can only upload png or jpeg files");
                    return View();
                }

                if (book.PosterImageFile.Length > 2097152)
                {
                    ModelState.AddModelError("PosterImageFile", "You can only upload images under 2 mb");
                    return View();
                }
                string deletePath = Path.Combine(_env.WebRootPath, "uploads/books", existBook.bookImages.FirstOrDefault(x => x.IsPoster == true)?.ImageUrl);

                if (System.IO.File.Exists(deletePath))
                {
                    System.IO.File.Delete(deletePath);
                }
                existBook.bookImages.FirstOrDefault(x => x.IsPoster == true).ImageUrl = book.PosterImageFile.SaveFile(_env.WebRootPath, "uploads/books");
            }


            
            if (book.ImageFiles is not null)
            {
                foreach (IFormFile file in book.ImageFiles)
                {
                    if (file.ContentType != "image/jpeg" && file.ContentType != "image/png")
                    {
                        ModelState.AddModelError("ImageFiles", "You can only upload png or jpeg files");
                        return View();
                    }

                    if (file.Length > 2097152)
                    {
                        ModelState.AddModelError("ImageFiles", "You can only upload images under 2 mb");
                        return View();
                    }



                    BookImage bookImage = new BookImage
                    {
                        BookId = book.Id,
                        ImageUrl = file.SaveFile(_env.WebRootPath, "uploads/books"),
                        IsPoster =false
                    };
                    existBook.bookImages.Add(bookImage);

                }
            }


            existBook.Name = book.Name;
            existBook.CostPrice = book.CostPrice;
            existBook.SalePrice = book.SalePrice;
            existBook.DiscountPrice = book.DiscountPrice;
            existBook.CategoryId = book.CategoryId;
            existBook.AuthorId = book.AuthorId;
            existBook.BookCode = book.BookCode;
            existBook.Desc = book.Desc;
            _dataContext.SaveChanges();
            return RedirectToAction("Index");
        }
        public IActionResult HardDelete(int id)
        {
            Book book = _dataContext.Books.Include(x=>x.bookImages).FirstOrDefault(x=>x.Id==id);
            if (book is null) return View("Error");
            foreach (BookImage bookImg in book.bookImages)
            {
                if (bookImg.BookId == id)
                {
                    string deletepath = Path.Combine(_env.WebRootPath, "uploads/books", bookImg.ImageUrl);
                    if (System.IO.File.Exists(deletepath))
                    {
                        System.IO.File.Delete(deletepath);
                    }
                }
            }
            _dataContext.Books.Remove(book);
            _dataContext.SaveChanges();
            return Ok();
        }
        public IActionResult SoftDelete(int id)
        {
            Book book = _dataContext.Books.Find(id);
            if (book is null) return View("Error");
            book.IsDeleted= true;
            _dataContext.SaveChanges();
            return RedirectToAction("index");
        }
    }
}
