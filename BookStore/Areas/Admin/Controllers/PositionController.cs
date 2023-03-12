using BookStore.Data;
using BookStore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace BookStore.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "SuperAdmin,Admin")]
    public class PositionController : Controller
    {
        private readonly DataContext _dataContext;
        public PositionController(DataContext dataContext)
        {
            _dataContext = dataContext;
        }
        public IActionResult Index()
        {
            List<Position> positions = _dataContext.Positions.ToList();
            List<Position> positionsList = new List<Position>();
            foreach (Position position in positions)
            {
                if (position.IsDeleted == false)
                {
                    positionsList.Add(position);
                }
            }
            var query = positionsList.AsQueryable();
            return View(query.ToList());
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(Position position)
        {

            if (!ModelState.IsValid) { return NotFound(); }
            _dataContext.Positions.Add(position);
            _dataContext.SaveChanges();
            return RedirectToAction("index");
        }
        public IActionResult Update(int id)
        {

            Position position = _dataContext.Positions.FirstOrDefault(x => x.Id == id);
            if (position == null) { return NotFound(); }
            return View(position);
        }
        [HttpPost]
        public IActionResult Update(Position position)
        {
            Position existposition = _dataContext.Positions.FirstOrDefault(x => x.Id == position.Id);
            if (existposition == null) return NotFound();
            existposition.Name = position.Name;
            _dataContext.SaveChanges();
            return RedirectToAction("index");
        }
        public IActionResult HardDelete(int id)
        {
            Position position = _dataContext.Positions.FirstOrDefault(x => x.Id == id);
            if (position == null) return NotFound();
            _dataContext.Remove(position);
            _dataContext.SaveChanges();
            return Ok();
        }
        public IActionResult SoftDelete(int id)
        {
            Position position = _dataContext.Positions.Find(id);
            if (position is null) return NotFound();
            position.IsDeleted = true;
            _dataContext.SaveChanges();
            return RedirectToAction("index");
        }
    }
}
