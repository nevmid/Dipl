using Backend.Models.DTOs.MovieDTOs;
using Backend.Models.Entities;

namespace Backend.Interfaces
{
    public interface IMovieService
    {
        public Task<MovieDto?> GetInfoAboutMovie(int id);
        public Task<List<Movie>> GetMovies();
        public Task<List<Genre>> GetAllGenres();
        public Task<Movie> CreateMovie(CreateMovieDto createMovieDto);
        public Task<Movie?> UpdateMovie(int id, UpdateMovieDto updateMovieDto);
        public Task<bool> DeleteMovie(int id);
    }
}
