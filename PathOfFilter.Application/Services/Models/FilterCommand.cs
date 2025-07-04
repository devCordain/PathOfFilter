namespace PathOfFilter.Application.Services.Models;

public class FilterCommand
{
	public string Name { get; init; }
	public Dictionary<string, string> Options { get; init; }

	private FilterCommand(string input)
    {
		var inputs = input.Split(" ");

		Name = inputs[0];
        
		Options = new Dictionary<string, string>();
        for (int i = 1; i < inputs.Length; i += 2)
			Options.Add(inputs[i], inputs[i + 1]);
    }

    internal static FilterCommand? TryCreate(string? input)
    {
		try
		{
			if (input == null) return null;

			return new FilterCommand(input);
		}
		catch (Exception e)
		{
			Console.WriteLine(e.Message);
            return null;
        }
    }

    public override string ToString() => $"{Name} ({string.Join(" ", Options.Select(x => $"{x.Key} {x.Value}"))})";
}