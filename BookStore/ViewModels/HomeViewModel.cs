using BookStore.Models;

namespace BookStore.ViewModels
{
    public class HomeViewModel
    {
        public List<Slider> Sliders { get; set; }
        public List<Book> FeaturedBooks { get; set; }
        public List<Book> books { get; set; }
        public List<Book> NewBooks { get; set; }
        public List<Book> DiscountedBooks { get; set; }
        public List<Author> authors { get; set; }
        public List<Category> categories { get; set; }
        public List<BookImage> bookImages { get; set; }
        public List<Team> teams { get; set; }
        public List<Position> positions { get; set; }
        public List<Testimonial> testimonials { get; set; }
        public List<Text> texts { get; set; }
        public List<Image> images { get; set; }
        public List<Title> titles { get; set; }
        public List<Setting> settings { get; set; }

    }
}
