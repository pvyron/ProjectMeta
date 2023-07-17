using System.Security.Claims;

namespace ProMe.Abstractions;
public interface IIdentityProvider
{
    public List<Claim> Claims { get; }
    public Guid? UserId { get; }
    public Guid? CampaignManagerId { get; }

    public void AddClaim(Claim claim);
}
