using ProjectService.Application.Repositories;
using ProjectService.Application.Services;
using ProjectService.Infrastructure.Configuration;
using ProjectService.Infrastructure.Persistence;
using ProjectService.Infrastructure.Repositories;

namespace ProjectService;

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
        
        // MongoDB configuration - читаємо з environment variables
        builder.Services.Configure<MongoDbSettings>(options =>
        {
            options.ConnectionString = builder.Configuration["MongoSettings:ConnectionString"] 
                                       ?? throw new InvalidOperationException("MongoSettings:ConnectionString is required");
            options.DatabaseName = builder.Configuration["MongoSettings:DatabaseName"] 
                                   ?? throw new InvalidOperationException("MongoSettings:DatabaseName is required");
        });


        // builder.Services.Configure<UserServiceSettings>(
        //     builder.Configuration.GetSection(UserServiceSettings.SectionName));

        builder.Services.AddSingleton<MongoDbContext>();
        builder.Services.AddScoped<IProjectRepository, ProjectRepository>();
        builder.Services.AddScoped<IUserSettingsRepository, UserSettingsRepository>();
        builder.Services.AddScoped<IProjectService, ProjectService.Application.Services.ProjectService>();
        builder.Services.AddScoped<IUserSettingsService, UserSettingsService>();
        builder.Services.AddScoped<IIndicatorAnalyticsService, IndicatorAnalyticsService>();
        
        builder.Services.AddHttpClient<UserServiceClient>(client =>
        {
            var baseUrl = builder.Configuration["UserService:BaseUrl"] 
                          ?? throw new InvalidOperationException("UserService:BaseUrl is required");
            client.BaseAddress = new Uri(baseUrl);
            client.Timeout = TimeSpan.FromSeconds(30);
        });

        var app = builder.Build();

        // Тестування підключення до MongoDB
        using (var scope = app.Services.CreateScope())
        {
            try
            {
                var mongoContext = scope.ServiceProvider.GetRequiredService<MongoDbContext>();
                var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        
                await DatabaseInitializer.InitializeAsync(mongoContext);
        
                var status = await DatabaseInitializer.GetDatabaseStatusAsync(mongoContext);
                logger.LogInformation("MongoDB status: {Status}", status);
            }
            catch (Exception ex)
            {
                var logger = app.Services.GetRequiredService<ILogger<Program>>();
                logger.LogError(ex, "MongoDB initialization failed");
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

        app.MapGet("/health", async (MongoDbContext context) =>
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