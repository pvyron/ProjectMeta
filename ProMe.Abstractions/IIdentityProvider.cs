using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ProMe.Abstractions;
public interface IIdentityProvider
{
    public List<Claim> Claims { get; }
    public Guid? UserId { get; }

    public void AddClaim(Claim claim);
}
