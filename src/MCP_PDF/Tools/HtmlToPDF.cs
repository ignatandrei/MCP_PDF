using System.ComponentModel;
using System.Text;
using System.Text.Unicode;
using ModelContextProtocol.Server;
/// <summary>
/// 
/// </summary>
internal class HtmlToPDF { 
    [McpServerTool]
    [Description("Generates a pdf from a html template")]
    public byte[] GetPDF(string htmlTemplate)
    {
        return Encoding.UTF8.GetBytes( htmlTemplate);
    }
}
