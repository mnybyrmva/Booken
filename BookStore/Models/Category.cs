using System.ComponentModel.DataAnnotations;

namespace BookStore.Models
{
    public class Category
    {
        public int Id { get; set; }
        [StringLength(maximumLength: 50)]
        public string Name { get; set; }
        public bool IsDeleted { get; set; }
        public List<Book> Books { get; set; }

    }
}
