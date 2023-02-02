using RemoteServer.Models;

namespace RemoteServer.Tests;

public class UserTest1
{
    [Fact]
    public void CreateUser()
    {
        Assert.True(true);
    }
    
    [Fact]
    public async void PasswordCreateAndValidate()
    {
        var user = new User
        {
            Username = "test",
            FullName = "test",
            JobTitle = "test"
        };
        const string password = "SuperSecretPassword";
        await user.SetPassword(password);
        Assert.True(await user.IsPasswordValidAsync(password));
        Assert.False(await user.IsPasswordValidAsync("WrongPassword"));
    }
}