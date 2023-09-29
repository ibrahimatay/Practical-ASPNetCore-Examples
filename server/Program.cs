// See https://aka.ms/new-console-template for more information

using CoreWCF;
using CoreWCF.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using server;

var builder = WebApplication.CreateBuilder();
builder.WebHost.ConfigureKestrel(options => { options.ListenLocalhost(8080); });
builder.Services.AddServiceModelServices();

var app = builder.Build();
app.UseServiceModel(builder =>
{
    builder
        .AddService<EchoService>()
        .AddServiceEndpoint<EchoService, IEchoService>(new BasicHttpBinding(), "/echo");
});

app.Run();

Console.WriteLine("Hello, World!");