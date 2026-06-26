using Backend.Interfaces;
using Backend.Models.DTOs.GenreDTOs;
using Backend.Models.DTOs.MovieDTOs;
using Backend.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace Backend.Repositories
{
    public class MovieRepository : IMovieRepository
    {
        private readonly ApplicationDbContext _context;

        public MovieRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<Movie> CreateMovie(Movie movie, List<string> genres)
        {
            await _context.Movies.AddAsync(movie);
            var result = await GetOrCreateGenresAsync(genres);
            foreach (var genre in result)
            {
                await _context.MovieGenres.AddAsync(new MovieGenre
                {
                    MovieId = movie.Id,
                    GenreId = genre.Id
                });
            }
            await _context.SaveChangesAsync();
            return movie;
        }

        public async Task DeleteMovie(Movie movie)
        {
            _context.Movies.Remove(movie);
            await _context.SaveChangesAsync();
            return;
        }

        public async Task<List<Movie>> GetAllMovies()
        {
            return await _context.Movies
                .Include(m => m.Sessions)
                .Include(m => m.MovieGenres)
                    .ThenInclude(mg => mg.Genre)
                .ToListAsync();
        }

        public async Task<Movie?> GetMovieById(int id)
        {
            return await _context.Movies
                .Include(m => m.MovieGenres)
                    .ThenInclude(mg => mg.Genre)
                .FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task<Movie?> GetMovieByIdForUpdate(int id)
        {
            return await _context.Movies
                .Include(m => m.MovieGenres)
                .FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task<MovieDto?> GetMovieDtoById(int id)
        {
            var movie = await GetMovieById(id);

            if (movie == null)
                return null;

            return new MovieDto
            {
                Id = movie.Id,
                Title = movie.Title,
                OriginalTitle = movie.OriginalTitle,
                Description = movie.Description,
                Year = movie.Year,
                Duration = movie.Duration,
                PosterUrl = movie.PosterUrl,
                TrailerUrl = movie.TrailerUrl,
                Age = movie.Age,
                Genres = movie.MovieGenres.Select(mg => new GenreDto
                {
                    Id = mg.Genre.Id,
                    Name = mg.Genre.Name
                }).ToList()
            };
        }

        public async Task<List<Genre>> GetGenresByIds(List<int> genreIds)
        {
            return await _context.Genres
                .Where(g => genreIds.Contains(g.Id))
                .ToListAsync();
        }

        public async Task<List<Genre>> GetOrCreateGenresAsync(List<string> genreNames)
        {
            var result = new List<Genre>();

            foreach (var genreName in genreNames)
            {
                var normalizedName = genreName.Trim();

                var existingGenre = await _context.Genres
                    .FirstOrDefaultAsync(g => g.Name.ToLower() == normalizedName.ToLower());

                if (existingGenre != null)
                {
                    result.Add(existingGenre);
                }
                else
                {
                    var newGenre = new Genre
                    {
                        Name = normalizedName,
                    };

                    _context.Genres.Add(newGenre);
                    result.Add(newGenre);
                }
            }

            await _context.SaveChangesAsync();
            return result;
        }

        public async Task UpdateMovieGenres(int movieId, List<int> newGenreIds)
        {
            var oldGenres = await _context.MovieGenres
                .Where(mg => mg.MovieId == movieId)
                .ToListAsync();

            _context.MovieGenres.RemoveRange(oldGenres);

            foreach (var genreId in newGenreIds)
            {
                await _context.MovieGenres.AddAsync(new MovieGenre
                {
                    MovieId = movieId,
                    GenreId = genreId
                });
            }

            await _context.SaveChangesAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
            return;
        }

        public async Task<List<Genre>> GetAllGenres()
        {
            var genres = await _context.Genres
                .ToListAsync();

            return genres;
        }
    }
}
