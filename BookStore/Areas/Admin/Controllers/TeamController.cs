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
    public class TeamController : Controller
        {
            private readonly DataContext _dataContext;
            private readonly IWebHostEnvironment _env;

            public TeamController(DataContext dataContext, IWebHostEnvironment env)
            {
                _dataContext = dataContext;
                _env = env;
            }
            public IActionResult Index()
            {
            List<Team> teams = _dataContext.Teams.Include(x => x.Position).ToList();
            List<Team> teamsList = new List<Team>();
            foreach (Team team in teams)
            {
                if (team.IsDeleted == false)
                {
                    teamsList.Add(team);
                }
            }
            var query = teamsList.AsQueryable().Include(x => x.Position);
            return View(query.ToList());
        }
            public IActionResult Create()
            {
                ViewBag.Position = _dataContext.Positions.ToList();
                return View();
            }
            [HttpPost]
            public IActionResult Create(Team team)
            {
                ViewBag.Position = _dataContext.Positions.ToList();
                if (team.ImageFile != null)
                {
                    if (team.ImageFile.ContentType == "image/jpeg" && team.ImageFile.ContentType == "image/png")
                    {
                        ModelState.AddModelError("ImageFile", "you can upload jpeg or png files");
                        return View();
                    }
                    if (team.ImageFile.Length > 2097152)
                    {
                        ModelState.AddModelError("ImageFile", "you can upload lower than 2Mb");
                        return View();
                    }
                    team.ImageUrl = FileManager.SaveFile(team.ImageFile, _env.WebRootPath, "uploads/teams");

                }
                else
                {
                    ModelState.AddModelError("ImageFile", "Required");

                }
                if (!ModelState.IsValid)
                {
                    return View(team);
                }
                _dataContext.Teams.Add(team);
                _dataContext.SaveChanges();
                return RedirectToAction("index");
            }
            public IActionResult Update(int id)
            {
                ViewBag.Position = _dataContext.Positions.ToList();
                Team team = _dataContext.Teams.Find(id);
                if (team == null)
                {
                    return NotFound();
                }
                return View(team);
            }
            [HttpPost]
            public IActionResult Update(Team team)
            {
                ViewBag.Position = _dataContext.Positions.ToList();
                Team existteam = _dataContext.Teams.Find(team.Id);
                if (existteam == null)
                {
                    return NotFound();
                }
                if (!ModelState.IsValid)
                {
                    return View(team);
                }
                if (team.ImageFile != null)
                {
                    if (team.ImageFile.ContentType == "image/jpeg" && team.ImageFile.ContentType == "image/png")
                    {
                        ModelState.AddModelError("ImageFile", "you can upload jpeg or png files");
                        return View();
                    }
                    if (team.ImageFile.Length > 2097152)
                    {
                        ModelState.AddModelError("ImageFile", "you can upload lower than 2Mb");
                        return View();
                    }
                    string deletepath = Path.Combine(_env.WebRootPath, "uploads/teams", existteam.ImageUrl);
                    if (System.IO.File.Exists(deletepath))
                    {
                        System.IO.File.Delete(deletepath);
                    }
                    existteam.ImageUrl = FileManager.SaveFile(team.ImageFile, _env.WebRootPath, "uploads/teams");

                }
                existteam.PositionId = team.PositionId;
                existteam.FullName = team.FullName;
                existteam.Twitter = team.Twitter;
                existteam.Instagram = team.Instagram;
            existteam.Facebook = team.Facebook;
                _dataContext.SaveChanges();
                return RedirectToAction("index");


            }
            public IActionResult HardDelete(int id)
            {
                Team team = _dataContext.Teams.Find(id);
                if (team is null) return NotFound();

                string deletepath = Path.Combine(_env.WebRootPath, "uploads/teams", team.ImageUrl);
                if (System.IO.File.Exists(deletepath))
                {
                    System.IO.File.Delete(deletepath);
                }
                _dataContext.Teams.Remove(team);
                _dataContext.SaveChanges();
                return Ok();
            }
        public IActionResult SoftDelete(int id)
        {
            Team team = _dataContext.Teams.Find(id);
            if (team is null) return NotFound();
            team.IsDeleted = true;
            _dataContext.SaveChanges();
            return RedirectToAction("index");
        }
    }
}

