using System.Text.Json;
using System.Text.Json.Nodes;

namespace MCP_PDF.Data;
internal record ArrayData(string[] Properties, JsonElement[] JsonArray)
{

}
