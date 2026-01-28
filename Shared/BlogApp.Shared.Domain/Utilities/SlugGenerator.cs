namespace BlogApp.Shared.Domain.Utilities;

public static partial class SlugGenerator
{
    public static string Generate(string title)
    {
        if (string.IsNullOrEmpty(title)) return string.Empty;

        // 1. Lowercase
        string str = title.ToLowerInvariant();

        // 2. Remove invalid chars
        str = RemoveInvalidCharecters().Replace(str, "");

        // 3. Convert spaces to hyphens
        str = ConvertSpacesToHyphens().Replace(str, "-").Trim('-');

        return str;
    }

    [GeneratedRegex(@"\s+")]
    private static partial Regex ConvertSpacesToHyphens();

    [GeneratedRegex(@"[^a-z0-9\s-]")]
    private static partial Regex RemoveInvalidCharecters();
}
