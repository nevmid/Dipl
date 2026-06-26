using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Backend.Interfaces;
using Backend.Models.DTOs.BookingDTOs;
using System.Text.Json;
using Backend.Models.Entities;
using Backend.Models.DTOs.PaymentDTOs;

namespace Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookingsController : ControllerBase
    {
        private readonly IBookingService _bookingService;
        private readonly ITicketService _ticketService;

        public BookingsController(
            IBookingService bookingService,
            ITicketService ticketService)
        {
            _bookingService = bookingService;
            _ticketService = ticketService;
        }
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetUserBookings()
        {
            try
            {
                var userId = GetCurrentUserId();
                var bookings = await _bookingService.GetUserBookingsAsync(userId);
                return Ok(bookings);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = $"Server Error | Get User Bookings: {ex}" });
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

                return Ok(new { success = true, booking });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Server Error | Create Booking: {ex}" });
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
                    return NotFound(new { message = "Бронирование не найдено или не модет быть отменено" });

                return Ok(new { message = "Бронирование отменено" });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "Server Error | Cancel Booking" });
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

        [HttpGet("{bookingId}/ticket")]
        [Authorize]
        public async Task<IActionResult> GetTicketByBooking(int bookingId)
        {
            try
            {
                var userId = GetCurrentUserId();
                var booking = await _bookingService.GetBookingAsync(bookingId, userId);

                if (booking == null)
                    return NotFound(new { success = false, message = "Бронирование не найдено" });

                var ticket = await _ticketService.GetTicketByBookingIdAsync(bookingId);

                if (ticket == null)
                    return NotFound(new { success = false, message = "Билет еще не сгенерирован" });

                return Ok(new { success = true, ticket });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = $"Server error: {ex.Message}" });
            }
        }

        [HttpPost("{id}/pay")]
        [Authorize]
        public async Task<IActionResult> InitiatePayment(int id)
        {
            try
            {
                var userId = GetCurrentUserId();
                var result = await _bookingService.InitiatePaymentAsync(id, userId);

                if (!result.Success)
                    return BadRequest(new { message = result.Error });

                return Ok(new
                {
                    success = true,
                    paymentUrl = result.PaymentUrl,
                    paymentId = result.PaymentId,
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "Server error" });
            }
        }

        [HttpPost("webhook/yookassa")]
        [AllowAnonymous]
        public async Task<IActionResult> YooKassaWebhook([FromBody] YooKassaWebhookPayload webhook)
        {
            try
            {
                var paymentId = webhook.Object.Id;
                Console.WriteLine($"Event: {webhook.Object.Status}, PaymentId: {paymentId}");
                if (webhook.Object.Status == "succeeded" && webhook.Object.Paid)
                {
                    var metadata = webhook.Object.Metadata;
                    var bookingId = int.Parse(metadata.BookingId);
                    var result = await _bookingService.ConfirmPaymentAndGenerateTicketAsync(bookingId, paymentId!);
                }
                if (webhook.Object.Status == "canceled")
                {
                    var metadata = webhook.Object.Metadata;
                    var bookingId = int.Parse(metadata.BookingId);
                    var userId = int.Parse(metadata.UserId);
                    var result = await _bookingService.CancelBookingAsync(bookingId, userId);
                }
                return Ok();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Webhook error: {ex.Message}");
                return Ok();
            }
        }

        [HttpPost("{id}/refund")]
        [Authorize]
        public async Task<IActionResult> RefundBooking(int id, [FromBody] RefundRequestDto request)
        {
            try
            {
                var userId = GetCurrentUserId();
                var result = await _bookingService.ProcessRefundAsync(id, request.Reason, userId);

                if (result.Status == "completed")
                {
                    return Ok(new
                    {
                        success = true,
                        message = $"Возврат на сумму {result.Amount}₽ успешно выполнен",
                        refundId = result.RefundId,
                        refundTransactionId = result.RefundTransactionId,
                        amount = result.Amount,
                        status = result.Status
                    });
                }

                return BadRequest(new
                {
                    success = false,
                    message = result.ErrorMessage ?? "Не удалось выполнить возврат",
                    status = result.Status
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        private int GetCurrentUserId()
        {
            return int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        }
    }
}
