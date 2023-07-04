using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ProMe.Abstractions;
using ProMe.DataAccess;
using ProMe.Workflow.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using ProMe.Workflow.Commands;
using ProMe.Workflow.Validation;
using MediatR;
using Microsoft.AspNetCore.Http;
using ProMe.Workflow.Models;

namespace ProMe.Workflow;
public static class Installers
{
    public static WebApplicationBuilder AddDataAccess(this WebApplicationBuilder builder, string mssqlConnectionString, Action<SqlServerDbContextOptionsBuilder>? sqlServerOptionsAction = null)
    {
        builder.Services.AddDbContext<ProMeDBContext>(options =>
        {
            options.UseSqlServer(mssqlConnectionString, sqlServerOptionsAction);
        });

        builder.Services.AddValidatorsFromAssembly(typeof(Installers).Assembly);
        builder.Services.AddSingleton<IAuthenticationService, AuthenticationService>();
        builder.Services.AddScoped<IAuthorizationService, AuthorizationService>();
        builder.Services.AddSingleton<IEmailService, EmailService>();
        builder.Services.AddScoped<IIdentityProvider, IdentityProvider>();

        //builder.Host.UseOrleansClient(client =>
        //{
        //    client.UseLocalhostClustering();
        //});

        return builder;
    }

    public static MediatRServiceConfiguration AddValidation(this MediatRServiceConfiguration configuration, IServiceCollection services)
    {
        configuration
            .AddValidation<Register>()
            .AddValidation<Login>()
            .AddValidation<Refresh>();

        services.AddValidatorsFromAssembly(typeof(Installers).Assembly);

        return configuration;
    }

    private static MediatRServiceConfiguration AddValidation<TRequest>(this MediatRServiceConfiguration configuration) where TRequest : notnull
    {
        return configuration.AddBehavior<IPipelineBehavior<TRequest, IResult>, ValidationBehavior<TRequest>>();
    }
}
