using MediatR;
using ProMe.Api.Endpoints;
using ProMe.Workflow;
using ProMe.Workflow.Commands;
using ProMe.Workflow.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
var services = builder.Services;
var configuration = builder.Configuration;

builder.AddDataAccess(configuration.GetConnectionString("ProMeDB")!);

services.AddEndpointsApiExplorer()
    .AddSwaggerGen()
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

app.MapGet("/", (IMediator mediator, string name) =>
{
    return mediator.Send(new AddContact(name));
});

app.MapAuth();

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

