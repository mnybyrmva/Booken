namespace BookStore.Models
{
    public class Position
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<Team>? teams { get; set; }
        public bool IsDeleted { get; set; }

    }
}
