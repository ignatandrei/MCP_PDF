//[assembly: LightBddScope]

[assembly: ConfiguredLightBddScope]


namespace MCPArrayTests;

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