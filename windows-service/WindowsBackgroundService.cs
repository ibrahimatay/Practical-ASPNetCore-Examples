using Microsoft.Extensions.Hosting;

namespace windows_service;

public class WindowsBackgroundService : BackgroundService
{
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        throw new NotImplementedException();
    }
}