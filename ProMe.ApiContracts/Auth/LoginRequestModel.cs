namespace ProMe.ApiContracts.Auth;
public sealed class LoginRequestModel
{
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
    public bool? RememberMe { get; set; } = false;
}
