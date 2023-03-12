using BookStore.Data;
using BookStore.Models;
using BookStore.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using static System.Reflection.Metadata.BlobBuilder;

namespace BookStore.Service
{
    public class LayoutService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly DataContext _datacontext;


        public LayoutService(UserManager<AppUser> userManager, IHttpContextAccessor httpContextAccessor, DataContext datacontext)
        {
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
            _datacontext = datacontext;
        }
        public async Task<AppUser> GetUser()
        {
            AppUser appUser = null;
            if (_httpContextAccessor.HttpContext.User.Identity.IsAuthenticated)
            {
                appUser = await _userManager.FindByNameAsync(_httpContextAccessor.HttpContext.User.Identity.Name);
                return appUser;
            }
            return null;
        }
        public async Task<List<BasketItem>> GetBasket()
        {
            AppUser member = null;
            if (_httpContextAccessor.HttpContext.User.Identity.IsAuthenticated)
            {
                member = await _userManager.FindByNameAsync(_httpContextAccessor.HttpContext.User.Identity.Name);
            }
            List<BasketItem> basketItems = null;

            if (member == null)
            {
                string basketItemsStr = _httpContextAccessor.HttpContext.Request.Cookies["BasketItems"];
                if (basketItemsStr != null)
                {
                    basketItems = JsonConvert.DeserializeObject<List<BasketItem>>(basketItemsStr);
                }
                if (basketItems != null)
                {
                    foreach (BasketItem basketItem in basketItems)
                    {
                        basketItem.Book = _datacontext.Books.FirstOrDefault(x => x.Id == basketItem.BookId);
                    }
                }
            }
            else
            {
                basketItems = _datacontext.BasketItems.Include(x => x.Book).Where(x => x.AppUserId == member.Id).ToList();
            }

            return basketItems;
        }
        public List<Setting> GetSetting()
        {
            List<Setting> settings = _datacontext.Settings.ToList();
            return settings;
    }
        public List<Author> GetAuthors()
        {
            List<Author>  authors= _datacontext.Authors.ToList();
            return authors;
        }
        public List<Category> GetCategories()
        {
            List<Category> categories = _datacontext.Categories.ToList();
            return categories;
        }
    }
}
