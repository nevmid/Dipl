using System.ComponentModel.DataAnnotations;

namespace Backend.Models.DTOs.MovieDTOs
{
    public class UpdateMovieDto
    {
        [MaxLength(255)]
        public string? Title { get; set; }
        [MaxLength(255)]
        public string? OriginalTitle { get; set; }
        public string? Description { get; set; }
        [Range(1888, 2500)]
        public int? Year { get; set; }
        [Range(1, 1000)]
        public int? Duration { get; set; }
        [Range(0, 100)]
        public int? Age { get; set; }
        public string? PosterUrl { get; set; }
        [DataType(DataType.Upload)]
        public IFormFile? PosterFile { get; set; }
        public List<string>? Genres { get; set; }
        [Url]
        public string TrailerUrl { get; set; } = string.Empty;
    }
}
