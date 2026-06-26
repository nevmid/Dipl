using Backend.Interfaces;
using Backend.Models.DTOs.ReportsDTOs;

namespace Backend.Services
{
    public class ReportService : IReportService
    {
        private readonly IReportRepository _reportRepository;

        public ReportService(IReportRepository reportRepository)
        {
            _reportRepository = reportRepository;
        }

        public async Task<List<SalesReportDto>> GetSalesReportAsync(DateTime? startDate, DateTime? endDate)
        {
            return await _reportRepository.GetSalesReportAsync(startDate, endDate);
        }

        public async Task<List<HallLoadReportDto>> GetHallLoadReportAsync(DateTime? startDate, DateTime? endDate)
        {
            return await _reportRepository.GetHallLoadReportAsync(startDate, endDate);
        }

        public async Task<List<MoviePopularityReportDto>> GetMoviePopularityReportAsync(DateTime? startDate, DateTime? endDate, int limit)
        {
            return await _reportRepository.GetMoviePopularityReportAsync(startDate, endDate, limit);
        }

        public async Task<DailyReportDto> GetDailyReportAsync(DateTime date)
        {
            return await _reportRepository.GetDailyReportAsync(date);
        }

        public async Task<object> GetSummaryAsync()
        {
            return await _reportRepository.GetSummaryAsync();
        }
    }
}