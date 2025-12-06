namespace MCPArrayExport;

public class Exporter
{
    static SupportedBrowser WhatBrowser= SupportedBrowser.Chromium;
    public static async Task<bool> DownloadBrowser()
    {
        
        var browserFetcher = new BrowserFetcher();
        browserFetcher.Browser = WhatBrowser;
        return await browserFetcher.DownloadAsync() != null;
    }
    private readonly ILogger _logger;

    public Exporter(ILogger logger)
    {
        _logger = logger;
    }
    public async Task<string> ConvertJsonArrayToHTML([Description("array serialized  as json")] string JsonDataArray)
    {
        _logger.LogInformation("Converting JSON array to HTML");
        try
        {
            var result = await ConvertArrayToHTML(JsonDataArray);
            _logger.LogInformation("JSON array to HTML conversion completed successfully");
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to convert JSON array to HTML");
            throw;
        }
    }
    public async Task<string> ConvertArrayToHTML(string JsonDataArray)
    {
        _logger.LogDebug("Starting array to HTML conversion");

        ArrayData arrayData = ConvertFromJsonArray(JsonDataArray);
        ArrayTemplate arrayTemplate = new(arrayData);

        // Generate HTML content from the template
        _logger.LogDebug("Rendering HTML template");
        string htmlContent = await arrayTemplate.RenderAsync();

        _logger.LogDebug("HTML template rendered successfully. Length: {HtmlLength} characters", htmlContent.Length);
        return htmlContent.Trim();
    }

    private ArrayData ConvertFromJsonArray(string JsonDataArray)
    {
        _logger.LogDebug("Parsing JSON array data");

        var options = new JsonDocumentOptions()
        {
            AllowTrailingCommas = true,
        };
        var jsonDocument = JsonDocument.Parse(JsonDataArray, options);
        var jsonArray = jsonDocument.RootElement;

        List<string> firstItemProperties = [];
        var isArray = jsonArray.ValueKind == JsonValueKind.Array;
        if (!isArray)
        {
            _logger.LogError("Provided JSON data is not an array. ValueKind: {ValueKind}", jsonArray.ValueKind);
            throw new ArgumentException("Provided JSON data is not an array.");
        }

        var arrayLength = jsonArray.GetArrayLength();
        _logger.LogDebug("JSON array contains {ArrayLength} items", arrayLength);

        // Check if the array has at least one element
        if (arrayLength == 0)
        {
            _logger.LogError("Provided JSON array is empty");
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
                _logger.LogDebug("Extracted {PropertyCount} properties from first item: {Properties}",
                    firstItemProperties.Count, firstItemProperties);
            }
        }
        ArrayData arrayData = new(firstItemProperties.ToArray(), jsonArray.EnumerateArray().ToArray());
        return arrayData;
    }

    public async Task<byte[]> ConvertArrayToExcel(string JsonDataArray)
    {
        _logger.LogInformation($"Converting JSON array length {JsonDataArray.Length} to CSV");
        try
        {
            var data = ConvertFromJsonArray(JsonDataArray);
            var ms = new MemoryStream();
            using (SpreadsheetDocument spreadsheetDocument = SpreadsheetDocument.Create(ms, DocumentFormat.OpenXml.SpreadsheetDocumentType.Workbook))
            {
                WorkbookPart workbookPart = spreadsheetDocument.AddWorkbookPart();
                workbookPart.Workbook = new Workbook();
                WorksheetPart worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
                SheetData sheetData = new SheetData();
                var rowHeader = new Row();
                foreach (var item in data.Properties)
                {
                    var cell = new Cell
                    {
                        DataType = CellValues.String,
                        CellValue = new CellValue(item)
                    };
                    rowHeader.AppendChild(cell);
                }
                sheetData.AppendChild(rowHeader);
                foreach (var item in data.JsonArray)
                {
                    var row = new Row();
                    foreach (var prop in data.Properties)
                    {
                        var cell = new Cell();
                        if (item.TryGetProperty(prop, out var value))
                        {
                            cell.DataType = CellValues.String;
                            cell.CellValue = new CellValue(value.ToString() ?? string.Empty);
                        }
                        else
                        {
                            cell.DataType = CellValues.String;
                            cell.CellValue = new CellValue(string.Empty);
                        }
                        row.AppendChild(cell);
                    }
                    sheetData.AppendChild(row);
                }
                worksheetPart.Worksheet = new Worksheet(sheetData);

                Sheets sheets = spreadsheetDocument!.WorkbookPart!.Workbook.AppendChild(new Sheets());
                Sheet sheet = new Sheet()
                {
                    Id = spreadsheetDocument.WorkbookPart.GetIdOfPart(worksheetPart),
                    SheetId = 1,
                    Name = "Sheet1"
                };
                sheets.Append(sheet);

                workbookPart.Workbook.Save();

            }
            ms.Position = 0;
            var result = ms.ToArray();
            _logger.LogInformation($"JSON array to excel conversion completed successfully. Excel length: {result.Length} characters");

            return result;

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to convert JSON array to CSV");
            throw;
        }
    }

    public async Task<byte[]> ConvertArrayToPDF(string JsonDataArray)
    {
        _logger.LogInformation($"Converting JSON array length {JsonDataArray.Length} to PDF");
        try
        {
            var htmlContent = await ConvertArrayToHTML(JsonDataArray);
            using var ms = new MemoryStream();
            var browserFetcher = new BrowserFetcher();
            browserFetcher.Browser = WhatBrowser;
            var b= await browserFetcher.DownloadAsync();            
            await using var browser = await Puppeteer.LaunchAsync(new LaunchOptions
            {
                Browser = WhatBrowser,
                Headless = true,
                ExecutablePath = b.GetExecutablePath(),
                Args = new[] { "--no-sandbox", "--disable-setuid-sandbox" } // Disable sandboxing
            });
            await using var page = await browser.NewPageAsync();
            await page.SetContentAsync(htmlContent);
            using var pdfStream = await page.PdfStreamAsync();
            await pdfStream.CopyToAsync(ms);

            ms.Position = 0;
            var result = ms.ToArray();
            _logger.LogInformation($"JSON array to pdf conversion completed successfully. pdf length: {result.Length} characters");
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to convert JSON array to pdf");
            throw;
        }
    }

    
    public async Task<byte[]> ConvertArrayToWord(string JsonDataArray)
    {
        _logger.LogInformation($"Converting JSON array length {JsonDataArray.Length} to CSV");
        try
        {
            var data = ConvertFromJsonArray(JsonDataArray);
            // Convert HTML to PDF using the shared PDF generator
            //Excel2007File template = new(data);
            //var result = await template.RenderAsync();
            var ms = new MemoryStream();
            using (WordprocessingDocument document = WordprocessingDocument.Create(ms, DocumentFormat.OpenXml.WordprocessingDocumentType.Document))
            {
                MainDocumentPart mainPart = document.AddMainDocumentPart();
                mainPart.Document = new Document();
                Body body = new Body();

                WordProcess.Table table = new();

                WordProcess.TableRow rowHeader = new();
                foreach (var item in data.Properties)
                {

                    var cell = new WordProcess.TableCell(new WordProcess.Paragraph(new WordProcess.Run(new WordProcess.Text(item))));
                    rowHeader.AppendChild(cell);
                }
                table.Append(rowHeader);
                foreach (var item in data.JsonArray)
                {
                    WordProcess.TableRow row = new();
                    foreach (var prop in data.Properties)
                    {
                        WordProcess.TableCell cell;
                        if (item.TryGetProperty(prop, out var value))
                        {
                            cell = new WordProcess.TableCell(new WordProcess.Paragraph(new WordProcess.Run(new WordProcess.Text(value.ToString()))));
                        }
                        else
                        {
                            cell = new WordProcess.TableCell(new WordProcess.Paragraph(new WordProcess.Run(new WordProcess.Text(string.Empty))));
                        }
                        row.AppendChild(cell);
                    }
                    table.Append(row);
                }
                body.Append(table);
                mainPart.Document.Append(body);
                mainPart.Document.Save();

            }
            ms.Position = 0;
            var result = ms.ToArray();
            _logger.LogInformation($"JSON array to word conversion completed successfully. word length: {result.Length} characters");

            return result;

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to convert JSON array to CSV");
            throw;
        }
    }


    public async Task<string> ConvertArrayToCSV(string JsonDataArray)
    {
        _logger.LogInformation($"Converting JSON array length {JsonDataArray.Length} to CSV");
        try
        {
            var data = ConvertFromJsonArray(JsonDataArray);
            // Convert HTML to PDF using the shared PDF generator
            ArrayCSVTemplate arrayCSVTemplate = new(data);
            var result = await arrayCSVTemplate.RenderAsync();

            _logger.LogInformation("JSON array to CSV conversion completed successfully. CSV length: {CsvLength} characters", result.Length);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to convert JSON array to CSV");
            throw;
        }
    }
}

