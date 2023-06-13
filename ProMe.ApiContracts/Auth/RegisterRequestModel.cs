using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProMe.ApiContracts.Auth;
public sealed class RegisterRequestModel
{
    public string Email { get; set; } = null!;
    public string OriginalPassword { get; set; } = null!;
    public string RepeatPassword { get; set; } = null!;
}
