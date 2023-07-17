using FluentValidation;
using LanguageExt;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using MudBlazor;
using ProMe.NativeApplication.Services;

namespace ProMe.NativeApplication.Pages;

public partial class Login
{
    [Inject]
    ISnackbar _snackbar { get; set; }

    [Inject]
    IdentityAuthenticationStateProvider _authenticationStateProvider { get; set; }

    [Inject]
    NavigationManager _navigationManager { get; set; }

    MudForm form;
    LoginModelFluentValidator loginValidator = new();
    LoginModel loginModel = new();

    private async Task OnLoginClicked()
    {
        await form.Validate();

        if (!form.IsValid)
        {
            return;
        }

        var result = await _authenticationStateProvider.Login(new LoginCredentials
        {
            Email = loginModel.Email,
            Password = loginModel.Password,
            RememberMe = true
        });

        result.Match(
            Succ =>
            {
                _navigationManager.NavigateTo("/");
                return Unit.Default;
            },
            Fail =>
            {
                _snackbar.Add(Fail.Message, MudBlazor.Severity.Error);
                return Unit.Default;
            });
    }


}

public class LoginModel
{
    public string Email { get; set; } = "";
    public string Password { get; set; } = "";
}

public class LoginModelFluentValidator : AbstractValidator<LoginModel>
{
    public LoginModelFluentValidator()
    {
        RuleFor(x => x.Email)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .EmailAddress();

        RuleFor(x => x.Password)
                .NotEmpty();
    }

    public Func<object, string, Task<IEnumerable<string>>> ValidateValue => async (model, propertyName) =>
    {
        var result = await ValidateAsync(ValidationContext<LoginModel>.CreateWithOptions((LoginModel)model, x => x.IncludeProperties(propertyName)));
        if (result.IsValid)
            return Array.Empty<string>();
        return result.Errors.Select(e => e.ErrorMessage);
    };
}