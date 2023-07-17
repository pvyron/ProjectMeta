using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using ProMe.ApiContracts.Campaign;
using ProMe.DataAccess;
using ProMe.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProMe.Workflow.Commands;

public sealed record CreateCampaignManager(CampaignManagerRequestModel Model) : IRequest<IResult>;

public sealed class CreateCampaignManagerValidator : AbstractValidator<CreateCampaignManager>
{
    public CreateCampaignManagerValidator()
    {
        RuleFor(x => x.Model.Name).NotEmpty();
        RuleFor(x => x.Model.Email).NotEmpty().EmailAddress();
    }
}

internal sealed class CreateCampaignManagerHandler : IRequestHandler<CreateCampaignManager, IResult>
{
    private readonly ProMeDBContext _proMeDB;

    public CreateCampaignManagerHandler(ProMeDBContext proMeDB)
    {
        _proMeDB = proMeDB;
    }

    public async Task<IResult> Handle(CreateCampaignManager request, CancellationToken cancellationToken)
    {
        var campaignManager = new CampaignManager
        {
            Name = request.Model.Name!,
            Email = request.Model.Email!,
            AccessKey = Convert.ToBase64String(Guid.NewGuid().ToByteArray())
        };

        campaignManager = (await _proMeDB.CampaignManagers.AddAsync(campaignManager, cancellationToken)).Entity;

        await _proMeDB.SaveChangesAsync(cancellationToken);

        return Results.Created($"/CampaignManagers/{campaignManager.Id}", new CampaignManagerResponseModel
        {
            AccessKey = campaignManager.AccessKey,
            Id = campaignManager.Id,
            Name = campaignManager.Name,
        });
    }
}
