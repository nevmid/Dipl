using Backend.Interfaces;
using Backend.Models.DTOs.TicketDTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;

namespace Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ScanController : ControllerBase
    {
        private readonly ITicketService _ticketService;

        public ScanController(ITicketService ticketService)
        {
            _ticketService = ticketService;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ScanTicket([FromQuery] string token)
        {
            try
            {
                var validation = await _ticketService.ValidateTicketAsync(token);

                if (!validation.IsValid)
                {
                    return BadRequest(new { success = false, message = validation.ErrorMessage });
                }

                return Ok(new { success = true, validation.Ticket });
            }
            catch(Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPost("confirm")]
        [Authorize(Roles = "admin,staff")]
        public async Task<IActionResult> ConfirmEntry([FromBody] ConfirmEntryRequestDto request)
        {
            var staffUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var result = await _ticketService.MarkTicketAsUsedAsync(request.Token);

            if (!result)
                return BadRequest(new { message = "Не удалось подтвердить вход" });

            return Ok(new { success = true, message = "Вход подтверждён" });
        }
    }
}