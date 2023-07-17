using ProMe.Abstractions;
using System.Security.Claims;

namespace ProMe.Workflow.Services;
internal sealed class IdentityProvider : IIdentityProvider
{
    public List<Claim> Claims { get; } = new List<Claim>();

    public Guid? UserId => _userId;
    private Guid? _userId;

    public Guid? CampaignManagerId => _campaignManagerId;
    private Guid? _campaignManagerId;

    public void AddClaim(Claim claim)
    {
        if (claim.Type == "UserId")
        {
            _userId = Guid.Parse(claim.Value);
        }

        if (claim.Type == "CampaignManagerId")
        {
            _campaignManagerId = Guid.Parse(claim.Value);
        }

        Claims.Add(claim);
    }
}
