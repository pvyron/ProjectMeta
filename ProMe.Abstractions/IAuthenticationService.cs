using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProMe.Abstractions;
public interface IAuthenticationService
{
    ValueTask<(string key, string salt)> HashPassword(string password);
    ValueTask<bool> AuthenticatePasswordForEmail(string input, string key, string salt);
}
