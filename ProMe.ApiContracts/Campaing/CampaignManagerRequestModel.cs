using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProMe.ApiContracts.Campaign;
public sealed class CampaignManagerRequestModel
{
    public string? Name { get; set; }
    public string? Email { get; set; }
}
