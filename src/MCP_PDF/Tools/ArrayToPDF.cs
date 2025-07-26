using MCP_PDF.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace MCP_PDF.Tools;
internal class ArrayToPDF
{
    public async Task<byte[]> ConvertArrayToPDF(string JsonDataArray)
    {
        // Parse the JSON array
        var jsonDocument = JsonDocument.Parse(JsonDataArray);
        var jsonArray = jsonDocument.RootElement;

        List<string> firstItemProperties = [];
        var isArray = jsonArray.ValueKind == JsonValueKind.Array;
        if(!isArray)
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
                    firstItemProperties.Add(property.Name) ;
                }
            }
        }
        ArrayData arrayData = new(firstItemProperties.ToArray(), jsonArray.EnumerateArray().ToArray());
        ArrayTemplate arrayTemplate = new(arrayData);


        // Here you would convert 'content' to PDF bytes
        byte[] pdfBytes = Encoding.UTF8.GetBytes("Andrei"); // Placeholder conversion
        
        return await Task.FromResult(pdfBytes);
    }

    
}
