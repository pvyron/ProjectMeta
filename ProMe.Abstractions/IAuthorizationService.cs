using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProMe.Abstractions;
public interface IAuthorizationService
{
    ValueTask<string> GenerateAccessTokenForUser(Guid userId);
    ValueTask<string> GenerateRefreshTokenForUser(Guid userId);

    ValueTask<Guid?> ValidateAccessTokenForUser(string accessToken);
    ValueTask<bool> ValidateRefreshTokenForUser(Guid userId, string refreshToken);

    ValueTask<(string accessToken, string refreshToken)> RefreshSession(string accessTokenOld, string refreshTokenOld);
}
