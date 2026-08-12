using Microsoft.AspNetCore.Identity;

namespace GeSchool.Infrastructure.Identity;

public class ApplicationUser : IdentityUser
{
    public string Nom { get; set; } = string.Empty;
    public string Prenom { get; set; } = string.Empty;
    public int? EtudiantId { get; set; }
    public int? EnseignantId { get; set; }
}
