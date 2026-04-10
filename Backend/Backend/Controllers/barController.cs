using System.ComponentModel.DataAnnotations;
using Backend.Interfaces;
using Backend.Models.DTOs.BarDTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BarController : ControllerBase
    {
        private readonly IBarService _barService;

        public BarController(IBarService barService)
        {
            _barService = barService;
        }

        [HttpGet("categories")]
        public async Task<IActionResult> GetCategories()
        {
            try
            {
                var categories = await _barService.GetGategories();

                return Ok(categories);
            }
            catch (Exception)
            {
                return StatusCode(500, "Server Error | Get Categories");    
            }
        }

        [HttpGet("items")]
        public async Task<IActionResult> GetItems()
        {
            try
            {
                var items = await _barService.GetBarItems();

                return Ok(items);
            }
            catch (Exception)
            {
                return StatusCode(500, "Server Error | Get Items");
            }
        }

        [HttpPost("category")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> CreateCategory([FromBody] string name)
        {
            try
            {
                var category = await _barService.CreateCategory(name);

                return Ok(new
                {
                    mesage = "Категория добавлена",
                    category
                });
            }
            catch(ValidationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception)
            {
                return StatusCode(500, "Server Error | Create Category");
            }
        }

        [HttpPost("item")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> CreateItem([FromBody] barItemDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            try
            {
                var item = await _barService.CreateBarItem(request);

                return Ok(new
                {
                    mesage = "Товар добавлен",
                    item
                });
            }
            catch (ValidationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception)
            {
                return StatusCode(500, "Server Error | Create Item");
            }
        }

        [HttpPut("category/{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> UpdateCategory(int id, [FromBody] string name)
        {
            try
            {
                var category = await _barService.UpdateCategory(id, name);

                if (category == null)
                    return BadRequest(new { message = "Категория не найдена" });

                return Ok(new
                {
                    mesage = "Категория обновлена",
                    category
                });
            }
            catch (ValidationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception)
            {
                return StatusCode(500, "Server Error | Update Category");
            }
        }

        [HttpPut("item/{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> UpdateItem(int id, [FromBody] barItemDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            try
            {
                var item = await _barService.UpdateBarItem(id, request);

                if (item == null)
                    return BadRequest(new { message = "Товар не найден" });

                return Ok(new
                {
                    mesage = "Товар обновлён",
                    item
                });
            }
            catch (ValidationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception)
            {
                return StatusCode(500, "Server Error | Update Item");
            }
        }

        [HttpDelete("category/{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            try
            {
                var success = await _barService.DeleteCategory(id);

                if(!success)
                    return BadRequest(new { message = "Категория не найдена" });

                return Ok(new
                {
                    mesage = "Категория удалена",
                    success
                });
            }
            catch (Exception)
            {
                return StatusCode(500, "Server Error | Delete Category");
            }
        }

        [HttpDelete("item/{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteItem(int id)
        {
            try
            {
                var success = await _barService.DeleteBarItem(id);

                if (!success)
                    return BadRequest(new { message = "Товар не найден" });

                return Ok(new
                {
                    mesage = "Товар удален",
                    success
                });
            }
            catch (Exception)
            {
                return StatusCode(500, "Server Error | Delete Item");
            }
        }
    }
}
