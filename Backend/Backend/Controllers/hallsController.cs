using System.ComponentModel.DataAnnotations;
using Backend.Interfaces;
using Backend.Models.DTOs.HallDTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HallsController : ControllerBase
    {
        private readonly IHallService _hallService;

        public HallsController(IHallService hallService)
        {
            _hallService = hallService;
        }

        [HttpGet]
        public async Task<IActionResult> GetHalls()
        {
            try
            {
                var halls = await _hallService.GetHalls();

                return Ok(new { success = true, halls });
            }
            catch (Exception)
            {
                return StatusCode(500, new { success = false, message = "Server Error | Get Halls" });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetInfoAboutHall(int id)
        {
            try
            {
                var hall = await _hallService.GetHallById(id);

                if (hall == null)
                    return BadRequest(new {success = false, message = "Зал не найден" });

                return Ok(new { success = true, hall });
            }
            catch (Exception)
            {
                return StatusCode(500, new { success = false, message = "Server Error | Get Info About Hall" });
            }
        }

        [HttpPost]
        [Authorize(Roles ="admin")]
        public async Task<IActionResult> CreateHall([FromBody] RequestHallDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            try
            {
                var hall = await _hallService.CreateHall(request);

                return Ok(new
                {
                    message = "Зал добавлен",
                    hall
                });
            }
            catch(ValidationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception)
            {
                return StatusCode(500, "Server Error | Create Hall");
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> UpdateHall(int id, [FromBody] RequestHallDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            try
            {
                var hall = await _hallService.UpdateHall(id, request);

                if (hall == null)
                    return BadRequest(new { message = "Зал не найден" });

                return Ok(new
                {
                    message = "Зал обновлён",
                    hall
                });
            }
            catch (ValidationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception)
            {
                return StatusCode(500, "Server Error | Update Hall");
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteHall(int id)
        {
            try
            {
                var success = await _hallService.DeleteHall(id);

                if (!success)
                    return BadRequest(new { message = "Зал не найден" });

                return Ok(new
                {
                    message = "Зал удалён",
                    success
                });
            }
            catch (Exception)
            {
                return StatusCode(500, "Server Error | Delete Hall");
            }
        }
    }
}
