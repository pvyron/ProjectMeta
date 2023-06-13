using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProMe.Abstractions;
public interface IEmailService
{
    ValueTask<Guid> SendVerificationEmail(string email);
}
