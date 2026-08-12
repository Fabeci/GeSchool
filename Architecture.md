# Architecture

## Diagramme des couches

```
        GeSchool.web
             │
             ▼
     GeSchool.Application
             │
             ▼
    GeSchool.Infrastructure
             │
             ▼
       GeSchool.Domain
```

## Règles de dépendance

- **`GeSchool.Domain`** ne référence aucun autre projet. Il contient uniquement les entités métier (`Entities`), les énumérations (`Enums`) et le type de base commun (`Common/BaseEntity`). Aucune dépendance à Entity Framework, ASP.NET Core ou toute autre technologie.
- **`GeSchool.Application`** référence uniquement `GeSchool.Domain`. Il définit les abstractions (`Interfaces/Repositories`, `Interfaces/Services`), les DTOs, les validateurs (FluentValidation), les mappings (AutoMapper) et les implémentations de services. Il ne référence jamais `GeSchool.Infrastructure` — il ne connaît donc jamais Entity Framework Core directement.
- **`GeSchool.Infrastructure`** référence `GeSchool.Domain` et `GeSchool.Application`. Il implémente les abstractions applicatives : `ApplicationDbContext` (EF Core), les configurations Fluent API, les repositories, et ASP.NET Core Identity (`ApplicationUser`, `RoleSeeder`).
- **`GeSchool.web`** référence `GeSchool.Application` et `GeSchool.Infrastructure` — jamais `GeSchool.Domain` directement. Les controllers ne dépendent que des interfaces de service (`IEtudiantService`, etc.) ; `Program.cs` est le seul point de câblage des extensions `AddInfrastructure` / `AddApplicationServices`, aucun `DbContext` n'est utilisé directement dans un controller.

Cette organisation garantit que le cœur métier (`Domain`, `Application`) reste indépendant de tout détail technique (base de données, framework web), conformément aux principes de la Clean Architecture.

## Suppression en cascade

Seule la relation `Inscription → Note` est configurée en cascade (`DeleteBehavior.Cascade`) : supprimer une inscription supprime ses notes. Toutes les autres relations (`Departement → Enseignant/Etudiant/Cours`, `Enseignant → Cours`, `Etudiant → Inscription`, `Cours → Inscription`) sont en `Restrict`, pour éviter la perte accidentelle de données académiques et l'erreur SQL Server « multiple cascade paths » (un `Cours` est atteignable à la fois via `Departement` et via `Enseignant`).

## Extensions futures

- **Swagger / API** : non ajouté à ce stade car il n'existe pas encore d'endpoints API — à ajouter (`Swashbuckle.AspNetCore`) si une API REST est introduite en complément du MVC.
- **CRUD et logique métier** : les controllers et les implémentations de services sont volontairement minimaux (structure uniquement). Le calcul des moyennes/bulletins (`IBulletinService`) est un stub à implémenter dans une étape ultérieure.
