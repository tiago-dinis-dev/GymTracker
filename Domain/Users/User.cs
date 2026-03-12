using Domain.Common;

namespace Domain.Users;

public class User : AggregateRoot
{
    public Guid UserId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public float? Weight { get; private set; }
    public float? Height { get; private set; }

    private User() { }

    public User(Guid userId, string name, string email, float? weight = null, float? height = null)
    {
        UserId = userId;
        Name = name;
        Email = email;
        Weight = weight;
        Height = height;
    }

    public void UpdateProfile(string? name = null, string? email = null, float? weight = null, float? height = null)
    {
        if (!string.IsNullOrWhiteSpace(name)) Name = name;
        if (!string.IsNullOrWhiteSpace(email)) Email = email;
        if (weight.HasValue) Weight = weight.Value;
        if (height.HasValue) Height = height.Value;
    }
}

