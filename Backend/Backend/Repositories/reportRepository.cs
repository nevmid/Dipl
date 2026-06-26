using Backend.Interfaces;
using Backend.Models.DTOs.ReportsDTOs;
using Microsoft.EntityFrameworkCore;

namespace Backend.Repositories
{
    public class ReportRepository : IReportRepository
    {
        private readonly ApplicationDbContext _context;

        public ReportRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<SalesReportDto>> GetSalesReportAsync(DateTime? startDate, DateTime? endDate)
        {
            var query = _context.Bookings
                .Include(b => b.BookingSeats)
                .Include(b => b.Session)
                .Where(b => b.StatusId == 2 || b.StatusId == 4);

            if (startDate.HasValue)
                query = query.Where(b => b.CreatedAt >= startDate.Value);
            if (endDate.HasValue)
                query = query.Where(b => b.CreatedAt <= endDate.Value);

            var report = await query
                .GroupBy(b => b.CreatedAt.Date)
                .Select(g => new SalesReportDto
                {
                    Date = g.Key,
                    TotalBookings = g.Count(),
                    TotalTickets = g.Sum(b => b.BookingSeats.Count),
                    TotalIncome = g.Sum(b => b.FinalAmount),
                    AverageTicketPrice = g.Average(b => b.FinalAmount / b.BookingSeats.Count)
                })
                .OrderBy(r => r.Date)
                .ToListAsync();

            return report;
        }

        public async Task<List<HallLoadReportDto>> GetHallLoadReportAsync(DateTime? startDate, DateTime? endDate)
        {
            var halls = await _context.Halls
                .Include(h => h.Seats)
                .ToListAsync();

            var sessionsQuery = _context.Sessions.AsQueryable();
            if (startDate.HasValue)
                sessionsQuery = sessionsQuery.Where(s => s.StartTime >= startDate.Value);
            if (endDate.HasValue)
                sessionsQuery = sessionsQuery.Where(s => s.StartTime <= endDate.Value);

            var sessions = await sessionsQuery.ToListAsync();

            var bookingsQuery = _context.BookingSeats
                .Include(bs => bs.Booking)
                    .ThenInclude(b => b.Session)
                .Where(bs => bs.Booking.StatusId == 2 || bs.Booking.StatusId == 4);

            if (startDate.HasValue)
                bookingsQuery = bookingsQuery.Where(bs => bs.Booking.Session.StartTime >= startDate.Value);
            if (endDate.HasValue)
                bookingsQuery = bookingsQuery.Where(bs => bs.Booking.Session.StartTime <= endDate.Value);

            var bookedSeats = await bookingsQuery.ToListAsync();

            var report = new List<HallLoadReportDto>();

            foreach (var hall in halls)
            {
                var hallSessions = sessions.Where(s => s.HallId == hall.Id).ToList();
                var totalSeats = hall.Seats?.Count ?? 0;
                var totalSessions = hallSessions.Count;
                var ticketsSold = bookedSeats
                    .Where(bs => bs.Booking.Session.HallId == hall.Id)
                    .Count();

                var maxPossibleTickets = totalSeats * totalSessions;
                var loadPercentage = maxPossibleTickets > 0
                    ? (double)ticketsSold / maxPossibleTickets * 100
                    : 0;

                report.Add(new HallLoadReportDto
                {
                    HallId = hall.Id,
                    HallName = hall.Name,
                    TotalSeats = totalSeats,
                    TotalSessions = totalSessions,
                    TotalTicketsSold = ticketsSold,
                    LoadPercentage = Math.Round(loadPercentage, 2)
                });
            }

            return report.OrderByDescending(r => r.LoadPercentage).ToList();
        }

        public async Task<List<MoviePopularityReportDto>> GetMoviePopularityReportAsync(DateTime? startDate, DateTime? endDate, int limit)
        {
            var query = _context.Bookings
                .Include(b => b.BookingSeats)
                .Include(b => b.Session)
                    .ThenInclude(s => s.Movie)
                .Where(b => b.StatusId == 2 || b.StatusId == 4);

            if (startDate.HasValue)
                query = query.Where(b => b.Session.StartTime >= startDate.Value);
            if (endDate.HasValue)
                query = query.Where(b => b.Session.StartTime <= endDate.Value);

            var report = await query
                .GroupBy(b => new {
                    b.Session.MovieId,
                    b.Session.Movie.Title,
                    b.Session.Movie.PosterUrl,
                    b.Session.Movie.Year
                })
                .Select(g => new MoviePopularityReportDto
                {
                    MovieId = g.Key.MovieId,
                    MovieTitle = g.Key.Title,
                    PosterUrl = g.Key.PosterUrl,
                    Year = g.Key.Year,
                    TotalTickets = g.Sum(b => b.BookingSeats.Count),
                    TotalIncome = g.Sum(b => b.FinalAmount),
                    TotalSessions = g.Select(b => b.SessionId).Distinct().Count(),
                    TicketsPerSession = g.Sum(b => b.BookingSeats.Count) / g.Select(b => b.SessionId).Distinct().Count()
                })
                .OrderByDescending(r => r.TotalTickets)
                .Take(limit)
                .ToListAsync();

            return report;
        }

        public async Task<DailyReportDto> GetDailyReportAsync(DateTime date)
        {
            var startDate = date.Date;
            var endDate = startDate.AddDays(1);

            var bookings = await _context.Bookings
                .Include(b => b.BookingSeats)
                .Where(b => b.CreatedAt >= startDate && b.CreatedAt < endDate)
                .ToListAsync();

            var confirmedBookings = bookings.Where(b => b.StatusId == 2 || b.StatusId == 4).ToList();

            return new DailyReportDto
            {
                Date = startDate,
                BookingsCount = bookings.Count,
                TicketsSold = confirmedBookings.Sum(b => b.BookingSeats.Count),
                Income = confirmedBookings.Sum(b => b.FinalAmount),
                BonusPointsUsed = bookings.Sum(b => b.BonusUsed),
                DiscountAmount = bookings.Sum(b => (b.TotalAmount - b.FinalAmount))
            };
        }

        public async Task<object> GetSummaryAsync()
        {
            var totalIncome = await _context.Bookings
                .Where(b => b.StatusId == 2 || b.StatusId == 4)
                .SumAsync(b => (decimal?)b.FinalAmount) ?? 0;

            var totalTickets = await _context.Bookings
                .Where(b => b.StatusId == 2 || b.StatusId == 4)
                .SumAsync(b => b.BookingSeats.Count);

            var totalUsers = await _context.Users.CountAsync();
            var totalMovies = await _context.Movies.CountAsync();

            var activeSessions = await _context.Sessions
                .Where(s => s.StartTime > DateTime.UtcNow)
                .CountAsync();

            var popularMovie = await _context.Bookings
                .Where(b => b.StatusId == 2 || b.StatusId == 4)
                .GroupBy(b => b.Session.MovieId)
                .Select(g => new
                {
                    MovieId = g.Key,
                    MovieTitle = g.First().Session.Movie.Title,
                    Tickets = g.Sum(b => b.BookingSeats.Count)
                })
                .OrderByDescending(x => x.Tickets)
                .FirstOrDefaultAsync();

            return new
            {
                TotalIncome = totalIncome.ToString("F2"),
                TotalTickets = totalTickets,
                TotalUsers = totalUsers,
                TotalMovies = totalMovies,
                ActiveSessions = activeSessions,
                PopularMovie = popularMovie != null ? new
                {
                    popularMovie.MovieId,
                    popularMovie.MovieTitle,
                    popularMovie.Tickets
                } : null
            };
        }
    }
}