using Backend.Interfaces;
using Backend.Models.DTOs.BookingDTOs;
using Backend.Models.DTOs.PaymentDTOs;
using Backend.Models.Entities;
using Backend.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Backend.Services
{
    public class BookingService : IBookingService
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly ISessionRepository _sessionRepository;
        private readonly ISeatRepository _seatRepository;
        private readonly ITicketService _ticketService;
        private readonly IYookassaService _yookassaService;
        private readonly IUserRepository _userRepository;
        private const int BOOKING_EXPIRY_MINUTES = 15;

        private const int STATUS_PENDING = 1;
        private const int STATUS_CONFIRMED = 2;
        private const int STATUS_CANCELLED = 3;
        private const int STATUS_PAID = 4;
        private const int STATUS_FAILED = 5;
        private const int STATUS_REFUNDED= 6;
        private const int STATUS_ACTIVE= 7;
        private const int STATUS_USED = 8;
        private const int STATUS_EXPIRED= 9;

        public BookingService(
            IBookingRepository bookingRepository,
            ISessionRepository sessionRepository,
            ISeatRepository seatRepository,
            ITicketService ticketService,
            IYookassaService yookassaService,
            IUserRepository userRepository)
        {
            _bookingRepository = bookingRepository;
            _sessionRepository = sessionRepository;
            _seatRepository = seatRepository;
            _ticketService = ticketService;
            _yookassaService = yookassaService;
            _userRepository = userRepository;
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

                var user = await _userRepository.GetUserById(userId);
                var totalAmount = CalculateTotalAmount(seats, session);

                var bonusToUse = dto.isBonusUsed ?
                    Math.Min(user.LoyaltyAccount.Balance, (int)(totalAmount * 0.7)) : 0;

                var finalAmount = totalAmount - bonusToUse;
                if (finalAmount < 0) finalAmount = 0;

                var booking = new Booking
                {
                    UserId = userId,
                    SessionId = dto.SessionId,
                    TotalAmount = totalAmount,
                    FinalAmount = finalAmount,
                    BonusUsed = (int)bonusToUse,
                    CreatedAt = DateTime.UtcNow,
                    StatusId = STATUS_PENDING,
                    BookingSeats = dto.SeatIds.Select(seatId => new BookingSeat
                    {
                        SeatId = seatId
                    }).ToList()
                };

                await _bookingRepository.CreateAsync(booking);

                var createdBooking = await _bookingRepository.GetByIdWithDetailsAsync(booking.Id);

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
            using var transaction = await _bookingRepository.BeginTransactionAsync();

            try
            {
                var booking = await _bookingRepository.GetByIdWithDetailsAsync(id);

                if (booking == null || booking.UserId != userId)
                    return false;

                if (booking.StatusId != STATUS_PENDING)
                {
                    throw new InvalidOperationException(
                        $"Невозможно отменить бронирование со статусом {booking.Status.Name}");
                }

                if (booking.Payment != null && booking.Payment.StatusId == STATUS_PENDING)
                {
                    booking.Payment.StatusId = STATUS_CANCELLED;
                }

                //Console.WriteLine(booking.Payment.TransactionId);

                //var success = await _yookassaService.CancelPaymentAsync(booking.Payment.TransactionId);
                //if (!success)
                //    return false;

                booking.StatusId = STATUS_CANCELLED;
                await _bookingRepository.UpdateAsync(booking);
                await transaction.CommitAsync();
                return true;
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
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
                    Status = booking.Payment.Status.Name,
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
            var seatsFormatted = new List<string>();
            if (booking.BookingSeats != null && booking.BookingSeats.Any())
            {
                foreach (var bs in booking.BookingSeats)
                {
                    if (bs.Seat != null)
                    {
                        seatsFormatted.Add($"Ряд {bs.Seat.RowNum} Место {bs.Seat.ColNum}");
                    }
                }
            }

            return new BookingResponseDto
            {
                Id = booking.Id,
                UserId = booking.UserId,
                SessionId = booking.SessionId,
                Status = booking.Status.Name,
                TotalAmount = booking.TotalAmount,
                FinalAmount = booking.FinalAmount,
                BonusUsed = booking.BonusUsed,
                CreatedAt = booking.CreatedAt,
                SeatsFormatted = seatsFormatted,
                SeatIds = booking.BookingSeats?.Select(bs => bs.SeatId).ToList() ?? new(),
                Payment = booking.Payment != null ? new PaymentResponseDto
                {
                    Id = booking.Payment.Id,
                    Amount = booking.Payment.Amount,
                    Status = booking.Payment.Status.Name,
                    CreatedAt = booking.CreatedAt,
                    PaymentUrl = booking.Payment.PaymentUrl
                } : null,
                Ticket = booking.Ticket ?? null,
                Movie = booking.Session.Movie ?? null,
                Hall = booking.Session.Hall ?? null,
                Session = booking.Session ?? null,
            };
        }

        public async Task<PaymentInitiationDto> InitiatePaymentAsync(int bookingId, int userId)
        {
            try
            {
                var booking = await _bookingRepository.GetByIdWithDetailsAsync(bookingId);

                if (booking == null || booking.UserId != userId)
                    return new PaymentInitiationDto { 
                        Success = false,
                        Error = "Бронирование не найдено"
                    };

                if (booking.StatusId != STATUS_PENDING)
                    return new PaymentInitiationDto
                    {
                        Success = false,
                        Error = $"Невозможно оплатить бронирование со статусом {booking.Status?.Name}"
                    };

                if (booking.CreatedAt.AddMinutes(BOOKING_EXPIRY_MINUTES) < DateTime.UtcNow)
                {
                    booking.StatusId = STATUS_EXPIRED;
                    await _bookingRepository.UpdateAsync(booking);
                    return new PaymentInitiationDto { Success = false, Error = "Время бронирования истекло" };
                }

                var description = $"Билеты в кино: {booking.Session?.Movie?.Title}";
                var (success, paymentId, paymentUrl, error) = await _yookassaService.CreatePaymentAsync(
                    (decimal)booking.FinalAmount,
                    description,
                    booking.Id,
                    userId
                );

                if (!success)
                    return new PaymentInitiationDto { Success = false, Error = error ?? "Ошибка создания платежа" };

                if (booking.Payment == null)
                {
                    booking.Payment = new Payment
                    {
                        BookingId = booking.Id,
                        Amount = booking.TotalAmount,
                        StatusId = STATUS_PENDING,
                        TransactionId = paymentId,
                        PaymentUrl = paymentUrl,
                        CreatedAt = DateTime.UtcNow
                    };
                }
                else
                {
                    booking.Payment.TransactionId = paymentId;
                    booking.Payment.PaymentUrl = paymentUrl;
                    booking.Payment.StatusId = STATUS_PENDING;
                }

                await _bookingRepository.UpdateAsync(booking);

                return new PaymentInitiationDto
                {
                    Success = true,
                    PaymentId = booking.Payment.Id, 
                    PaymentUrl = paymentUrl
                };
            }
            catch (Exception ex)
            {
                return new PaymentInitiationDto { Success = false, Error = ex.Message };
            }
        }

        public async Task<bool> ConfirmPaymentAndGenerateTicketAsync(int bookingId, string transactionId)
        {
            using var transaction = await _bookingRepository.BeginTransactionAsync();

            try
            {
                var booking = await _bookingRepository.GetByIdWithDetailsAsync(bookingId);

                if (booking == null)
                    return false;

                if (booking.Payment?.StatusId != STATUS_PENDING)
                    return false;

                if (booking.User.LoyaltyAccount == null)
                {
                    booking.User.LoyaltyAccount = new LoyaltyAccount { Balance = 0 };
                }
                if (booking.BonusUsed == 0)
                    booking.User.LoyaltyAccount.Balance += booking.FinalAmount * 0.05;
                else
                    booking.User.LoyaltyAccount.Balance -= booking.BonusUsed;
                booking.Payment.StatusId = STATUS_PAID;
                booking.StatusId = STATUS_CONFIRMED;

                var ticket = await _ticketService.GenerateTicketAsync(booking.Id);

                await _bookingRepository.SaveChangesAsync();

                await transaction.CommitAsync();

                return true;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public async Task<RefundResponseDto> ProcessRefundAsync(int bookingId, string reason, int userId)
        {
            using var transaction = await _bookingRepository.BeginTransactionAsync();

            try
            {
                var booking = await _bookingRepository.GetByIdWithDetailsAsync(bookingId);

                if (booking == null)
                    return new RefundResponseDto { Status = "error", ErrorMessage = "Бронирование не найдено" };

                if (booking.UserId != userId)
                    return new RefundResponseDto { Status = "error", ErrorMessage = "Нет доступа" };

                if (booking.StatusId != STATUS_CONFIRMED)
                    return new RefundResponseDto { Status = "error", ErrorMessage = "Возврат невозможен для этого бронирования" };

                if (booking.Ticket != null && booking.Ticket.IsUsed)
                    return new RefundResponseDto { Status = "error", ErrorMessage = "Билет уже использован" };

                var sessionStart = booking.Session?.StartTime;
                if (sessionStart.HasValue && sessionStart.Value < DateTime.UtcNow)
                    return new RefundResponseDto { Status = "error", ErrorMessage = "Сеанс уже прошёл, возврат невозможен" };

                var payment = booking.Payment;
                if (payment == null || payment.StatusId != STATUS_PAID)
                    return new RefundResponseDto { Status = "error", ErrorMessage = "Платёж не найден или не оплачен" };

                //if (sessionStart.HasValue && sessionStart.Value.AddMinutes(30) < DateTime.UtcNow)
                //    return new RefundResponseDto { Status = "error", ErrorMessage = "Возврат невозможен" };

                var (success, refundId, error) = await _yookassaService.CreateRefundAsync(
                    payment.TransactionId,
                    (decimal)booking.FinalAmount,
                    reason
                );

                if (!success)
                    return new RefundResponseDto { Status = "failed", ErrorMessage = error ?? "Ошибка возврата" };

                payment.StatusId = STATUS_REFUNDED;
                booking.StatusId = STATUS_CANCELLED;

                if (booking.BonusUsed > 0)
                {
                    booking.User.LoyaltyAccount.Balance += booking.BonusUsed;
                }
                else
                {
                    var earnedBonuses = booking.FinalAmount * 0.05;
                    if (earnedBonuses > 0)
                    {
                        if (booking.User.LoyaltyAccount.Balance - earnedBonuses < 0)
                            booking.User.LoyaltyAccount.Balance = 0;
                        else
                            booking.User.LoyaltyAccount.Balance -= earnedBonuses;
                    }
                }

                if (booking.Ticket != null)
                {
                    booking.Ticket.IsUsed = true;
                }

                await _bookingRepository.SaveChangesAsync();
                await transaction.CommitAsync();

                return new RefundResponseDto
                {
                    RefundId = refundId != null ? 0 : 0,
                    RefundTransactionId = refundId ?? string.Empty,
                    Amount = (decimal)booking.TotalAmount,
                    Status = "completed",
                    CreatedAt = DateTime.UtcNow
                };
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return new RefundResponseDto { Status = "error", ErrorMessage = ex.Message };
            }
        }
    }
}