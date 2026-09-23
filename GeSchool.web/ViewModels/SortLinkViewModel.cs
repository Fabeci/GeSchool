namespace GeSchool.web.ViewModels;

public class SortLinkViewModel
{
    public string Text { get; set; } = string.Empty;
    public string SortKey { get; set; } = string.Empty;
    public string? CurrentSort { get; set; }
    public string? CurrentDir { get; set; }
}
