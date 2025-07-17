using MongoDB.Bson;

namespace ProjectService.Domain.Entities;

public class Project
{
    public ObjectId Id { get; set; }
    public int UserId { get; set; }
    public string Name { get; set; }
    public List<Chart> Charts { get; set; } = new();
}