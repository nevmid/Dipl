namespace Backend.Models.DTOs.RecommendationDTOs
{
    public class RecommendationDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? PosterUrl { get; set; }
        public int Year { get; set; }
        public List<string> Genres { get; set; } = new();
        public int MatchScore { get; set; }
    }
}
