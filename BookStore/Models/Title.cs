using System.ComponentModel.DataAnnotations;

namespace BookStore.Models
{
    public class Title
    {
        public int Id { get; set; }
        [StringLength(maximumLength:50)]
        public string Name { get; set; }
        [StringLength(maximumLength: 50)]
        public string Text { get; set; }
        [StringLength(maximumLength: 500)]
        public string Description { get; set; }
        public bool IsDeleted { get; set; }

    }
}
