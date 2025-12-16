
using MCPArrayExport;

[TestFixture]
[FeatureDescription(
@"As a developer 
I want to test the inference diff toon from JSON 
So that I can ensure the system works correctly")]
public partial class JSONvsToon : FeatureFixture
{

    [Test]
    [Scenario]
    [TestCase(new object[2] { """
      [1]{id,name}:
    3,andrei
    """, StringRepresentationType.Toon}    
    )]
    [TestCase(new object[2] { """
      person[1]{id,name}:
    3,andrei
    """, StringRepresentationType.Toon }
    )]
    [TestCase(new object[2] { $$"""
      [{"id":3,"name":"andrei"}]
    """, StringRepresentationType.JSON }
    )]
    [TestCase(new object[2] { $$"""
      [
        {"id":3,"name":"andrei"}
      ]
    """, StringRepresentationType.JSON }
    )]
    [TestCase(new object[2] { $$"""
      [
        
                     {"id":3,"name":"andrei"}
      ]
    """, StringRepresentationType.JSON }
    )]
    public async Task VerifyStringInferrred(string? text, StringRepresentationType result)
    {
        await Runner.RunScenarioAsync(            
            _ => Given_this_text(text),
            _ => Then_must_be_of_type(result)
        );
    }
    Exporter? arrayToAny;
    private ILogger<Exporter> _logger;

    [OneTimeSetUp]
    public void Setup()
    {
        // Create a mock logger for testing
        var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
        _logger = loggerFactory.CreateLogger<Exporter>();
        //_logger  = new NullLogger<ArrayToAny>();
        arrayToAny = new Exporter(_logger);
    }

    private async Task Given_this_text(string text)
    {
        await Task.Yield();
        infer= arrayToAny!.InferFrom(text.ToString()!);
    }

    StringRepresentationType infer=StringRepresentationType.None;
    private async Task Then_must_be_of_type(StringRepresentationType result)
    {
        await Task.Yield();        
        infer.ShouldBe(result);
    }
}
