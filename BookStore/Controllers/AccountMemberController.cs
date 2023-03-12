using BookStore.Data;
using BookStore.Models;
using BookStore.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookStore.Controllers
{
    public class AccountMemberController : Controller
    {
        private UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
		private readonly DataContext _datacontext;

		public AccountMemberController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, DataContext dataContext)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _datacontext= dataContext;
        }
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(MemberLoginViewModel memberLoginVM)
        {
            if (!ModelState.IsValid) return View("Error");
            AppUser member = await _userManager.FindByNameAsync(memberLoginVM.UserName);
            if (member == null)
            {
                ModelState.AddModelError("", "Username and Password is incorrect!");
                return View();
            }
            var result = await _signInManager.PasswordSignInAsync(member, memberLoginVM.Password, false, false);
            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Username and Password is incorrect!");
                return View();
            }
            return RedirectToAction("index", "home");
        }
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login");
        }
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(MemberRegisterViewModel memberRegisterVM)
        {
            if (!ModelState.IsValid) return View();

            AppUser user = null;

            user = await _userManager.FindByNameAsync(memberRegisterVM.UserName);

            if (user != null)
            {
                ModelState.AddModelError("Username", "Already exist!");
                return View();
            }

            //user = await _userManager.FindByEmailAsync(memberRegisterVM.Email);
            user = _datacontext.Users.FirstOrDefault(x => x.NormalizedEmail == memberRegisterVM.Email.ToUpper());
            if (user != null)
            {
                ModelState.AddModelError("Email", "Already exist!");
                return View();
            }

            user = new AppUser
            {
                FullName = memberRegisterVM.FullName,
                UserName = memberRegisterVM.UserName,
                Email = memberRegisterVM.Email,
                IsAdmin = false
            };

            var result = await _userManager.CreateAsync(user, memberRegisterVM.Password);

            if (!result.Succeeded)
            {
                foreach (var err in result.Errors)
                {
                    ModelState.AddModelError("", err.Description);
                    
                }
                
            }

            await _userManager.AddToRoleAsync(user, "Member");

            await _signInManager.SignInAsync(user, isPersistent: false);

            return RedirectToAction("index", "home");
        }
        public async Task<IActionResult> Profile()
		{
			AppUser member = null;
			if (User.Identity.IsAuthenticated)
			{
				member = await _userManager.FindByNameAsync(User.Identity.Name);
			}
			List<Order> orders = _datacontext.Orders.Where(x => x.AppUserId == member.Id).ToList();

			return View(orders);
		}
        public IActionResult Detail(int id)
        {
            Order order = _datacontext.Orders.Include(x => x.OrderItems).FirstOrDefault(x => x.Id == id);
            if (order == null) return View("Error");
            return View(order);
        }
    }
}
