using Backend.Models.DTOs.ReportsDTOs;

namespace Backend.Interfaces
{
    public interface IReportService
    {
        public Task<List<SalesReportDto>> GetSalesReportAsync(DateTime? startDate, DateTime? endDate);
        public Task<List<HallLoadReportDto>> GetHallLoadReportAsync(DateTime? startDate, DateTime? endDate);
        public Task<List<MoviePopularityReportDto>> GetMoviePopularityReportAsync(DateTime? startDate, DateTime? endDate, int limit);
        public Task<DailyReportDto> GetDailyReportAsync(DateTime date);
        public Task<object> GetSummaryAsync();
    }
}
