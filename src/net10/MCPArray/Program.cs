using Serilog;
using Serilog.Events;

var http = (args?.Length>0)? args[0]=="http":false;
//http = true;
bool stdio = !http;
IHostApplicationBuilder builder;
if (stdio)
{
    builder = Host.CreateApplicationBuilder(args);
    
}
else
{
    builder = WebApplication.CreateBuilder();
}
// Configure all logs to go to stderr (stdout is used for the MCP protocol messages).
//builder.Logging.AddConsole(o => o.LogToStandardErrorThreshold = LogLevel.Trace);
var logFile = Environment.GetEnvironmentVariable("MCP_LOG_FILE");

var config = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
    .MinimumLevel.Override("Microsoft.Playwright", LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .WriteTo.Console(
        outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}",
        standardErrorFromLevel: LogEventLevel.Verbose);
if (!string.IsNullOrWhiteSpace(logFile)) {
    config = config
        .WriteTo.File(logFile,
        rollingInterval: RollingInterval.Day,
        outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}");
    };

Log.Logger = config.CreateLogger();
builder.Services.AddSerilog(Log.Logger);

var server = builder.Services
    .AddMcpServer();

if (stdio)
{
    server = server.WithStdioServerTransport();

}
else
{
    server = server.WithHttpTransport();
    builder.Services.AddOpenApi();
    
}

server.WithTools<ArrayToAny>();
server.Services.AddTransient<Exporter>();
server.Services.AddTransient<ArrayToAny>();

IHost app;
if (stdio)
{
    app = (builder as HostApplicationBuilder)!.Build();
}
else
{
    var web= (builder as WebApplicationBuilder)!.Build();
    web.UseSerilogRequestLogging();
    web.MapOpenApi();
    web.MapOpenApi("/openapi/{documentName}.yaml");
    web.MapMcp();
    web.UseOpenAPISwaggerUI();
    web.AddAll_ArrayToAny();
    var logProgram = web.Services.GetRequiredService<ILogger<Program>>();
    logProgram.LogInformation("MCPArray is running. Navigate to /swagger to see the API documentation.");

    var logExporter = web.Services.GetRequiredService<ILogger<Exporter>>();
    logExporter.LogInformation("Exporter service is ready.");
    //web.MapGet("/"  , () => "MCPArray is running. Navigate to /swagger to see the API documentation.");
    app = web;

}


var t1= Exporter.DownloadBrowser();
var t2 = app.RunAsync();

await Task.WhenAll(t1, t2);
