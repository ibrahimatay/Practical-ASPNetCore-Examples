
using System.Diagnostics;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DiagnosticAdapter;
using Microsoft.Extensions.Logging;


// https://github.com/dotnet/runtime/blob/main/src/libraries/System.Diagnostics.DiagnosticSource/src/DiagnosticSourceUsersGuide.md

try
{
    
    var builder = WebApplication.CreateBuilder(args);
    
    //builder.Services.AddMiddlewareAnalysis();
    builder.Services.AddSingleton<DiagnosticListener>();
    builder.Services.AddSingleton<DiagnosticObserver>();
    builder.Services.AddSingleton<MyDiagnosticListener>();
    
    var app = builder.Build();
    app.UseRouting();
    
    var diagnosticListener = app.Services.GetService<DiagnosticListener>();
    var listener = app.Services.GetService<MyDiagnosticListener>();
    
    diagnosticListener.SubscribeWithAdapter(listener);

    app.Use(async (context, next) =>
    {
        if (context.Request.Path == "/hello2")
        {
            await context.Response.WriteAsync("Hello world 2");
        }
        else if (context.Request.Path == "/exception")
        {
            throw new Exception("Custom Exception");
        }
        else if (context.Request.Path == "/500")
        {
            var feature = context.Features.Get<IExceptionHandlerFeature>();

            if (feature != null)
            {
                await context.Response.WriteAsync($"<h1>Custom Error Page</h1> {HtmlEncoder.Default.Encode(feature.Error.Message)}");
                await context.Response.WriteAsync($"<hr />{HtmlEncoder.Default.Encode(feature.Error.Source)}");
            }
        }
        else
        {
            await next.Invoke();
        }
    });

    app.Run(async context =>
    {
        context.Response.Headers.Add("content-type", "text/html");

        await context.Response.WriteAsync("<h1>Middleware Analysis</h1>");
        await context.Response.WriteAsync("Check your console for the output");

        await context.Response.WriteAsync(@"
                    <ul>
                        <li><a href=""/hello"">Hello</a></li>
                        <li><a href=""/hello2"">Hello 2</a></li>
                        <li><a href=""/exception"">Exception Page</a></li>
                        <li><a href=""/500"">500</a></li>
                    </ul>
                    ");
    });
}
catch (Exception exception)
{
    Console.WriteLine(exception);
}

class DiagnosticObserver : IObserver<DiagnosticListener>
{
    void IObserver<DiagnosticListener>.OnNext(DiagnosticListener diagnosticListener)
    {
        Console.WriteLine(diagnosticListener.Name);
    }

    void IObserver<DiagnosticListener>.OnError(Exception error)
    {
        Console.WriteLine(error);
    }

    void IObserver<DiagnosticListener>.OnCompleted()
    {
        Console.WriteLine("DiagnosticListener.OnCompleted");
    }
}

public class MyDiagnosticListener
{
    private readonly ILogger _log;

    public MyDiagnosticListener(ILogger log)
    {
        _log = log;
    }
    
    //There are three events that you can listen to
    //- Microsoft.AspNetCore.MiddlewareAnalysis.MiddlewareStarting
    //- Microsoft.AspNetCore.MiddlewareAnalysis.MiddlewareException
    //- Microsoft.AspNetCore.MiddlewareAnalysis.MiddlewareFinished
    // https://github.com/dotnet/aspnetcore/blob/main/src/Middleware/MiddlewareAnalysis/src/AnalysisMiddleware.cs
    
    [DiagnosticName("Microsoft.AspNetCore.MiddlewareAnalysis.MiddlewareStarting")]
    public void OnStarting(HttpContext httpContext, string name, Guid instanceId, long timestamp)
    {
        _log.LogInformation($"MiddlewareStarting: {name}; {httpContext.Request.Path};");
    }

    [DiagnosticName("Microsoft.AspNetCore.MiddlewareAnalysis.MiddlewareException")]
    public void OnException(HttpContext httpContext, Exception exception, string name, Guid instanceId, long timestamp, long duration)
    {
        var durationInMs = (1000.0 * duration / Stopwatch.Frequency);
        _log.LogInformation($"MiddlewareException: {name}; {exception.Message}; {httpContext.Request.Path};{durationInMs} ms");
    }

    [DiagnosticName("Microsoft.AspNetCore.MiddlewareAnalysis.MiddlewareFinished")]
    public void OnFinished(HttpContext httpContext, string name, Guid instanceId, long timestamp, long duration)
    {
        var durationInMs = (1000.0 * duration / Stopwatch.Frequency);
        _log.LogInformation($"MiddlewareFinished: {name}; {httpContext.Response.StatusCode};{durationInMs} ms");
    }
}