namespace validdata.M365.Connectors.OpenApiTools;

internal static class StringExtensions
{
    public static string LowerFirstChar(this string theString)
    {
        return char.ToLowerInvariant(theString[0]) + theString[1..];
    }
}