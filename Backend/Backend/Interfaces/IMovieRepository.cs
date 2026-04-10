using Backend.Models.Entities;

namespace Backend.Interfaces
{
    public interface IMovieRepository
    {
        public Task<Movie?> GetMovieById(int id);
        public Task<List<Movie>> GetAllMovies();
        public Task<Movie> CreateMovie(Movie movie);
        public Task SaveChangesAsync();
        public Task DeleteMovie(Movie movie);
    }
}
