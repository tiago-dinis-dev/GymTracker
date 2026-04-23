using Domain.Users;

namespace Domain.Tests.Users;

public class UserTests
{
    [Fact]
    public void Constructor_SetsAllProperties()
    {
        var userId = Guid.NewGuid();

        var user = new User(userId, "John", "john@test.com", 80f, 180f);

        Assert.Equal(userId, user.UserId);
        Assert.Equal("John", user.Name);
        Assert.Equal("john@test.com", user.Email);
        Assert.Equal(80f, user.Weight);
        Assert.Equal(180f, user.Height);
    }

    [Fact]
    public void Constructor_OptionalFieldsCanBeNull()
    {
        var user = new User(Guid.NewGuid(), "John", "john@test.com");

        Assert.Null(user.Weight);
        Assert.Null(user.Height);
    }

    [Fact]
    public void UpdateProfile_UpdatesName()
    {
        var user = new User(Guid.NewGuid(), "John", "john@test.com");

        user.UpdateProfile(name: "Jane");

        Assert.Equal("Jane", user.Name);
        Assert.Equal("john@test.com", user.Email);
    }

    [Fact]
    public void UpdateProfile_UpdatesEmail()
    {
        var user = new User(Guid.NewGuid(), "John", "john@test.com");

        user.UpdateProfile(email: "jane@test.com");

        Assert.Equal("jane@test.com", user.Email);
        Assert.Equal("John", user.Name);
    }

    [Fact]
    public void UpdateProfile_UpdatesWeightAndHeight()
    {
        var user = new User(Guid.NewGuid(), "John", "john@test.com");

        user.UpdateProfile(weight: 85f, height: 175f);

        Assert.Equal(85f, user.Weight);
        Assert.Equal(175f, user.Height);
    }

    [Fact]
    public void UpdateProfile_NullValues_DoNotOverwrite()
    {
        var user = new User(Guid.NewGuid(), "John", "john@test.com", 80f, 180f);

        user.UpdateProfile();

        Assert.Equal("John", user.Name);
        Assert.Equal("john@test.com", user.Email);
        Assert.Equal(80f, user.Weight);
        Assert.Equal(180f, user.Height);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void UpdateProfile_EmptyOrWhitespaceName_DoesNotUpdate(string? name)
    {
        var user = new User(Guid.NewGuid(), "John", "john@test.com");

        user.UpdateProfile(name: name);

        Assert.Equal("John", user.Name);
    }
}
