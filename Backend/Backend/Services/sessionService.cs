using System.ComponentModel.DataAnnotations;
using Backend.Interfaces;
using Backend.Models.DTOs.SessionDTOs;
using Backend.Models.Entities;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Backend.Services
{
    public class SessionService : ISessionService
    {
        private readonly IMovieRepository _movieRepository;
        private readonly IHallRepository _hallRepository;
        private readonly ISessionRepository _sessionRepository;

        public SessionService(
            IMovieRepository movieRepository,
            IHallRepository hallRepository,
            ISessionRepository sessionRepository)
        {
            _movieRepository = movieRepository;
            _hallRepository = hallRepository;
            _sessionRepository = sessionRepository;
        }
        public async Task<Session> CreateSession(CreateSessionDto dto)
        {
            try
            {
                var movie = await _movieRepository.GetMovieById(dto.MovieId);
                if (movie == null)
                    throw new ValidationException($"Фильм с ID {dto.MovieId} не найден");

                var hall = await _hallRepository.GetHallById(dto.HallId);
                if (hall == null)
                    throw new ValidationException($"Зал с ID {dto.HallId} не найден");

                var endTime = dto.StartTime.AddMinutes(movie.Duration);

                bool isHallAvailable =
                    await _sessionRepository.IsHallAvailable(dto.HallId, dto.StartTime, endTime, null);

                if (!isHallAvailable)
                    throw new ValidationException("Зал занят в выбранное время");

                var session = new Session
                {
                    MovieId = dto.MovieId,
                    HallId = dto.HallId,
                    StartTime = dto.StartTime,
                    EndTime = endTime,
                    Price = dto.Price
                };

                return await _sessionRepository.CreateSession(session);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<sessionsResponseDto>> GetAll(DateTime? date, int? hallId)
        {
            try
            {
                var sessions = await _sessionRepository.GetSessions();

                if (date != null)
                {
                    sessions = sessions.Where(s => s.StartTime.Date == date.Value.Date).ToList();
                }
                if (hallId != null)
                {
                    sessions = sessions.Where(s => s.HallId == hallId).ToList();
                }

                var response = sessions
                    .Select(s => new sessionsResponseDto
                    {
                        Id = s.Id,
                        MovieId = s.MovieId,
                        HallId = s.HallId,
                        StartTime = s.StartTime,
                        EndTime = s.EndTime,
                        Price = s.Price,
                        Movie = s.Movie,
                        Hall = s.Hall
                    })
                    .ToList();

                return response;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<sessionsResponseDto?> GetInfoAboutSession(int id)
        {
            try
            {
                var session = await _sessionRepository.GetSessionById(id);

                if (session == null)
                    return null;

                return new sessionsResponseDto {
                    Id = session.Id,
                    MovieId = session.MovieId,
                    HallId = session.HallId,
                    StartTime = session.StartTime,
                    EndTime = session.EndTime,
                    Price = session.Price,
                    //Movie = session.Movie,
                    //Hall = session.Hall,
                    Bookings = session.Bookings
                };
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<Session?> UpdateSession(int id, UpdateSessionDto dto)
        {
            try
            {
                var session = await _sessionRepository.GetSessionById(id);

                if (session == null)
                    return null;

                if (session.Bookings.Any(b => b.Status.Name != "cancelled"))
                    throw new InvalidOperationException(
                        "Нельзя редактировать сеанс: уже есть бронирования");

                var movie = await _movieRepository.GetMovieById(dto.MovieId);
                if (movie == null)
                    throw new ValidationException($"Фильм с ID {dto.MovieId} не найден");

                var hall = await _hallRepository.GetHallById(dto.HallId);
                if (hall == null)
                    throw new ValidationException($"Зал с ID {dto.HallId} не найден");

                var endTime = dto.StartTime.AddMinutes(movie.Duration);

                bool isHallAvailable =
                    await _sessionRepository.IsHallAvailable(dto.HallId, dto.StartTime, endTime, id);

                if (!isHallAvailable)
                    throw new ValidationException("Зал занят в выбранное время");

                session.MovieId = dto.MovieId;
                session.HallId = dto.HallId;
                session.StartTime = dto.StartTime;
                session.EndTime = endTime;
                session.Price = dto.Price;

                await _sessionRepository.SaveChangesAsync();

                return session;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
