namespace GeSchool.web.ViewModels;

public class ConfirmDeleteModalViewModel
{
    public string Id { get; set; } = "confirmDeleteModal";
    public string Title { get; set; } = "Confirmer la suppression";
    public string Message { get; set; } = "Cette action est irréversible.";
    public string Controller { get; set; } = string.Empty;
    public string Action { get; set; } = "Delete";
    public string IdFieldName { get; set; } = "Id";
}
