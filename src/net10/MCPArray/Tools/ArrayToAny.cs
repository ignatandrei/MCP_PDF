
namespace MCP_PDF.Tools;

[MCP2File.AddMCPExportToFile()]
public partial class ArrayToAny 
{
    private readonly Exporter _exporter;
    private readonly ILogger _logger;

    public ArrayToAny(Exporter exporter, ILogger logger)
    {
        this._exporter = exporter;
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    [McpServerTool]
    [Description("Generates a html from a json array serialized as string")]
    public async Task<string> ConvertJsonArrayToHTML([Description("array serialized  as json")] string JsonDataArray)
    {
        return await _exporter.ConvertJsonArrayToHTML(JsonDataArray);
    }




    [McpServerTool]
    [Description("Generates an Excel from a json array serialized as string. The result is in  Base64 .Save to a temporary file and convert later to byte array ")]
    public async Task<byte[]> ConvertArrayToExcel(string JsonDataArray)
    {
        var result = await _exporter.ConvertArrayToExcel(JsonDataArray);
        _logger.LogInformation($"JSON array to excel conversion completed successfully. Excel length: {result.Length} characters");
        return result;


    }
    [McpServerTool]
    [Description("Generates to pdf from a json array serialized as string. The result is in  Base64 .Save to a temporary file and convert later to byte array ")]
    public async Task<byte[]> ConvertArrayToPDF(string JsonDataArray)
    {
        var result = await _exporter.ConvertArrayToPDF(JsonDataArray);
        _logger.LogInformation($"JSON array to pdf conversion completed successfully. pdf length: {result.Length} characters");
        return result   ;
    }

    [McpServerTool]
    [Description("Generates to word from a json array serialized as string . The result is in  Base64. Save to a temporary file and convert later to byte array ")]
    public async Task<byte[]> ConvertArrayToWord(string JsonDataArray)

    {
        var result = await _exporter.ConvertArrayToPDF(JsonDataArray);
        _logger.LogInformation($"JSON array to word conversion completed successfully. word length: {result.Length} characters");
        return result;

    }

    [McpServerTool]
    [Description("Generates a csv from a json array serialized as string")]
    public async Task<string> ConvertArrayToCSV(string JsonDataArray)
    {
        var result = await _exporter.ConvertArrayToCSV(JsonDataArray);
        _logger.LogInformation($"JSON array to csv conversion completed successfully. csv length: {result.Length} characters");
        return result;
    }

}
