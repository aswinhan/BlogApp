namespace BlogApp.Shared.Domain.Utilities;

public static class SlugGenerator
{
    public static string Generate(string title)
    {
        if (string.IsNullOrEmpty(title)) return string.Empty;

        // 1. Lowercase
        string str = title.ToLowerInvariant();

        // 2. Remove invalid chars
        str = Regex.Replace(str, @"[^a-z0-9\s-]", "");

        // 3. Convert spaces to hyphens
        str = Regex.Replace(str, @"\s+", "-").Trim('-');

        return str;
    }
}
