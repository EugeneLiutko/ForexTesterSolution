using MongoDB.Bson;
using ProjectService.Domain.Enums;

namespace ProjectService.Domain.Entities;

public class UserSettings
{
    public ObjectId Id { get; set; }
    public int UserId { get; set; }
    public Language Language { get; set; }
    public Theme Theme { get; set; }
}