namespace IntegrationTests.Infrastructure;

public class SharedTestFixture : IAsyncLifetime
{
    public TestWebApplicationFactory<UserService.Program> UserServiceFactory { get; private set; }
    public TestWebApplicationFactory<ProjectService.Program> ProjectServiceFactory { get; private set; }
    public HttpClient UserServiceClient { get; private set; }
    public HttpClient ProjectServiceClient { get; private set; }

    public async Task InitializeAsync()
    {
        // Ініціалізуємо фабрики
        UserServiceFactory = new TestWebApplicationFactory<UserService.Program>();
        ProjectServiceFactory = new TestWebApplicationFactory<ProjectService.Program>();

        // Запускаємо контейнери
        await UserServiceFactory.InitializeAsync();
        await ProjectServiceFactory.InitializeAsync();

        // Створюємо HTTP клієнти
        UserServiceClient = UserServiceFactory.CreateClient();
        ProjectServiceClient = ProjectServiceFactory.CreateClient();

        // Заповнюємо бази даних
        await DatabaseSeeder.SeedUserServiceDatabase(UserServiceFactory.GetPostgreSqlConnectionString());
        await DatabaseSeeder.SeedProjectServiceDatabase(ProjectServiceFactory.GetMongoConnectionString(), "testprojectsdb");

        Console.WriteLine("🚀 SharedTestFixture initialized successfully");
    }

    public async Task DisposeAsync()
    {
        UserServiceClient?.Dispose();
        ProjectServiceClient?.Dispose();
        
        if (ProjectServiceFactory != null)
            await ProjectServiceFactory.DisposeAsync();
            
        if (UserServiceFactory != null)
            await UserServiceFactory.DisposeAsync();

        Console.WriteLine("🧹 SharedTestFixture disposed");
    }
}