using Microsoft.EntityFrameworkCore;

namespace Backend.Infrastructure
{
    public class BookingExpirationService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private const int STATUS_PENDING = 1;
        private const int STATUS_EXPIRED = 9;

        public BookingExpirationService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await CancelExpiredBookings();
                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }

        private async Task CancelExpiredBookings()
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var expiredBookings = await context.Bookings
                .Where(b => b.StatusId == STATUS_PENDING
                    && b.CreatedAt.AddMinutes(15) < DateTime.UtcNow)
                .ToListAsync();

            foreach (var booking in expiredBookings)
            {
                booking.StatusId = STATUS_EXPIRED;

            }

            await context.SaveChangesAsync();
        }
    }
}
