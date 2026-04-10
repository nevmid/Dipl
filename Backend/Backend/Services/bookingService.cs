using Backend.Interfaces;
using Backend.Models.DTOs.BookingDTOs;
using Backend.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace Backend.Services
{
    public class BookingService : IBookingService
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly ISessionRepository _sessionRepository;
        private readonly ISeatRepository _seatRepository;
        private readonly IPaymentGateway _paymentGateway;

        private const int BOOKING_EXPIRY_MINUTES = 15;

        public BookingService(
            IBookingRepository bookingRepository,
            ISessionRepository sessionRepository,
            ISeatRepository seatRepository,
            IPaymentGateway paymentGateway)
        {
            _bookingRepository = bookingRepository;
            _sessionRepository = sessionRepository;
            _seatRepository = seatRepository;
            _paymentGateway = paymentGateway;
        }

        public async Task<BookingResponseDto?> CreateBookingAsync(int userId, CreateBookingDto dto)
        {
            try
            {
                var session = await _sessionRepository.GetSessionById(dto.SessionId);
                if (session == null)
                {
                    return null;
                }

                if (session.StartTime <= DateTime.UtcNow)
                {
                    throw new InvalidOperationException("Сессия уже началась или завершилась");
                }

                if (dto.SeatIds == null || !dto.SeatIds.Any())
                {
                    throw new ArgumentException("Необходимо выбрать хотя бы одно место");
                }

                var seats = await _seatRepository.GetSeatsByIdsAsync(dto.SeatIds);
                if (seats.Count != dto.SeatIds.Count)
                {
                    throw new ArgumentException("Некоторые места не найдены");
                }

                var areSeatsAvailable = await _bookingRepository.AreSeatsAvailableAsync(
                    dto.SessionId, dto.SeatIds);

                if (!areSeatsAvailable)
                {
                    throw new InvalidOperationException("Некоторые места уже забронированы");
                }

                var totalAmount = CalculateTotalAmount(seats, session);

                var booking = new Booking
                {
                    UserId = userId,
                    SessionId = dto.SessionId,
                    Status = "pending",
                    TotalAmount = totalAmount,
                    CreatedAt = DateTime.UtcNow,
                    BookingSeats = dto.SeatIds.Select(seatId => new BookingSeat
                    {
                        SeatId = seatId
                    }).ToList(),
                    Payment = new Payment
                    {
                        Amount = totalAmount,
                        Status = "pending"
                    }
                };

                await _bookingRepository.CreateAsync(booking);

                return MapToDto(booking);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<BookingResponseDto?> GetBookingAsync(int id, int? userId = null)
        {
            try
            {
                var booking = await _bookingRepository.GetByIdWithDetailsAsync(id);

                if (booking == null)
                    return null;

                return MapToDto(booking);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<BookingResponseDto>> GetUserBookingsAsync(int userId)
        {
            try
            {
                var bookings = await _bookingRepository.GetUserBookings(userId);
                return bookings.Select(MapToDto).ToList();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<BookingResponseDto>> GetAllBookingsAsync()
        {
            try
            {
                var bookings = await _bookingRepository.GetAllAsync();
                return bookings.Select(MapToDto).ToList();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> CancelBookingAsync(int id, int userId)
        {
            try
            {
                var booking = await _bookingRepository.GetByIdAsync(id);

                if (booking == null || booking.UserId != userId)
                    return false;

                if (booking.Status != "pending")
                {
                    throw new InvalidOperationException(
                        $"Невозможно отменить бронирование со статусом {booking.Status}");
                }

                if (booking.Payment != null && booking.Payment.Status == "pending")
                {
                    booking.Payment.Status = "cancelled";
                }


                booking.Status = "cancelled";
                await _bookingRepository.UpdateAsync(booking);

                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<PaymentResponseDto?> ProcessPaymentAsync(ProcessPaymentDto dto, int userId)
        {
            try
            {
                var booking = await _bookingRepository.GetByIdWithDetailsAsync(dto.BookingId);

                if (booking == null || booking.UserId != userId)
                {
                    return null;
                }

                if (booking.Status != "pending")
                {
                    throw new InvalidOperationException(
                        $"Невозможно оплатить бронирование со статусом {booking.Status}");
                }

                if (booking.CreatedAt.AddMinutes(BOOKING_EXPIRY_MINUTES) < DateTime.UtcNow)
                {
                    booking.Status = "expired";
                    await _bookingRepository.UpdateAsync(booking);
                    throw new InvalidOperationException("Время бронирования истекло");
                }

                if (Math.Abs(dto.Amount - booking.TotalAmount) > 0.01)
                {
                    throw new ArgumentException(
                        $"Сумма платежа ({dto.Amount}) не совпадает с суммой бронирования ({booking.TotalAmount})");
                }

                if (booking.Payment == null)
                {
                    booking.Payment = new Payment
                    {
                        BookingId = booking.Id,
                        Amount = dto.Amount,
                        Status = "pending"
                    };
                }

                var paymentResult = await _paymentGateway.ProcessPaymentAsync(new PaymentRequest
                {
                    Amount = dto.Amount,
                    BookingId = dto.BookingId,
                    UserId = userId,
                    PaymentMethod = dto.PaymentMethod
                });

                booking.Payment.Status = paymentResult.Success ? "completed" : "failed";

                if (paymentResult.Success)
                {
                    booking.Status = "confirmed";
                }

                await _bookingRepository.UpdateAsync(booking);

                return new PaymentResponseDto
                {

                    Id = booking.Payment.Id,
                    Amount = booking.Payment.Amount,
                    Status = booking.Payment.Status,
                    CreatedAt = DateTime.UtcNow
                };
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<PaymentResponseDto?> GetPaymentStatusAsync(int bookingId, int userId)
        {
            try
            {
                var booking = await _bookingRepository.GetByIdWithDetailsAsync(bookingId);

                if (booking == null || booking.UserId != userId)
                    return null;

                if (booking.Payment == null)
                    return null;

                return new PaymentResponseDto
                {
                    Id = booking.Payment.Id,
                    Amount = booking.Payment.Amount,
                    Status = booking.Payment.Status,
                    CreatedAt = booking.CreatedAt
                };
            }
            catch (Exception)
            {
                throw;
            }
        }

        private double CalculateTotalAmount(List<Seat> seats, Session session)
        {
            return seats.Count * session.Price;
        }

        private BookingResponseDto MapToDto(Booking booking)
        {
            return new BookingResponseDto
            {
                Id = booking.Id,
                UserId = booking.UserId,
                SessionId = booking.SessionId,
                Status = booking.Status,
                TotalAmount = booking.TotalAmount,
                CreatedAt = booking.CreatedAt,
                SeatIds = booking.BookingSeats?.Select(bs => bs.SeatId).ToList() ?? new(),
                Payment = booking.Payment != null ? new PaymentResponseDto
                {
                    Id = booking.Payment.Id,
                    Amount = booking.Payment.Amount,
                    Status = booking.Payment.Status,
                    CreatedAt = booking.CreatedAt
                } : null
            };
        }
    }
}