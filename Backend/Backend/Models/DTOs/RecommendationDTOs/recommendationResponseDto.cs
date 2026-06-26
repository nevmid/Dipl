using Backend.Models.DTOs.RecommendationDTOs;

namespace Backend.Models.DTOs.RecommendationDTOs
{
    public class RecommendationsResponseDto
    {
        public List<RecommendationDto> Personalized { get; set; } = [];
        public List<RecommendationDto> Collaborative { get; set; } = [];
        public List<RecommendationDto> Trending { get; set; } = [];
    }
}
