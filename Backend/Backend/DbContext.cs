using Backend.Configurations;
using Backend.Models.Entities;
using Microsoft.EntityFrameworkCore;

public class ApplicationDbContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Movie> Movies { get; set; }
    public DbSet<Hall> Halls { get; set; }
    public DbSet<Status> Statuses { get; set; }
    public DbSet<Ticket> Tickets { get; set; }
    public DbSet<Session> Sessions { get; set; }
    public DbSet<Booking> Bookings{ get; set; }
    public DbSet<Seat> Seats { get; set; }
    public DbSet<BookingSeat> BookingSeats { get; set; }
    public DbSet<LoyaltyAccount> LoyaltyAccounts { get; set; }
    public DbSet<Payment> Payments { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<Genre> Genres { get; set; }
    public DbSet<MovieGenre> MovieGenres { get; set; }
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new UserConfiguration());
        modelBuilder.ApplyConfiguration(new MovieConfiguration());
        modelBuilder.ApplyConfiguration(new StatusCinfiguration());
        modelBuilder.ApplyConfiguration(new SessionConfiguration());
        modelBuilder.ApplyConfiguration(new HallConfiguration());
        modelBuilder.ApplyConfiguration(new TicketConfiguration());
        modelBuilder.ApplyConfiguration(new BookingConfiguration());
        modelBuilder.ApplyConfiguration(new BookingSeatConfiguration());
        modelBuilder.ApplyConfiguration(new SeatConfiguration());
        modelBuilder.ApplyConfiguration(new LoyaltyAccountConfiguration());
        modelBuilder.ApplyConfiguration(new PaymentConfiguration());
        modelBuilder.ApplyConfiguration(new RoleConfiguration());
        modelBuilder.ApplyConfiguration(new GenreConfiguration());
        modelBuilder.ApplyConfiguration(new MovieGenreConfiguration());

        base.OnModelCreating(modelBuilder);
    }
}