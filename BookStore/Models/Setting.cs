using System.ComponentModel.DataAnnotations;

namespace BookStore.Models
{
    public class Setting
    {
        public int Id { get; set; }
        [StringLength(maximumLength: 50)]
        public string Key { get; set; }
        [StringLength(maximumLength: 500)]
        public string Value { get; set; }
    }
}
