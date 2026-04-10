namespace Backend.Models.Entities
{
    public class BarCategory
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public List<BarItem> BarItems { get; set; } = [];
    }
}
