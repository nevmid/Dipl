namespace Backend.Models.Entities
{
    public class BarItem
    {
        public int Id { get; set; }
        public int CategoryId { get; set; }
        public string Name { get; set; } = string.Empty;
        public double Price { get; set; }
        public bool IsAvailable { get; set; }
        public BarCategory Category { get; set; } = null!;
    }
}
