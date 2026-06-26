using Backend.Models.DTOs.GenreDTOs;

namespace Backend.Models.DTOs.MovieDTOs
{
    public class MovieDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string OriginalTitle { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int Year { get; set; }
        public int Age { get; set; }
        public int Duration { get; set; }
        public string? PosterUrl { get; set; }
        public string? TrailerUrl { get; set; }
        public List<GenreDto> Genres { get; set; } = [];
    }
}
