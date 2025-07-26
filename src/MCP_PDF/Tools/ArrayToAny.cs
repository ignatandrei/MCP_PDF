using MCP_PDF.Data;
using ModelContextProtocol.Server;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace MCP_PDF.Tools;

public class ArrayToAny : IAsyncDisposable
{
    private readonly PdfGenerator _pdfGenerator;
    private bool _disposed = false;

    public ArrayToAny()
    {
        _pdfGenerator = new PdfGenerator();
    }
    [McpServerTool]
    [Description("Generates a html from a json array serialized as string")]
    public async Task<string> ConvertJsonArrayToHTML(string JsonDataArray)
    {
        return await ConvertArrayToHTML(JsonDataArray);
    }

    public static async Task<string> ConvertArrayToHTML(string JsonDataArray)
    {
        // Parse the JSON array
        var options = new JsonDocumentOptions()
        {
            AllowTrailingCommas = true,
        };
        var jsonDocument = JsonDocument.Parse(JsonDataArray,options);
        var jsonArray = jsonDocument.RootElement;

        List<string> firstItemProperties = [];
        var isArray = jsonArray.ValueKind == JsonValueKind.Array;
        if (!isArray)
        {
            throw new ArgumentException("Provided JSON data is not an array.");
        }
        // Check if the array has at least one element
        if (!(jsonArray.GetArrayLength() > 0))
        {
            throw new ArgumentException("Provided JSON array is empty.");
        }

        {
            var firstItem = jsonArray[0];

            // Extract all properties from the first item
            if (firstItem.ValueKind == JsonValueKind.Object)
            {
                foreach (var property in firstItem.EnumerateObject())
                {
                    firstItemProperties.Add(property.Name);
                }
            }
        }
        ArrayData arrayData = new(firstItemProperties.ToArray(), jsonArray.EnumerateArray().ToArray());
        ArrayTemplate arrayTemplate = new(arrayData);

        // Generate HTML content from the template
        string htmlContent = await arrayTemplate.RenderAsync();
        return htmlContent.Trim();

    }
    [McpServerTool]
    [Description("Generates a pdf from a json array serialized as string")]
    public async Task<byte[]> ConvertArrayToPDF(string JsonDataArray)
    {
        var htmlContent = await ConvertArrayToHTML(JsonDataArray);
        // Convert HTML to PDF using the shared PDF generator
        byte[] pdfBytes = await _pdfGenerator.GeneratePdfFromHtml(htmlContent);
        
        return pdfBytes;
    }

    public async ValueTask DisposeAsync()
    {
        await DisposeAsyncCore();
        GC.SuppressFinalize(this);
    }

    protected virtual async ValueTask DisposeAsyncCore()
    {
        if (!_disposed)
        {
            if (_pdfGenerator != null)
            {
                await _pdfGenerator.DisposeAsync();
            }
            _disposed = true;
        }
    }
}
