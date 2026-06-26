using Backend.Models.DTOs.MovieDTOs;
using Backend.Models.Entities;

namespace Backend.Interfaces
{
    public interface IMovieRepository
    {
        public Task<Movie?> GetMovieById(int id);
        public Task<List<Movie>> GetAllMovies();
        public Task<List<Genre>> GetAllGenres();
        public Task<Movie> CreateMovie(Movie movie, List<string> genres);
        public Task SaveChangesAsync();
        public Task DeleteMovie(Movie movie);
        public Task<Movie?> GetMovieByIdForUpdate(int id);
        public Task<MovieDto?> GetMovieDtoById(int id);
        public Task<List<Genre>> GetGenresByIds(List<int> genreIds);
        public Task<List<Genre>> GetOrCreateGenresAsync(List<string> genreNames);
        public Task UpdateMovieGenres(int movieId, List<int> newGenreIds);
    }
}
