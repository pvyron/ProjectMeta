namespace ProMe.Abstractions;
public interface IAuthenticationService
{
    ValueTask<(string key, string salt)> HashPassword(string password);
    ValueTask<bool> AuthenticatePasswordForEmail(string input, string key, string salt);
}
