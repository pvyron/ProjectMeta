using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProMe.ApiContracts.Auth;
public sealed class LoginResponseModel
{
    public string? BearerToken { get; set; }
    public string? RefreshToken { get; set; }
}
