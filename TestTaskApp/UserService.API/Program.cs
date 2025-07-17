
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using UserService.Application.Repositories;
using UserService.Application.Services;
using UserService.Infrastructure.Persistence;
using UserService.Infrastructure.Repositories;

namespace UserService;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        builder.Services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        
        builder.Services.ConfigureHttpJsonOptions(options =>
        {
            options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
        });
        
        builder.Services.AddDbContext<UserDbContext>(options =>
            options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
        
        builder.Services.AddScoped<IUserRepository, UserRepository>();
        builder.Services.AddScoped<IUserService, Application.Services.UserService>();
        
        builder.Services.AddScoped<ISubscriptionRepository, SubscriptionRepository>();
        builder.Services.AddScoped<ISubscriptionService, SubscriptionService>();

        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowAll", policy =>
            {
                policy.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader();
            });
        });
        
        var app = builder.Build();
        
        using (var scope = app.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<UserDbContext>();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    
            try
            {
                logger.LogInformation("Checking database and tables...");
                await DatabaseInitializer.InitializeAsync(context);
                var statusAfter = await DatabaseInitializer.GetDatabaseStatusAsync(context);
                logger.LogInformation("Database status after initialization: {Status}", statusAfter);
                
                logger.LogInformation("Database check completed.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Database initialization failed.");
                throw;
            }
        }

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();


        app.MapControllers();

        app.MapGet("/health", async (UserDbContext context) =>
        {
            try
            {
                var isInitialized = await DatabaseInitializer.IsDatabaseInitializedAsync(context);
                return Results.Ok(new 
                { 
                    Status = "Healthy",
                    DatabaseInitialized = isInitialized,
                    Timestamp = DateTime.UtcNow 
                });
            }
            catch (Exception ex)
            {
                return Results.Problem($"Health check failed: {ex.Message}");
            }
        });
        
        app.Run();
    }
}
