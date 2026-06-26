namespace Backend.Models.DTOs.ReportsDTOs
{
    public class SalesReportDto
    {
        public DateTime Date { get; set; }
        public int TotalBookings { get; set; }
        public int TotalTickets { get; set; }
        public double TotalIncome { get; set; }
        public double AverageTicketPrice { get; set; }
    }

    public class HallLoadReportDto
    {
        public int HallId { get; set; }
        public string HallName { get; set; }
        public int TotalSeats { get; set; }
        public int TotalSessions { get; set; }
        public int TotalTicketsSold { get; set; }
        public double LoadPercentage { get; set; }
    }

    public class MoviePopularityReportDto
    {
        public int MovieId { get; set; }
        public string MovieTitle { get; set; }
        public string PosterUrl { get; set; }
        public int TotalTickets { get; set; }
        public int Year {  get; set; }
        public double TotalIncome { get; set; }
        public int TotalSessions { get; set; }
        public double TicketsPerSession { get; set; }
    }

    public class DailyReportDto
    {
        public DateTime Date { get; set; }
        public int BookingsCount { get; set; }
        public int TicketsSold { get; set; }
        public double Income { get; set; }
        public int BonusPointsUsed { get; set; }
        public double DiscountAmount { get; set; }
    }
}