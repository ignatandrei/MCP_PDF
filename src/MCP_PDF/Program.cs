using System;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
var nameFile = Environment.GetEnvironmentVariable("logMCPFile");
if (string.IsNullOrEmpty(nameFile))
{
    nameFile = "mcp-pdf-.log";
}
else
{
    nameFile = nameFile.Trim();
}
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
    .MinimumLevel.Override("Microsoft.Playwright", LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .WriteTo.Console(
        outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}",
        standardErrorFromLevel: LogEventLevel.Verbose)
    .WriteTo.File("mcp-pdf-.log", 
        rollingInterval: RollingInterval.Day,
        outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}")
    .CreateLogger();

try
{
    Log.Information("Starting MCP PDF Server");

    var builder = Host.CreateApplicationBuilder(args);

    // Use Serilog for logging
    builder.Services.AddSerilog();

    // Add the MCP services: the transport to use (stdio) and the tools to register.
    builder.Services
        .AddMcpServer()
        .WithStdioServerTransport()
        .WithTools<HtmlToPDF>()
        .WithTools<ArrayToAny>();

    await builder.Build().RunAsync();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}

partial class Program
{
    [ModuleInitializer]
    public static void Initialize()
    {
        Log.Information("Initializing Playwright");
        var exitCode = Microsoft.Playwright.Program.Main(new[] { "install" });
        if (exitCode != 0)
        {
            Log.Fatal("Playwright exited with code {ExitCode}", exitCode);
            throw new Exception($"Playwright exited with code {exitCode}");
        }
        Log.Information("Playwright initialized successfully");
    }
}
