using Backend.Models.DTOs.RecommendationDTOs;

namespace Backend.Interfaces
{
    public interface IRecommendationService
    {
        public Task<RecommendationsResponseDto> GetRecommendationsForUser(int? userId);
    }
}
