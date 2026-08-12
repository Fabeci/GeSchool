# GeSchool

Application de gestion de scolarité universitaire (départements, enseignants, étudiants, cours, inscriptions, notes, bulletins) construite en ASP.NET Core MVC.

## Architecture

Clean Architecture en 4 couches, avec une règle de dépendance strictement descendante :

```
GeSchool.web (MVC)
      │
      ▼
GeSchool.Application (cas d'usage, DTOs, interfaces)
      │
      ▼
GeSchool.Infrastructure (EF Core, Identity, repositories)
      │
      ▼
GeSchool.Domain (entités métier)
```

Voir [Architecture.md](Architecture.md) pour le détail des règles de dépendance.

## Technologies

- .NET 8 / ASP.NET Core MVC
- Entity Framework Core 8 (SQL Server)
- ASP.NET Core Identity (rôles : Administrateur, Enseignant, Etudiant)
- AutoMapper (mapping Entité ↔ DTO)
- FluentValidation (validation des DTOs)

## Structure des projets

| Projet | Rôle |
|---|---|
| `GeSchool.Domain` | Entités métier (`Departement`, `Enseignant`, `Etudiant`, `Cours`, `Inscription`, `Note`), enums, aucune dépendance externe |
| `GeSchool.Application` | Interfaces de services et de repositories, DTOs, validators, mappings, implémentations de services |
| `GeSchool.Infrastructure` | `ApplicationDbContext`, configurations Fluent API, repositories EF Core, Identity (`ApplicationUser`, `RoleSeeder`) |
| `GeSchool.web` | Controllers MVC, Views, câblage DI (`Program.cs`) |

## Commandes de lancement

```bash
# Restaurer et compiler la solution
dotnet build GeSchool.sln

# Générer une migration EF Core (depuis la racine du repo)
dotnet ef migrations add NomDeLaMigration --project GeSchool.Infrastructure --startup-project GeSchool.web

# Appliquer les migrations à la base de données
dotnet ef database update --project GeSchool.Infrastructure --startup-project GeSchool.web

# Lancer l'application
dotnet run --project GeSchool.web
```

La chaîne de connexion par défaut (`ConnectionStrings:DefaultConnection` dans `GeSchool.web/appsettings.json`) cible une instance LocalDB — à adapter selon l'environnement.

## Compte administrateur par défaut

Au premier démarrage, si aucun compte n'a le rôle `Administrateur`, un compte est semé automatiquement (`AdminUserSeeder`, `GeSchool.Infrastructure/Identity/AdminUserSeeder.cs`) :

| Email | Mot de passe |
|---|---|
| `admin@geschool.local` | `Admin@12345` |

**À changer immédiatement après la première connexion en dehors d'un environnement de développement local.**

## Gestion des utilisateurs

Un écran réservé au rôle `Administrateur` (`/Users`, lien "Utilisateurs" dans la navbar) permet de lister, créer et supprimer des comptes utilisateurs avec assignation de rôle (`Administrateur`/`Enseignant`/`Etudiant`).

**Limitation actuelle** : ces comptes ne sont pas liés à une fiche `Etudiant`/`Enseignant` du Domain (`EtudiantId`/`EnseignantId` restent vides) — aucun CRUD Departement/Etudiant/Enseignant n'existe encore pour choisir une fiche à lier. Cette liaison sera ajoutée dans une étape ultérieure.

## État actuel

Cette étape pose la fondation de l'architecture (entités, DTOs, interfaces, `DbContext`, configurations EF Core, Identity avec seed des rôles, câblage DI), une authentification minimale (connexion/déconnexion, compte Administrateur semé) et un écran de gestion des utilisateurs. Il n'y a pas encore d'inscription publique, pas de CRUD sur les entités métier (Departement, Enseignant, Etudiant, Cours, Inscription, Note), et aucune logique métier complète (ex. calcul de bulletin) n'est implémentée.
