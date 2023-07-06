namespace ProMe.Models;
public sealed class AuthorizationException : Exception
{
    public AuthorizationException(string message) : base(message)
    {

    }
}
