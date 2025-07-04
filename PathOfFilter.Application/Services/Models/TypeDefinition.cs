namespace PathOfFilter.Application.Services.Models;
internal class TypeDefinition
{
    public string Name { get; set; }
    public List<ItemAttribute> Attributes { get; set; }
}

internal class ItemAttribute
{
    Type Type { get; set; }
    bool Required { get; set; }
}
