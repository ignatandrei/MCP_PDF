using System.ComponentModel;
using System.Text;
using System.Text.Unicode;
using ModelContextProtocol.Server;
using Microsoft.Playwright;

/// <summary>
/// HTML to PDF converter using Playwright
/// </summary>
internal class HtmlToPDF : IAsyncDisposable
{
    private IBrowser? _browser;
    private IPlaywright? _playwright;
    private bool _disposed = false;

    [McpServerTool]
    [Description("Generates a pdf from a html template")]
    public async Task<byte[]> GetPDF(string htmlTemplate)
    {
        await EnsureBrowserInitialized();
        
        var page = await _browser!.NewPageAsync();
        try
        {
            await page.SetContentAsync(htmlTemplate);
            
            // Generate PDF with standard options
            var pdfBytes = await page.PdfAsync(new PagePdfOptions
            {
                Format = "A4",
                PrintBackground = true,
                Margin = new Margin
                {
                    Top = "1cm",
                    Right = "1cm",
                    Bottom = "1cm",
                    Left = "1cm"
                }
            });
            
            return pdfBytes;
        }
        finally
        {
            await page.CloseAsync();
        }
    }

    private async Task EnsureBrowserInitialized()
    {
        if (_browser == null)
        {
            _playwright = await Playwright.CreateAsync();
            _browser = await _playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
            {
                Headless = true
            });
        }
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
            if (_browser != null)
            {
                await _browser.DisposeAsync();
            }
            
            _playwright?.Dispose();
            _disposed = true;
        }
    }
}
