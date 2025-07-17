namespace ProjectService.Infrastructure.Configuration;

public class MongoDbSettings
{
    public const string SectionName = "MongoSettings";

    public string ConnectionString { get; set; } = string.Empty;
    public string DatabaseName { get; set; } = string.Empty;
}