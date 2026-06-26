using System.Text;
using Backend.Infrastructure;
using Backend.Interfaces;
using Backend.Repositories;
using Backend.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddCors();

var jwtSettings = builder.Configuration.GetSection("JwtOptions").Get<JwtOptions>();

var connection = builder.Configuration.GetConnectionString("ApplicationContext");

builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("JwtOptions"));
builder.Services.Configure<YookassaSettings>(builder.Configuration.GetSection("YooKassa"));
builder.Services.AddScoped<JwtProvider>();
builder.Services.AddScoped<PasswordHasher>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ITicketService, TicketService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IBookingService, BookingService>();
builder.Services.AddScoped<IBookingRepository, BookingRepository>();
builder.Services.AddScoped<IMovieService, MovieService>();
builder.Services.AddScoped<IMovieRepository, MovieRepository>();
builder.Services.AddScoped<IHallRepository, HallRepository>();
builder.Services.AddScoped<IHallService, HallService>();
builder.Services.AddScoped<ISessionRepository, SessionRepository>();
builder.Services.AddScoped<ISessionService, SessionService>();
builder.Services.AddScoped<IYookassaService, YooKassaService>();
builder.Services.AddScoped<ISeatRepository, SeatRepository>();
builder.Services.AddScoped<IReportRepository, ReportRepository>();
builder.Services.AddScoped<IReportService, ReportService>();
builder.Services.AddScoped<IRecommendationService, RecommendationService>();
builder.Services.AddHostedService<BookingExpirationService>();


builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseNpgsql(connection);
});

builder.Services.AddAuthorization();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var secretKey = builder.Configuration["JwtOptions:SecretKey"];

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateActor = false,
            ValidateAudience = false,
            ValidateIssuer = false,
            ValidateLifetime = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey!))
        };

        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                context.Token = context.Request.Cookies["FeelsGoodMan"];

                return Task.CompletedTask;
            }
        };
    });

var app = builder.Build();

app.UseCors(policy =>
{
    policy.WithOrigins("http://localhost:5173", "https://6rk3i9-178-141-21-81.ru.tuna.am")
    .AllowAnyHeader()
    .AllowAnyMethod()
    .AllowCredentials();
});
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
app.Run();
