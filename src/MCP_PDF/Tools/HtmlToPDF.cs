using MCP_PDF.Tools;
using Microsoft.Playwright;
using ModelContextProtocol.Server;
using System.ComponentModel;
using System.Text;
using System.Text.Unicode;

/// <summary>
/// HTML to PDF converter using Playwright
/// </summary>
internal class HtmlToPDF : IAsyncDisposable
{
    private readonly PdfGenerator _pdfGenerator;
    private bool _disposed = false;
    public HtmlToPDF()
    {
        _pdfGenerator = new PdfGenerator();
    }
    [McpServerTool]
    [Description("Generates a pdf from a html template")]
    public async Task<byte[]> GetPDF(string htmlTemplate)
    {
        return await _pdfGenerator.GeneratePdfFromHtml(htmlTemplate);
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
