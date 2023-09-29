using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using windows_service;

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);
builder.Services.AddWindowsService(options =>
{
    options.ServiceName = ".NET Windows Service";
});

builder.Services.AddHostedService<WindowsBackgroundService>();

IHost host = builder.Build();
host.Run();

Console.WriteLine("Hello, World!");