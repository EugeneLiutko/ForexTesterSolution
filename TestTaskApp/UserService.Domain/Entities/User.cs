namespace UserService.Domain.Entities;

public class User
{
    public int Id { get; private set; }
    public string Name { get; private set; }
    public string Email { get; private set; }
    public int SubscriptionId { get; private set; }
    public Subscription Subscription { get; private set; }

    private User() { }
    public User(string name, string email, int subscriptionId)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Email = email ?? throw new ArgumentNullException(nameof(email));
        SubscriptionId = subscriptionId;
    }
    public void UpdateDetails(string name, string email)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Email = email ?? throw new ArgumentNullException(nameof(email));
    }
}