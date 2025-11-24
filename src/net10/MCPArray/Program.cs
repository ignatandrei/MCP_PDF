
var stdio = (args?.Length>0)? args[0]=="stdio":false;
bool http = !stdio;
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

// Add the MCP services: the transport to use (stdio) and the tools to register.
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
//    ArrayToAny arrayToAny = new(web.Services.GetRequiredService<ILogger<ArrayToAny>>());
//    var arr = 
//        """
//[
//{"Name":"Ignat","Surname":"Andrei","Email":"ignatandrei@yahoo.com"},
//{"Name":"Adam","Surname":"Kowalski","Email":"adam.kowalski@notExisting.com"},
//]
//""";
    //File.WriteAllBytes("D:\\a.xlsx", await arrayToAny.ConvertArrayToExcel(arr));
    //File.WriteAllBytes("D:\\a.docx", await arrayToAny.ConvertArrayToWord(arr));
    //File.WriteAllBytes("D:\\a.pdf", await arrayToAny.ConvertArrayToPDF(arr));


    app = web;
}

var WhatBrowser = SupportedBrowser.Chromium;
var browserFetcher = new BrowserFetcher();
browserFetcher.Browser = WhatBrowser;

var t1= browserFetcher.DownloadAsync();
var t2 = app.RunAsync();

await Task.WhenAll(t1, t2);
