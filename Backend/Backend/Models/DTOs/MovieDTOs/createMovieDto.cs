using System.ComponentModel.DataAnnotations;

namespace Backend.Models.DTOs.MovieDTOs
{
    public class CreateMovieDto
    {
        [Required]
        [MaxLength(255)]
        public string Title { get; set; } = string.Empty;
        [Required]
        [MaxLength(255)]
        public string OriginalTitle { get; set; } = string.Empty;
        [Required]
        public string Description { get; set; } = string.Empty;
        [Required]
        [Range(1888, 2100)] 
        public int Year { get; set; }
        [Required]
        [Range(1, 1000)]
        public int Duration { get; set; }
        [Url]
        [DataType(DataType.ImageUrl)]
        public string? PosterUrl { get; set; }
        [DataType(DataType.Upload)]
        public IFormFile? PosterFile {  get; set; }
        [Required]
        [Url]
        public string TrailerUrl { get; set; } = string.Empty;
        [Required]
        [Range(0, 10)]
        public double Rating { get; set; }
    }
}
