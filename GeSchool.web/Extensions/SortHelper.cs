namespace GeSchool.web.Extensions;

public static class SortHelper
{
    public static List<T> Apply<T, TKey>(IEnumerable<T> source, Func<T, TKey> keySelector, string? sortDir)
    {
        return string.Equals(sortDir, "desc", StringComparison.OrdinalIgnoreCase)
            ? source.OrderByDescending(keySelector).ToList()
            : source.OrderBy(keySelector).ToList();
    }
}
