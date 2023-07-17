using LanguageExt;
using LanguageExt.Common;
using Microsoft.AspNetCore.Components.Authorization;
using ProMe.ApiContracts.Auth;
using ProMe.NativeApplication.Models.Exceptions;
using System.Diagnostics;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text.Json;

namespace ProMe.NativeApplication.Services;
public class IdentityAuthenticationStateProvider : AuthenticationStateProvider
{
    const string ACCESS_TOKEN_STORAGE_KEY = "accesstoken";
    const string REFRESH_TOKEN_STORAGE_KEY = "refreshtoken";
#if DEBUG
    const string API_LOGIN_URL = "https://localhost:7076/Login";
#endif
#if RELEASE
    const string API_LOGIN_URL = "https://localhost:7076/Login";
#endif

    private readonly HttpClient _apiClient;
    private readonly JsonSerializerOptions _jsonSerializerOptions;

    public IdentityAuthenticationStateProvider(JsonSerializerOptions jsonSerializerOptions)
    {
        _apiClient = new HttpClient();
        _jsonSerializerOptions = jsonSerializerOptions;
    }

    public async ValueTask<Result<Unit>> Login(LoginCredentials loginCredentials)
    {
        try
        {
            var result = await _apiClient.PostAsJsonAsync(
            API_LOGIN_URL,
            new LoginRequestModel
            {
                Email = loginCredentials.Email,
                Password = loginCredentials.Password,
                RememberMe = loginCredentials.RememberMe,
            },
            _jsonSerializerOptions);

            if (!(result?.IsSuccessStatusCode ?? false))
            {
                return new Result<Unit>(new AuthenticationFailedException($"{result.ReasonPhrase}: {await result.Content.ReadAsStringAsync()}"));
            }
            else
            {
                var response = await result.Content.ReadFromJsonAsync<LoginResponseModel>(_jsonSerializerOptions);

                if (response is null)
                {
                    return new Result<Unit>(new AuthenticationFailedException("Something weird"));
                }

                return await Login(new AuthorizationData { AccessToken = response.BearerToken, RefreshToken = response.RefreshToken });
            }
        }
        catch (Exception ex)
        {
            return new Result<Unit>(ex);
        }
    }

    public async ValueTask<Result<Unit>> Login(AuthorizationData authorizationData)
    {
        try
        {
            await SecureStorage.SetAsync(ACCESS_TOKEN_STORAGE_KEY, authorizationData.AccessToken);
            await SecureStorage.SetAsync(REFRESH_TOKEN_STORAGE_KEY, authorizationData.RefreshToken);
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());

            return Unit.Default;
        }
        catch (Exception ex)
        {
            return new Result<Unit>(ex);
        }
    }

    public async ValueTask<Result<Unit>> Logout()
    {
        try
        {
            SecureStorage.Remove(ACCESS_TOKEN_STORAGE_KEY);
            SecureStorage.Remove(REFRESH_TOKEN_STORAGE_KEY);
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());

            return Unit.Default;
        }
        catch (Exception ex)
        {
            return new Result<Unit>(ex);
        }
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var identity = new ClaimsIdentity();
        try
        {
            var userInfo = await SecureStorage.GetAsync(ACCESS_TOKEN_STORAGE_KEY);
            if (userInfo != null)
            {
                var claims = new[] { new Claim(ClaimTypes.Name, "ffUser") };
                identity = new ClaimsIdentity(claims, "Server authentication");
            }
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine("Request failed:" + ex.ToString());
        }

        return new AuthenticationState(new ClaimsPrincipal(identity));
    }
}

public record LoginCredentials
{
    public required string Email { get; init; }
    public required string Password { get; init; }
    public required bool RememberMe { get; init; }
}

public record AuthorizationData
{
    public required string AccessToken { get; init; }
    public required string RefreshToken { get; init; }
}