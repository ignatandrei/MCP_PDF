using System.Text.Json;
using System.Xml.Linq;
using ToonTokenizer;
using ToonTokenizer.Ast;

namespace MCPArrayExport.Data;
internal abstract record Element
{
    public abstract object? GetProperty(string PropertyName);
    public abstract object? GetJsonValue(object? Element);

    internal bool TryGetProperty(string prop, out object? valueOf)
    {
        valueOf = null;  
        var propertyValue = GetProperty(prop);
        if (propertyValue is null) return false;
        var value = GetJsonValue(propertyValue);
        valueOf = value;
        if(value is null) return false;
        return true;
    }
}
internal record ElementToon(AstNode[] nodes, Dictionary<string,int> dictReverseProps) : Element
{
    
    public override object GetProperty(string PropertyName)
    {
        if(!dictReverseProps.TryGetValue(PropertyName, out var index))
        {
            throw new ArgumentException($"Property {PropertyName} not found in element");
        }
        return nodes[index];
        //return nodes.FirstOrDefault(it=>it.GetPropertyNode(ParseResult!.Document!)!.Key == PropertyName);
    }
    override public object? GetJsonValue(object Element)
    {
        if (Element is not AstNode node)
        {
            throw new ArgumentException("Element is not a ValueNode. Did you call GetProperty ?");
        }
        if (node is not ValueNode token)
        {
            throw new ArgumentException("AST NODE is not a ValueNode. Please verify your data");
        }
       
        return token.RawValue;

    }
}
internal record ElementJSON(JsonElement JsonElement) :Element
{
    public override object? GetProperty(string PropertyName)
    {
        return JsonElement.GetProperty(PropertyName);
    }
    override public object? GetJsonValue(object? Element)
    {
        if (Element is not JsonElement element)
        {
            throw new ArgumentException("Element is not a JsonElement. Did you call GetProperty ?");
            
        }
        var valueKind = element.ValueKind;
        return valueKind switch
        {
            JsonValueKind.String => element.GetString(),
            JsonValueKind.Number => element.GetDouble(),
            JsonValueKind.True => true,
            JsonValueKind.False => false,
            JsonValueKind.Null => null,
            JsonValueKind.Array => "not handled array",
            JsonValueKind.Object => "not handled object",
            _ => "not handled " + valueKind.ToString()
        };
    }
}
internal record ArrayData(string[] Properties)
{
    public Element[] JsonArray { get; protected set; } = [];
    public virtual void Init() { }
}

internal record ArrayDataJson(string[] Properties, JsonElement[] data) :ArrayData(Properties)
{
    
    public override void Init()
    {
        base.JsonArray = data.Select(j => new ElementJSON(j)).ToArray();
    }
    

}
internal record ArrayDataToon(string[] Properties, ElementToon[] data) : ArrayData(Properties)
{

    public override void Init()
    {
        base.JsonArray = data;
    }


}