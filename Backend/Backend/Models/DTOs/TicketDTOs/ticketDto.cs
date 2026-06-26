namespace Backend.Models.DTOs.TicketDTOs
{
    public class TicketDto
    {
        public int Id { get; set; }
        public int BookingId { get; set; }
        public string TicketNumber { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
        public string ScanUrl { get; set; } = string.Empty;
        public string QrCode { get; set; } = string.Empty;
        public string MovieTitle { get; set; } = string.Empty;
        public DateTime SessionTime { get; set; }
        public string HallName { get; set; } = string.Empty;
        public string UserEmail { get; set; } = string.Empty;
        public bool IsUsed { get; set; }
        public DateTime GeneratedAt { get; set; }
    }

    public class TicketValidationResultDto
    {
        public bool IsValid { get; set; }
        public string? ErrorMessage { get; set; }
        public TicketDto? Ticket { get; set; }
    }

    public class ConfirmEntryRequestDto
    {
        public string Token { get; set; } = string.Empty;
    }
}