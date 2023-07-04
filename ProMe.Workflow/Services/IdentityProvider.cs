using ProMe.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ProMe.Workflow.Services;
internal sealed class IdentityProvider : IIdentityProvider
{
    public List<Claim> Claims { get; } = new List<Claim>();

    public Guid? UserId => _userId;
    private Guid? _userId;

    public void AddClaim(Claim claim)
    {
        if (claim.Type == "UserId")
        {
            _userId = Guid.Parse(claim.Value);
        }

        Claims.Add(claim);
    }
}
