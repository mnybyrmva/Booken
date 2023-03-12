using BookStore.Models;

namespace BookStore.ViewModels
{
    public class CheckoutItemViewModel
    {
        public Book Book { get; set; }
        public int Count { get; set; }
    }
}
