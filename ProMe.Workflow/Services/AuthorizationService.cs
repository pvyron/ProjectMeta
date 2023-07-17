using Azure.Data.Tables;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using ProMe.Abstractions;
using ProMe.DataAccess.Models;
using ProMe.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ProMe.Workflow.Services;
internal class AuthorizationService : IAuthorizationService
{
    private readonly string _signingSecret;
    private readonly TableClient _tableClient;

    public AuthorizationService(IConfiguration configuration)
    {
        _signingSecret = configuration.GetValue<string>("Authorization:JwtSigningSecret")!;
        _tableClient = new TableClient(configuration.GetConnectionString("AuthStorage"), "RefreshTokens");
    }

    public ValueTask<string> GenerateAccessTokenForUser(Guid userId)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_signingSecret);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[] { new Claim("UserId", userId.ToString()) }),
#if RELEASE
            Expires = DateTime.UtcNow.AddMinutes(15),
#endif
#if DEBUG
            Expires = DateTime.UtcNow.AddHours(15),
#endif
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha512Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);

        return ValueTask.FromResult(tokenHandler.WriteToken(token));
    }

    public async ValueTask<string> GenerateRefreshTokenForUser(Guid userId)
    {
        var refresh = Guid.NewGuid().ToString();

        await _tableClient.UpsertEntityAsync(new RefreshTokenTableEntity
        {
            PartitionKey = userId.ToString(),
            RowKey = refresh,
            ValidUntil = DateTime.UtcNow.AddDays(15),
        });

        return refresh;
    }

    public async ValueTask<(string accessToken, string refreshToken)> RefreshSession(string accessTokenOld, string refreshToken)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_signingSecret);
        tokenHandler.ValidateToken(accessTokenOld, new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = false,
        }, out SecurityToken validatedToken);

        var jwtToken = (JwtSecurityToken)validatedToken;

        var userId = Guid.Parse(jwtToken.Claims.First(x => x.Type == "UserId").Value);
        var refreshExpirationValue = jwtToken.Claims.FirstOrDefault(x => x.Type == "RefreshUntil")?.Value;

        if (refreshExpirationValue is null)
        {
            var savedToken = await _tableClient.GetEntityAsync<RefreshTokenTableEntity>(userId.ToString(), refreshToken);

            if (savedToken.Value is null || savedToken.Value.ValidUntil < DateTimeOffset.UtcNow.AddMinutes(5))
                throw new AuthorizationException("You are not authorized to perform this action, please login again");

            refreshExpirationValue = savedToken.Value.ValidUntil.ToString();
        }

        var refreshExpiration = DateTimeOffset.Parse(refreshExpirationValue);

        if (refreshExpiration < DateTimeOffset.UtcNow.AddDays(3))
        {
            refreshToken = await GenerateRefreshTokenForUser(userId);
            refreshExpiration = DateTimeOffset.UtcNow.AddDays(15);
        }

        tokenHandler = new JwtSecurityTokenHandler();
        key = Encoding.ASCII.GetBytes(_signingSecret);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[] { new Claim("UserId", userId.ToString()), new Claim("RefreshUntil", refreshExpiration.ToString()) }),
            Expires = DateTime.UtcNow.AddMinutes(15),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha512Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);

        return (tokenHandler.WriteToken(token), refreshToken);

    }

    public ValueTask<Guid?> ValidateAccessTokenForUser(string accessToken)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_signingSecret);
        try
        {
            tokenHandler.ValidateToken(accessToken, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true,
            }, out SecurityToken validatedToken);

            var jwtToken = (JwtSecurityToken)validatedToken;
            var userId = Guid.Parse(jwtToken.Claims.First(x => x.Type == "UserId").Value);

            // return account id from JWT token if validation successful
            return ValueTask.FromResult<Guid?>(userId);
        }
        catch
        {
            // return null if validation fails
            return ValueTask.FromResult<Guid?>(null);
        }
    }

    public async ValueTask<bool> ValidateRefreshTokenForUser(Guid userId, string refreshToken)
    {
        var savedToken = await _tableClient.GetEntityAsync<RefreshTokenTableEntity>(userId.ToString(), refreshToken);

        if (savedToken.Value is null)
            return false;

        if (savedToken.Value.ValidUntil >= DateTimeOffset.UtcNow)
            return false;

        return true;
    }
}
