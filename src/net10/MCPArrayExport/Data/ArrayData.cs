using System.Text.Json;

namespace MCPArrayExport.Data;
internal record ArrayData(string[] Properties, JsonElement[] JsonArray)
{

}
