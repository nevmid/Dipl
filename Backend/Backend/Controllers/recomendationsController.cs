using Backend.Interfaces;
using Backend.Models.DTOs.RecommendationDTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RecommendationsController : ControllerBase
    {
        private readonly IRecommendationService _recommendationService;

        public RecommendationsController(IRecommendationService recommendationService)
        {
            _recommendationService = recommendationService;
        }

        [HttpGet("for-user")]
        [AllowAnonymous]
        public async Task<ActionResult<RecommendationsResponseDto>> GetRecommendationsForUser()
        {
            try
            {
                int? userId = null;

                if (User.Identity?.IsAuthenticated == true)
                {
                    var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                    if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int id))
                    {
                        userId = id;
                    }
                }

                var recommendations = await _recommendationService.GetRecommendationsForUser(userId);
                return Ok(recommendations);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }
}