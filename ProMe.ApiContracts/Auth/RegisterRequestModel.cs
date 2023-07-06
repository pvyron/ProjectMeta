namespace ProMe.ApiContracts.Auth;
public sealed class RegisterRequestModel
{
    public string Email { get; set; } = null!;
    public string OriginalPassword { get; set; } = null!;
    public string RepeatPassword { get; set; } = null!;
}
