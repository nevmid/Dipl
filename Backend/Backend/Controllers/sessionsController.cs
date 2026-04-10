using System.ComponentModel.DataAnnotations;
using Backend.Interfaces;
using Backend.Models.DTOs.SessionDTOs;
using Backend.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SessionsController : ControllerBase
    {
        private readonly ISessionService _sessionService;

        public SessionsController(ISessionService sessionService)
        {
            _sessionService = sessionService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var sessions = await _sessionService.GetAll();

                return Ok(sessions);
            }
            catch (Exception)
            {
                return StatusCode(500, "Server Error | Get Sesions");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetInfoAboutSession(int id)
        {
            try
            {
                var session = await _sessionService.GetInfoAboutSession(id);

                if (session == null)
                    return BadRequest(new { message = "Сеанс не найден" });

                 return Ok(session);
            }
            catch (Exception)
            {
                return StatusCode(500, "Server Error | Get Info About Session");
            }
        }

        [HttpPost]
        [Authorize(Roles="admin")]
        public async Task<IActionResult> CreateSession([FromBody] CreateSessionDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            try
            {
                var session = await _sessionService.CreateSession(request);

                return Ok(new
                {
                    message = "Сеанс добавлен",
                    session
                });
            }
            catch (ValidationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception)
            {
                return StatusCode(500, "Server Error | Create Session");
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> UpdateSession(int id, [FromBody] UpdateSessionDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            try
            {
                var session = await _sessionService.UpdateSession(id, request);

                if (session == null)
                    return BadRequest(new { message = "Сеанс не найден" });

                return Ok(new
                {
                    message = "Сеанс обновлен",
                    session
                });
            }
            catch(InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (ValidationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception)
            {
                return StatusCode(500, "Server Error | Update Session");
            }
        }
    }
}
