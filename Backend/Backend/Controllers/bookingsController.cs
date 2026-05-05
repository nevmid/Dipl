using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Backend.Interfaces;
using Backend.Models.DTOs.BookingDTOs;

namespace Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookingsController : ControllerBase
    {
        private readonly IBookingService _bookingService;

        public BookingsController(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }
        [HttpGet]
        public async Task<IActionResult> GetUserBookings()
        {
            try
            {
                var userId = GetCurrentUserId();
                var bookings = await _bookingService.GetUserBookingsAsync(userId);
                return Ok(bookings);
            }
            catch (Exception)
            {
                return StatusCode(500, "Server Error | Get User Bookings");
            }
        }

        [HttpGet("all")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllBookings()
        {
            try
            {
                var bookings = await _bookingService.GetAllBookingsAsync();
                return Ok(bookings);
            }
            catch (Exception)
            {
                return StatusCode(500, "Server Error | Get All Bookings");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetBooking(int id)
        {
            try
            {
                var userId = GetCurrentUserId();
                var booking = await _bookingService.GetBookingAsync(id, userId);

                if (booking == null)
                    return NotFound(new { Error = "Бронирование не найдено" });

                return Ok(booking);
            }
            catch (Exception)
            {
                return StatusCode(500, "Server Error | Get Booking");
            }
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateBooking([FromBody] CreateBookingDto dto)
        {
            try
            {
                var userId = GetCurrentUserId();
                var booking = await _bookingService.CreateBookingAsync(userId, dto);

                if (booking == null)
                    return BadRequest(new { message = "Ошибка при создании бронирования" });

                return Ok(booking);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, "Server Error | Create Booking");
            }
        }

        [HttpPost("{id}/cancel")]
        [Authorize]
        public async Task<IActionResult> CancelBooking(int id)
        {
            try
            {
                var userId = GetCurrentUserId();
                var result = await _bookingService.CancelBookingAsync(id, userId);

                if (!result)
                    return NotFound(new { Error = "Бронирование не найдено или не модет быть отменено" });

                return Ok(new { message = "Бронирование отменено" });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, "Server Error | Cancel Booking");
            }
        }

        [HttpPost("payment")]
        [Authorize]
        public async Task<IActionResult> ProcessPayment([FromBody] ProcessPaymentDto dto)
        {
            try
            {
                var userId = GetCurrentUserId();
                var payment = await _bookingService.ProcessPaymentAsync(dto, userId);

                if (payment == null)
                    return NotFound(new { Error = "Бронирование не найдено" });

                return Ok(payment);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, "Server Error | Process Payment");
            }
        }

        [HttpGet("{id}/payment-status")]
        public async Task<IActionResult> GetPaymentStatus(int id)
        {
            try
            {
                var userId = GetCurrentUserId();
                var payment = await _bookingService.GetPaymentStatusAsync(id, userId);

                if (payment == null)
                    return NotFound(new { message = "Платёж не найден" });

                return Ok(payment);
            }
            catch (Exception)
            {
                return StatusCode(500, "Server Error | Get Payment Status");
            }
        }

        private int GetCurrentUserId()
        {
            return int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        }
    }
}
