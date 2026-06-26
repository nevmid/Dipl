using Backend.Infrastructure;
using Backend.Interfaces;
using Backend.Models.DTOs.TicketDTOs;
using Backend.Models.Entities;
using QRCoder;
using Microsoft.Extensions.Caching.Memory;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Backend.Services
{
    public class TicketService : ITicketService
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly JwtProvider _jwtProvider;
        private readonly IConfiguration _configuration;

        public TicketService(
            IBookingRepository bookingRepository,
            JwtProvider jwtProvider,
            IConfiguration configuration)
        {
            _bookingRepository = bookingRepository;
            _jwtProvider = jwtProvider;
            _configuration = configuration;
        }

        public async Task<TicketDto> GenerateTicketAsync(int bookingId)
        {
            var booking = await _bookingRepository.GetByIdWithDetailsAsync(bookingId);

            if (booking == null)
                throw new Exception("Бронирование не найдено");

            if (booking.StatusId != 2)
                throw new Exception("Бронирование не подтверждено");

            var ticketNumber = GenerateTicketNumber();

            var jwtData = new JwtProvider.TicketJwtData
            {
                BookingId = booking.Id,
                TicketNumber = ticketNumber,
                MovieTitle = booking.Session?.Movie?.Title ?? "Movie",
                SessionTime = booking.Session?.StartTime ?? DateTime.UtcNow,
                DurationMinutes = booking.Session?.Movie.Duration ?? 180,
                HallName = booking.Session?.Hall?.Name ?? "Hall",
                UserEmail = booking.User?.Email ?? ""
            };

            var token = _jwtProvider.generateTicketToken(jwtData);

            var ticket = new Ticket
            {
                BookingId = booking.Id,
                TicketNumber = ticketNumber,
                JwtToken = token,
                IsUsed = false,
                GeneratedAt = DateTime.UtcNow
            };

             await _bookingRepository.CreateTicketAsync(ticket);

            var baseUrl = _configuration["App:BaseUrl"];
            var scanUrl = $"{baseUrl}/scan?token={Uri.EscapeDataString(token)}";

            var qrCodeBase64 = GenerateQrCode(scanUrl);

            return new TicketDto
            {
                Id = ticket.Id,
                BookingId = booking.Id,
                TicketNumber = ticketNumber,
                Token = token,
                ScanUrl = scanUrl,
                QrCode = qrCodeBase64,
                MovieTitle = jwtData.MovieTitle,
                SessionTime = jwtData.SessionTime,
                HallName = jwtData.HallName,
                UserEmail = jwtData.UserEmail,
                IsUsed = false,
                GeneratedAt = DateTime.UtcNow
            };
        }

        public async Task<TicketValidationResultDto> ValidateTicketAsync(string token)
        {
            try
            {
                var (isValid, jwtData, error) = _jwtProvider.ValidateTicketToken(token);

                if (!isValid)
                    return new TicketValidationResultDto { IsValid = false, ErrorMessage = error };

                var booking = await _bookingRepository.GetByTicketNumberAsync(jwtData!.TicketNumber);

                if (booking?.Ticket == null)
                    return new TicketValidationResultDto { IsValid = false, ErrorMessage = "Билет не найден" };

                if (booking.Ticket.IsUsed)
                    return new TicketValidationResultDto { IsValid = false, ErrorMessage = "Билет уже использован" };

                var now = DateTime.UtcNow;
                var sessionStart = jwtData.SessionTime;

                if (now > sessionStart.AddHours(3))
                    return new TicketValidationResultDto { IsValid = false, ErrorMessage = "Билет недействителен" };

                return new TicketValidationResultDto
                {
                    IsValid = true,
                    Ticket = new TicketDto
                    {
                        BookingId = jwtData.BookingId,
                        TicketNumber = jwtData.TicketNumber,
                        MovieTitle = jwtData.MovieTitle,
                        SessionTime = jwtData.SessionTime,
                        HallName = jwtData.HallName,
                        UserEmail = jwtData.UserEmail,
                        IsUsed = false
                    }
                };
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> MarkTicketAsUsedAsync(string token)
        {
            var validation = await ValidateTicketAsync(token);

            if (!validation.IsValid || validation.Ticket == null)
                return false;

            var booking = await _bookingRepository.GetByTicketNumberAsync(validation.Ticket.TicketNumber);

            if (booking?.Ticket == null)
                return false;

            booking.Ticket.IsUsed = true;

            await _bookingRepository.UpdateAsync(booking);

            return true;
        }

        public async Task<TicketDto?> GetTicketByBookingIdAsync(int bookingId)
        {
            var booking = await _bookingRepository.GetByIdWithDetailsAsync(bookingId);

            if (booking?.Ticket == null)
                return null;

            var baseUrl = _configuration["App:BaseUrl"];

            return new TicketDto
            {
                Id = booking.Ticket.Id,
                BookingId = booking.Id,
                TicketNumber = booking.Ticket.TicketNumber,
                Token = booking.Ticket.JwtToken,
                ScanUrl = $"{baseUrl}/scan?token={Uri.EscapeDataString(booking.Ticket.JwtToken)}",
                QrCode = GenerateQrCode($"{baseUrl}/scan?token={Uri.EscapeDataString(booking.Ticket.JwtToken)}"),
                MovieTitle = booking.Session?.Movie?.Title ?? "",
                SessionTime = booking.Session?.StartTime ?? DateTime.UtcNow,
                HallName = booking.Session?.Hall?.Name ?? "",
                UserEmail = booking.User?.Email ?? "",
                IsUsed = booking.Ticket.IsUsed,
                GeneratedAt = booking.Ticket.GeneratedAt
            };
        }

        private string GenerateTicketNumber()
        {
            var date = DateTime.Now.ToString("yyMMdd");
            var random = Random.Shared.Next(10000, 99999);
            return $"TKT-{date}-{random}";
        }

        private string GenerateQrCode(string url)
        {
            using var qrGenerator = new QRCodeGenerator();
            using var qrCodeData = qrGenerator.CreateQrCode(url, QRCodeGenerator.ECCLevel.Q);

            using var qrCode = new PngByteQRCode(qrCodeData);
            var qrCodeBytes = qrCode.GetGraphic(20);

            return Convert.ToBase64String(qrCodeBytes);
        }
    }
}
