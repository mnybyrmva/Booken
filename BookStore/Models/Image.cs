using System.ComponentModel.DataAnnotations.Schema;

namespace BookStore.Models
{
    public class Image
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? ImageUrl { get; set; }
        [NotMapped]
        public IFormFile? ImageFile { get; set; }
        public bool IsDeleted { get; set; }

    }
}
