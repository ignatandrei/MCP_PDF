using Microsoft.Playwright;

namespace MCP_PDF.Tools;

/// <summary>
/// Shared PDF generator using Playwright for HTML to PDF conversion
/// </summary>
internal class PdfGenerator : IAsyncDisposable
{
    private IBrowser? _browser;
    private IPlaywright? _playwright;
    private bool _disposed = false;

    /// <summary>
    /// Generates a PDF from HTML content
    /// </summary>
    /// <param name="htmlContent">The HTML content to convert to PDF</param>
    /// <returns>PDF as byte array</returns>
    public async Task<byte[]> GeneratePdfFromHtml(string htmlContent)
    {
        await EnsureBrowserInitialized();
        
        var page = await _browser!.NewPageAsync();
        try
        {
            await page.SetContentAsync(htmlContent);
            
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