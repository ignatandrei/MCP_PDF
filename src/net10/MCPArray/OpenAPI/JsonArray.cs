
using Microsoft.AspNetCore.Mvc;
using static MCP_PDF.Tools.ArrayToAny_OpenAPI;

namespace MCPArray.OpenAPI;

public class rec_Convert
{
    public string DataArray { get; set; }
}
public enum ConvertArrayTo
{
    None = 0,
    HTML,
    Markdown,
    Excel,
    PDF,
    Word,
    CSV

}
public class JsonArray
{
    public static void Add_ConvertArray(Microsoft.AspNetCore.Routing.IEndpointRouteBuilder builder)
    {
        builder.MapPost("/api/ArrayToAny/ConvertJSONArrayTo/{what}", ([FromRoute] int what, [FromServices] ArrayToAny toolClass, [FromBody] rec_Convert value) =>
        {

            return what switch
            {
                ConvertArrayTo.HTML => toolClass.ConvertArrayToHTML(value.DataArray),
                ConvertArrayTo.Markdown => toolClass.ConvertArrayToMarkdown(value.DataArray),
                ConvertArrayTo.Excel => toolClass.ConvertArrayToExcel(value.DataArray),
                ConvertArrayTo.PDF => toolClass.ConvertArrayToPDF(value.DataArray),
                ConvertArrayTo.Word => toolClass.ConvertArrayToWord(value.DataArray),
                ConvertArrayTo.CSV => toolClass.ConvertArrayToCSV(value.DataArray),
                _ => throw new ArgumentOutOfRangeException(nameof(what), $"Not expected convert type value: {what}"),
            };
        });


    }
}