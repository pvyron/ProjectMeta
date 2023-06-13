using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProMe.ApiContracts.Auth;
public sealed class LoginRequestModel
{
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
    public bool? RememberMe { get; set; } = false;
}
