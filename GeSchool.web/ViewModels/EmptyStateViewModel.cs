namespace GeSchool.web.ViewModels;

public class EmptyStateViewModel
{
    public string Icon { get; set; } = "search";
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string? ActionText { get; set; }
    public string? ActionController { get; set; }
    public string? ActionAction { get; set; } = "Create";
}
