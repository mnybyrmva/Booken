using System.ComponentModel.DataAnnotations;

namespace BookStore.Models
{
    public class Author
    {
        public int Id { get; set; }
        [StringLength(maximumLength: 50)]
        public string FullName { get; set; }
        public bool IsDeleted { get; set; }
        public List<Book> Books { get; set; }

    }
}
