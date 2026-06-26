namespace Backend.Models.DTOs.HallDTOs
{
    public class ResponseHallDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int Rows { get; set; }
        public int Cols { get; set; }
        public int SessionsCount { get; set; }
    }
}