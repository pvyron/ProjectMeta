using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProMe.Workflow.Validation;
internal class ValidationBehavior<TRequest> : IPipelineBehavior<TRequest, IResult> where TRequest : notnull
{
    private readonly IValidator<TRequest> _validator;

    public ValidationBehavior(IValidator<TRequest> validator)
    {
        _validator = validator;
    }

    public async Task<IResult> Handle(TRequest request, RequestHandlerDelegate<IResult> next, CancellationToken cancellationToken)
    {
        var result = await _validator.ValidateAsync(request, cancellationToken);

        if (!result.IsValid)
        {
            var modelStateDictionary = new ModelStateDictionary();

            foreach (var error in result.Errors)
            {
                modelStateDictionary.AddModelError(error.PropertyName, error.ErrorMessage);
            }

            return Results.BadRequest(modelStateDictionary);
        }

        return await next();
    }
}
