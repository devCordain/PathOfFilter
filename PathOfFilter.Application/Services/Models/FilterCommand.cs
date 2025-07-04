namespace PathOfFilter.Application.Services.Models;

public class FilterCommand
{
	public string Name { get; init; }
	public Dictionary<string, string> Options { get; init; }

	private FilterCommand(string input)
    {
		var inputs = input.Split(" ", StringSplitOptions.RemoveEmptyEntries);

		Name = inputs[0];
        
		Options = new Dictionary<string, string>();
        
        // Special handling for load command with file paths
        if (Name.ToLower() == "load" && inputs.Length >= 3)
        {
            var key = inputs[1];
            var value = string.Join(" ", inputs.Skip(2));
            Options.Add(key, value);
        }
        else
        {
            // Regular key-value pair parsing
            for (int i = 1; i < inputs.Length; i += 2)
            {
                if (i + 1 < inputs.Length)
                {
                    Options.Add(inputs[i], inputs[i + 1]);
                }
                else
                {
                    // Handle case where there's a key without a value
                    Options.Add(inputs[i], "");
                }
            }
        }
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