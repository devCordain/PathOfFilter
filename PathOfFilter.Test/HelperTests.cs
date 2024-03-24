using PathOfFilter.Application.Services.Helpers;

namespace PathOfFilter.Test;
public class HelperTests
{

    public HelperTests()
    {
        
    }

    [Theory]
    [InlineData("Show # $type->gems-generic", "Show")]
    [InlineData("Show # $type->gems-generic # Some comment", "Show")]
    [InlineData("Hide # %H4 $type->gems-generic", "Hide")]
    [InlineData("Hide # %H4 $type->gems-generic # Some comment", "Hide")]
    [InlineData("Some text without comment", "Some text without comment")]
    [InlineData("Line with # comment", "Line with")]
    [InlineData("# Only a comment", "")]
    [InlineData("Show # $type->gems-generic # $tier->firstzone", "Show")]
    [InlineData("Hide # %H4 $type->gems-generic # $tier->levelingvaal", "Hide")]
    [InlineData("Line with # # # # # ####### comment", "Line with")]
    [InlineData("", "")]
    public void WithoutFilterComments_RemovesComments(string input, string expected)
    {
        var result = input.RemoveFilterComments();

        Assert.Equal(expected, result);
    }
}
