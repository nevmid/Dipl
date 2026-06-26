using Backend.Interfaces;
using Backend.Models.DTOs.ReportsDTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "admin")]
    public class ReportsController : ControllerBase
    {
        private readonly IReportService _reportService;

        public ReportsController(IReportService reportService)
        {
            _reportService = reportService;
        }

        [HttpGet("sales")]
        public async Task<IActionResult> GetSalesReport(
            [FromQuery] DateTime? startDate,
            [FromQuery] DateTime? endDate)
        {
            try
            {
                var utcStartDate = startDate.HasValue
                    ? (DateTime?)DateTime.SpecifyKind(startDate.Value, DateTimeKind.Utc)
                    : null;
                var utcEndDate = endDate.HasValue
                    ? (DateTime?)DateTime.SpecifyKind(endDate.Value, DateTimeKind.Utc)
                    : null;

                var report = await _reportService.GetSalesReportAsync(startDate, endDate);
                return Ok(report);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Server error: {ex.Message}" });
            }
        }

        [HttpGet("hall-load")]
        public async Task<IActionResult> GetHallLoadReport(
            [FromQuery] DateTime? startDate,
            [FromQuery] DateTime? endDate)
        {
            try
            {
                var utcStartDate = startDate.HasValue
                    ? (DateTime?)DateTime.SpecifyKind(startDate.Value, DateTimeKind.Utc)
                    : null;
                var utcEndDate = endDate.HasValue
                    ? (DateTime?)DateTime.SpecifyKind(endDate.Value, DateTimeKind.Utc)
                    : null;
                Console.WriteLine(startDate);
                Console.WriteLine(endDate);
                var report = await _reportService.GetHallLoadReportAsync(startDate, endDate);
                return Ok(report);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Server error: {ex.Message}" });
            }
        }

        [HttpGet("movie-popularity")]
        public async Task<IActionResult> GetMoviePopularityReport(
            [FromQuery] DateTime? startDate,
            [FromQuery] DateTime? endDate,
            [FromQuery] int limit = 10)
        {
            try
            {
                var utcStartDate = startDate.HasValue
                    ? (DateTime?)DateTime.SpecifyKind(startDate.Value, DateTimeKind.Utc)
                    : null;
                var utcEndDate = endDate.HasValue
                    ? (DateTime?)DateTime.SpecifyKind(endDate.Value, DateTimeKind.Utc)
                    : null;

                var report = await _reportService.GetMoviePopularityReportAsync(startDate, endDate, limit);
                return Ok(report);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Server error: {ex.Message}" });
            }
        }

        [HttpGet("daily")]
        public async Task<IActionResult> GetDailyReport([FromQuery] DateTime? date)
        {
            try
            {
                var utcStartDate = date.HasValue
                    ? (DateTime?)DateTime.SpecifyKind(date.Value, DateTimeKind.Utc)
                    : null;

                var targetDate = date ?? DateTime.UtcNow.Date;
                var report = await _reportService.GetDailyReportAsync(targetDate);
                return Ok(report);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Server error: {ex.Message}" });
            }
        }

        [HttpGet("summary")]
        public async Task<IActionResult> GetSummary()
        {
            try
            {
                var summary = await _reportService.GetSummaryAsync();
                return Ok(summary);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Server Error: {ex.Message}" });
            }
        }
    }
}