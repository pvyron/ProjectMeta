namespace ProMe.ApiContracts.Auth;
public sealed class LoginResponseModel
{
    public string? BearerToken { get; set; }
    public string? RefreshToken { get; set; }
}
