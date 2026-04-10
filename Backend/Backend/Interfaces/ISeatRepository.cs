using Backend.Models.Entities;

namespace Backend.Interfaces
{
    public interface ISeatRepository
    {
        public Task CreateSeat(Seat seat);
        public Task DeleteSeats(int hallId);
        public Task<List<Seat>> GetSeatsByIdsAsync(List<int> seatsIds);
    }
}
