using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BookStore.Models
{
    public class Team
    {
        public int Id { get; set; }
        public int PositionId { get; set; }
        public string? ImageUrl { get; set; }
        [StringLength(maximumLength: 50)]
        public string FullName { get; set; }
        [StringLength(maximumLength: 100)]
        public string Twitter { get; set; }
        [StringLength(maximumLength: 100)]
        public string Facebook { get; set; }
        [StringLength(maximumLength: 100)]
        public string Instagram { get; set; }
        public Position? Position { get; set; }
        [NotMapped]
        public IFormFile? ImageFile { get; set; }
        public bool IsDeleted { get; set; }
    }
}
