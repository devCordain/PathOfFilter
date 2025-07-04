namespace PathOfFilter.Application.Items;
public interface IBasicCurrency 
{
    public string Name { get; set; }
    public int? MinimumStackSize { get; set; }
    public int? MaximumStackSize { get; set; }
}
