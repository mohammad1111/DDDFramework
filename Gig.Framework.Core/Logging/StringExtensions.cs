namespace Gig.Framework.Core.Logging;

public static class StringExtensions
{
    public static string RemoveSuffix(this string name, string suffix)
    {
        var suffixIndex = name.IndexOf(suffix, StringComparison.InvariantCulture);
        var lastCharacterIndex = suffixIndex > -1 && suffixIndex + suffix.Length == name.Length
            ? suffixIndex
            : name.Length;
        return name.Substring(0, lastCharacterIndex);
    }
}