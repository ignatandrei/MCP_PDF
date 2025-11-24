using System.Text.Json;

namespace MCP_PDF.Data;
internal record ArrayData(string[] Properties, JsonElement[] JsonArray)
{

}
