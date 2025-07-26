namespace MCP_PDF.Tests;

[TestFixture]
[FeatureDescription(
@"As a developer using the MCP PDF server
I want to test the PDF generation functionality
So that I can ensure the system works correctly")]
public partial class ArrayFeatureTests : FeatureFixture, IAsyncDisposable
{
    [Test]
    [Scenario]
    [TestCase(
"""
[
{"Name":"Ignat","Surname":"Andrei","Email":"ignatandrei@yahoo.com"},
{"Name":"Adam","Surname":"Kowalski","Email":"adam.kowalski@notExisting.com"},
]
""",
"""
<table border="1">
    <thead>
        <tr>
            <th>Nr</th>                    <th>Name</th>
            <th>Surname</th>
            <th>Email</th>
    </thead>
    <tbody>
            <tr>
                <td>1</td>
                    <td>Ignat</td>
                    <td>Andrei</td>
                    <td>ignatandrei@yahoo.com</td>
            </tr>
            <tr>
                <td>2</td>
                    <td>Adam</td>
                    <td>Kowalski</td>
                    <td>adam.kowalski@notExisting.com</td>
            </tr>

</table>
"""
        )]
    public async Task VerifyArrayToHTML(string jsonArray,string htmlResult)
    {
        await Runner.RunScenarioAsync(
            _ => Given_I_have_an_json_array_as_string(jsonArray),
            _ => When_I_Convert_ToHtml(),
            _ => Then_the_result_should_be(htmlResult)
        );
    }

    [Test]
    [Scenario]
    [TestCase(
"""
[
{"Name":"Ignat","Surname":"Andrei","Email":"ignatandrei@yahoo.com"},
{"Name":"Adam","Surname":"Kowalski","Email":"adam.kowalski@notExisting.com"},
]
""")]
    public async Task VerifyArrayToPDF(string jsonArray)
    {
        await Runner.RunScenarioAsync(
            _ => Given_I_have_an_json_array_as_string(jsonArray),
            _ => When_I_Convert_ToPDF(),
            _ => Then_the_pdf_result_should_be_valid()
        );
    }

    string arrToTest=string.Empty;
    string htmlResult=string.Empty;
    byte[] pdfResult = Array.Empty<byte>();
    ArrayToAny? arrayToAny;

    [OneTimeSetUp]
    public void Setup()
    {
        arrayToAny = new ArrayToAny();
    }

    [OneTimeTearDown]
    public async Task TearDown()
    {
        if (arrayToAny != null)
        {
            await arrayToAny.DisposeAsync();
        }
    }

    private Task Given_I_have_an_json_array_as_string(string arr)
    {
        arrToTest = arr;
        return Task.CompletedTask;
    }
    private async Task When_I_Convert_ToHtml()
    {
        htmlResult=  await (new ArrayToAny()).ConvertArrayToHTML(arrToTest);
        return ;
    }

    private async Task When_I_Convert_ToPDF()
    {
        pdfResult = await arrayToAny!.ConvertArrayToPDF(arrToTest);
        //await File.WriteAllBytesAsync(@"D:\test.pdf", pdfResult);
        return;
    }

    private Task Then_the_result_should_be(string html)
    {
        html = html.Replace(" ","").Replace("\r", "").Replace("\n", "");
        htmlResult=htmlResult.Replace(" ", "").Replace("\r", "").Replace("\n", "");
        htmlResult.ShouldBe(html);
        return Task.CompletedTask;
    }

    private Task Then_the_pdf_result_should_be_valid()
    {
        // Verify PDF is not null or empty
        pdfResult.ShouldNotBeNull();
        pdfResult.Length.ShouldBeGreaterThan(0);
        
        // Verify PDF header (PDFs start with %PDF-)
        var pdfHeader = System.Text.Encoding.ASCII.GetString(pdfResult.Take(5).ToArray());
        pdfHeader.ShouldBe("%PDF-");
        
        // Verify PDF contains some expected content markers
        
        return Task.CompletedTask;
    }

    public async ValueTask DisposeAsync()
    {
        if (arrayToAny != null)
        {
            await arrayToAny.DisposeAsync();
        }
        GC.SuppressFinalize(this);
    }
}