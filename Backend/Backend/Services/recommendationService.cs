using Backend.Interfaces;
using Backend.Models.DTOs.RecommendationDTOs;
using Backend.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace Backend.Services
{
    public class RecommendationService : IRecommendationService
    {
        private readonly ApplicationDbContext _context;

        public RecommendationService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<RecommendationsResponseDto> GetRecommendationsForUser(int? userId)
        {
            var response = new RecommendationsResponseDto();

            response.Trending = await GetTrendingRecommendations();

            if (userId.HasValue && userId.Value > 0)
            {

                response.Personalized = await GetPersonalizedRecommendations(userId.Value);
                response.Collaborative = await GetCollaborativeRecommendations(userId.Value);
            }

            return response;
        }

        private async Task<List<RecommendationDto>> GetPersonalizedRecommendations(int userId, int take = 8)
        {
            try
            {
                var userGenrePreferences = await _context.Bookings
                    .Where(b => b.UserId == userId && b.StatusId == 2)
                    .SelectMany(b => b.Session.Movie.MovieGenres)
                    .GroupBy(mg => mg.GenreId)
                    .Select(g => new
                    {
                        GenreId = g.Key,
                        GenreName = g.First().Genre.Name,
                        Count = g.Count()
                    })
                    .OrderByDescending(x => x.Count)
                    .Take(5)
                    .ToListAsync();

                if (!userGenrePreferences.Any())
                    return new List<RecommendationDto>();

                var preferredGenreIds = userGenrePreferences.Select(x => x.GenreId).ToList();

                var watchedMovieIds = await _context.Bookings
                    .Where(b => b.UserId == userId && b.StatusId == 2)
                    .Select(b => b.Session.MovieId)
                    .Distinct()
                    .ToListAsync();

                var recommendations = await _context.Movies
                    .Where(m => !watchedMovieIds.Contains(m.Id))
                    .Select(m => new
                    {
                        Movie = m,
                        MatchCount = m.MovieGenres.Count(mg => preferredGenreIds.Contains(mg.GenreId)),
                        Genres = m.MovieGenres.Select(mg => mg.Genre.Name).ToList()
                    })
                    .Where(x => x.MatchCount > 0)
                    .OrderByDescending(x => x.MatchCount)
                    .Take(take)
                    .ToListAsync();

                return recommendations.Select(r => new RecommendationDto
                {
                    Id = r.Movie.Id,
                    Title = r.Movie.Title,
                    PosterUrl = r.Movie.PosterUrl,
                    Year = r.Movie.Year,
                    Genres = r.Genres,
                    MatchScore = (int)((double)r.MatchCount / preferredGenreIds.Count * 100)
                }).ToList();
            }
            catch (Exception ex)
            {
                return new List<RecommendationDto>();
            }
        }

        private async Task<List<RecommendationDto>> GetCollaborativeRecommendations(int userId, int take = 8)
        {
            try
            {
                var lastWatchedMovies = await _context.Bookings
                    .Where(b => b.UserId == userId && b.StatusId == 2)
                    .OrderByDescending(b => b.CreatedAt)
                    .Select(b => b.Session.MovieId)
                    .Distinct()
                    .Take(3)
                    .ToListAsync();


                if (!lastWatchedMovies.Any())
                    return new List<RecommendationDto>();

                var similarUsers = await _context.Bookings
                    .Where(b => lastWatchedMovies.Contains(b.Session.MovieId) 
                                && b.UserId != userId && b.StatusId == 2)
                    .Select(b => b.UserId)
                    .Distinct()
                    .ToListAsync();

                if (!similarUsers.Any())
                    return new List<RecommendationDto>();

                var watchedByUser = await _context.Bookings
                    .Where(b => b.UserId == userId && b.StatusId == 2)
                    .Select(b => b.Session.MovieId)
                    .Distinct()
                    .ToListAsync();

                var recommendations = await _context.Bookings
                    .Where(b => similarUsers.Contains(b.UserId)
                                && !watchedByUser.Contains(b.Session.MovieId)
                                && b.StatusId == 2)
                    .GroupBy(b => b.Session.MovieId)
                    .Select(g => new
                    {
                        MovieId = g.Key,
                        WatchCount = g.Count(),
                        Movie = g.First().Session.Movie
                    })
                    .OrderByDescending(x => x.WatchCount)
                    .Take(take)
                    .ToListAsync();

                var maxWatchCount = recommendations.Any() ? recommendations.Max(r => r.WatchCount) : 1;

                return recommendations.Select(r => new RecommendationDto
                {
                    Id = r.Movie.Id,
                    Title = r.Movie.Title,
                    PosterUrl = r.Movie.PosterUrl,
                    Year = r.Movie.Year,
                    Genres = r.Movie.MovieGenres.Select(mg => mg.Genre.Name).ToList(),
                    MatchScore = (int)((double)r.WatchCount / maxWatchCount * 100)
                }).ToList();
            }
            catch (Exception ex)
            {
                return new List<RecommendationDto>();
            }
        }

        private async Task<List<RecommendationDto>> GetTrendingRecommendations(int take = 8)
        {
            try
            {
                var oneWeekAgo = DateTime.UtcNow.AddDays(-7);

                var trending = await _context.Bookings
                    .Where(b => b.CreatedAt >= oneWeekAgo && b.StatusId == 2)
                    .GroupBy(b => b.Session.MovieId)
                    .Select(g => new
                    {
                        MovieId = g.Key,
                        BookingCount = g.Count(),
                        Movie = g.First().Session.Movie
                    })
                    .OrderByDescending(x => x.BookingCount)
                    .Take(take)
                    .ToListAsync();

                var maxBookingCount = trending.Any() ? trending.Max(t => t.BookingCount) : 1;

                return trending.Select(t => new RecommendationDto
                {
                    Id = t.Movie.Id,
                    Title = t.Movie.Title,
                    PosterUrl = t.Movie.PosterUrl,
                    Year = t.Movie.Year,
                    Genres = t.Movie.MovieGenres.Select(mg => mg.Genre.Name).ToList(),
                    MatchScore = (int)((double)t.BookingCount / maxBookingCount * 100)
                }).ToList();
            }
            catch (Exception ex)
            {
                return new List<RecommendationDto>();
            }
        }
    }
}
