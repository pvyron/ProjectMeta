using LanguageExt.Common;
using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProMe.Workflow.Queries;
public sealed record SearchContact : IRequest<IResult>;

internal sealed class SearchContactHandler : IRequestHandler<SearchContact, IResult>
{
    public async Task<IResult> Handle(SearchContact request, CancellationToken cancellationToken)
    {
        return Results.Ok();
    }
}
