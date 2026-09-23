namespace GeSchool.web.ViewModels;

public class MonCoursViewModel
{
    public string Code { get; set; } = string.Empty;
    public string Intitule { get; set; } = string.Empty;
    public int Credits { get; set; }
    public string Semestre { get; set; } = string.Empty;
    public string EnseignantNom { get; set; } = string.Empty;
    public DateTime DateInscription { get; set; }
}
