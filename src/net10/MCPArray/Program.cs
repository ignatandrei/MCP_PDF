
var http = (args?.Length>0)? args[0]=="http":false;
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
 builder.Logging.AddConsole(o => o.LogToStandardErrorThreshold = LogLevel.Trace);

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

server.WithTools<MCP_PDF.Tools.ArrayToAny>();
IHost app;
if (stdio)
{
    app = (builder as HostApplicationBuilder)!.Build();

}
else
{
    var web= (builder as WebApplicationBuilder)!.Build();
    web.MapOpenApi();
    web.MapOpenApi("/openapi/{documentName}.yaml");
    app = web;
}

var WhatBrowser = SupportedBrowser.Chromium;
var browserFetcher = new BrowserFetcher();
browserFetcher.Browser = WhatBrowser;

var t1= browserFetcher.DownloadAsync();
var t2 = app.RunAsync();

await Task.WhenAll(t1, t2);
