using LanguageExt.ClassInstances.Pred;
using Microsoft.Extensions.Configuration;
using ProMe.Abstractions;
using ProMe.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ProMe.Workflow.Services;
internal sealed class AuthenticationService : IAuthenticationService
{
    private readonly int _saltSize = 16; // 128 bit 
    private readonly int _keySize = 32; // 256 bit
    private readonly int _iterations = 10000;

    public AuthenticationService(IConfiguration configuration)
    {
        _saltSize = configuration.GetValue<int>("Authentication:SaltSize");
        _keySize = configuration.GetValue<int>("Authentication:KeySize");
        _iterations = configuration.GetValue<int>("Authentication:Iterations");
    }

    public ValueTask<bool> AuthenticatePasswordForEmail(string input, string key, string salt)
    {
        var keyBytes = Convert.FromBase64String(key);
        var saltBytes = Convert.FromBase64String(salt);

        using var algorithm = new Rfc2898DeriveBytes(input, saltBytes, _iterations, HashAlgorithmName.SHA512);

        var keyToCheck = algorithm.GetBytes(_keySize);

        return ValueTask.FromResult(keyToCheck.SequenceEqual(keyBytes));
    }

    public ValueTask<(string key, string salt)> HashPassword(string password)
    {
        using var algorithm = new Rfc2898DeriveBytes(password, _saltSize, _iterations, HashAlgorithmName.SHA512);

        var key = Convert.ToBase64String(algorithm.GetBytes(_keySize));
        var salt = Convert.ToBase64String(algorithm.Salt);

        return ValueTask.FromResult((key, salt));
    }
}
