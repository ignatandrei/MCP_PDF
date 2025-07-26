using MCP_PDF.Tests;

//[assembly: LightBddScope]
[assembly: ConfiguredLightBddScope]

namespace MCP_PDF.Tests
{
    internal class ConfiguredLightBddScopeAttribute : LightBddScopeAttribute
    {
        protected override void OnConfigure(LightBddConfiguration configuration)
        {
            configuration
                .ReportWritersConfiguration()
                .AddFileWriter<PlainTextReportFormatter>("Reports/FeaturesReport.txt")
                .AddFileWriter<HtmlReportFormatter>("Reports/FeaturesReport.html");
        }
    }
}