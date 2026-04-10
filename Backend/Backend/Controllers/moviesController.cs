using System.ComponentModel.DataAnnotations;
using System.Transactions;
using Backend.Interfaces;
using Backend.Models.DTOs.MovieDTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MoviesController : ControllerBase
    {
        private readonly IMovieService _movieService;

        public MoviesController(IMovieService movieService)
        {
            _movieService = movieService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetInfoAboutMovie(int id)
        {
            try
            {
                var movie = await _movieService.GetInfoAboutMovie(id);

                if (movie == null)
                {
                    return BadRequest(new {success = false,
                        message = "Фильм не найден" });
                }

                return Ok(new { success = true, movie });
            }
            catch (Exception)
            {
                return StatusCode(500, new {success = false, 
                    message = "Server Error | Get Info About Movie" });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetMovies()
        {
            try
            {
                var movies = await _movieService.GetMovies();

                return Ok(new { success = true, movies });
            }
            catch (Exception)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Server Error | Get Movies"
                });
            }
        }

        [HttpPost]
        [Authorize(Roles="admin")]
        public async Task<IActionResult> CreateMovie([FromForm] CreateMovieDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new {success = false, 
                    message = "Ошибка валидации данных фильма"});
            }

            try
            {
                var movie = await _movieService.CreateMovie(request);

                return Ok(new
                {
                    success = true,
                    message = "Фильм добавлен",
                    movie
                });
            }
            catch(ValidationException ex)
            {
                return BadRequest(new {success = false, message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500,
                    new { success = false,
                        message = "Server Error | Create Movie" });
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> UpdateMovie(int id, [FromForm] UpdateMovieDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Ошибка валидации данных фильма"
                });
            }
            try
            {
                var updatedMovie = await _movieService.UpdateMovie(id, request);

                if (updatedMovie == null)
                {
                    return BadRequest(new {success = false,
                        message = "Фильм не найден" });
                }

                return Ok(new
                {
                    success = true,
                    message = "Фильм обновлен",
                    updatedMovie
                });
            }
            catch (ValidationException ex)
            {
                return BadRequest(new { success = false,
                    message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500,
                    new { success = false, message = "Server Error | Update Movie" });
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles="admin")]
        public async Task<IActionResult> DeleteMovie(int id)
        {
            try
            {
                var success = await _movieService.DeleteMovie(id);

                if (!success)
                    return BadRequest(new 
                    {
                        success = false,
                        message = "Ошибка удаления фильма"
                    });

                return Ok(success);
            }
            catch (Exception)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Server Error | Delete Movie"
                });
            }
        }
    }
}
