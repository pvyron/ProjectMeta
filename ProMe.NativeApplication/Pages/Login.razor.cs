using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using ProMe.NativeApplication.Services;

namespace ProMe.NativeApplication.Pages;

public partial class Login
{
    [Inject] 
    ISnackbar Snackbar { get; set; }

    [Inject]
    IdentityAuthenticationStateProvider _authenticationStateProvider { get; set; }

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

        await _authenticationStateProvider.Login(new LoginCredentials
        {
            Email = loginModel.Email,
            Password = loginModel.Password,
            RememberMe = true
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