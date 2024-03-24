namespace PathOfFilter.Application.Services.Helpers;
internal static class StringExtensions
{
    internal static string WithoutFilterComments(this string input) => input.Split('#')[0].Trim();
}
