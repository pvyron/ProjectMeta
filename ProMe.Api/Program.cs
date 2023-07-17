using MediatR;
using Microsoft.OpenApi.Models;
using ProMe.Api.Endpoints;
using ProMe.Workflow;
using ProMe.Workflow.Commands;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
var services = builder.Services;
var configuration = builder.Configuration;

builder.AddDataAccess(configuration.GetConnectionString("ProMeDB")!);

services.AddEndpointsApiExplorer()
    .AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo { Title = "Test01", Version = "v1" });
        
        // User authentication
        c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
        {
            Name = "Authorization",
            Type = SecuritySchemeType.ApiKey,
            Scheme = "Bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Description = "JWT Authorization header using the Bearer scheme."
        });
        c.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                Array.Empty<string>()
            }
        });

        // Api key authentication
        c.AddSecurityDefinition("ApiKey", new OpenApiSecurityScheme()
        {
            Name = "x-api-key",
            Type = SecuritySchemeType.ApiKey,
            Scheme = "ApiKey",
            In = ParameterLocation.Header,
            Description = "Api key Authorization header for admin or campaing manager"
        });
        c.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "ApiKey"
                    }
                },
                Array.Empty<string>()
            }
        });
    })
    .AddMediatR(configuration =>
    {
        configuration.RegisterServicesFromAssembly(typeof(Installers).Assembly);
        configuration.AddValidation(services);
    });


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/", () => "I'm not the droid that you are looking for");

app.MapAuth();
app.MapContacts();
app.MapCampaigns();

app.UseHttpsRedirection();

while (true)
{
    try
    {
        app.Run();
        break;
    }
    catch
    {
        continue;
    }
}

