# GeSchool

Application de gestion de scolarité universitaire (départements, enseignants, étudiants, cours, inscriptions, notes, bulletins) construite en ASP.NET Core MVC (.NET 8) et Entity Framework Core, conformément au sujet de projet SUP'INFO — *Conception et réalisation d'une application de Gestion de la Scolarité*.

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

Voir [Rapport-de-projet.md](Rapport-de-projet.md) pour la présentation complète du projet (cahier des charges reformulé, modèle de données, règles de dépendance entre les couches, choix techniques, difficultés rencontrées).

## Technologies

- .NET 8 / ASP.NET Core MVC
- Entity Framework Core 8, Code First avec migrations (SQL Server / LocalDB)
- ASP.NET Core Identity (rôles : `Administrateur`, `Enseignant`, `Etudiant`)
- AutoMapper (mapping Entité ↔ DTO)
- FluentValidation (validation des DTOs Application)
- Bootstrap 5 (grid, modales, offcanvas) + design system maison (`wwwroot/css/variables.css`, `components.css`, `responsive.css`) et icônes Lucide vendorisées en SVG inline (`GeSchool.web/Extensions/IconHelper.cs`)

## Structure des projets

| Projet | Rôle |
|---|---|
| `GeSchool.Domain` | Entités métier (`Departement`, `Enseignant`, `Etudiant`, `Cours`, `Inscription`, `Note`), enums, aucune dépendance externe |
| `GeSchool.Application` | Interfaces de services et de repositories, DTOs, validators, mappings, implémentations de services (dont le calcul du bulletin) |
| `GeSchool.Infrastructure` | `ApplicationDbContext`, configurations Fluent API, repositories EF Core, Identity (`ApplicationUser`, `RoleSeeder`, `AdminUserSeeder`) |
| `GeSchool.web` | Controllers MVC, Views Razor, ViewModels, design system, câblage DI (`Program.cs`) |

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
| `admin@supinfo.sn` | `Admin@12345` |

**À changer immédiatement après la première connexion en dehors d'un environnement de développement local.**

## Gestion des utilisateurs

Un écran réservé au rôle `Administrateur` (`/Users`) permet de créer des comptes utilisateurs avec assignation de rôle, et de les lier optionnellement à une fiche `Etudiant` ou `Enseignant` existante (`ApplicationUser.EtudiantId`/`EnseignantId`). Cette liaison est ce qui permet à un compte Étudiant/Enseignant d'accéder à son propre espace ; sans elle, ces espaces affichent un message clair plutôt que de planter.

Les Étudiants et Enseignants peuvent aussi créer leur propre compte via `/Account/Register` (voir « Auto-inscription » ci-dessous) — l'Administrateur reste le seul moyen de créer un compte **Administrateur**.

**Limitations actuelles** : pas d'édition de compte existant ni de liaison rétroactive après création par l'Administrateur (seulement au moment du `Create`).

## Permissions par rôle

| Fonctionnalité | Administrateur | Enseignant | Étudiant |
|---|:---:|:---:|:---:|
| Départements, Utilisateurs (CRUD) | ✅ | — | — |
| Enseignants, Étudiants (CRUD) | ✅ | Lecture seule, limité à ses cours/étudiants | — |
| Cours (CRUD) | ✅ | Lecture seule, limité à ses cours | — |
| Inscriptions, Notes (CRUD) | ✅ | ✅, limité à ses cours | — |
| Bulletins | ✅ (tous) | — | ✅ (le sien uniquement) |
| Mon profil / Mes cours | — | — | ✅ |

Le périmètre "ses cours" d'un Enseignant est déterminé par la liaison `ApplicationUser.EnseignantId`. Toute tentative d'accès (même par manipulation directe d'URL ou de formulaire) à une ressource hors de ce périmètre renvoie un refus d'accès — voir `GeSchool.web/Controllers/CoursController.cs`, `EtudiantsController.cs`, `InscriptionsController.cs`, `NotesController.cs`.

## Auto-inscription

`/Account/Register` (`AccountController.Register`) permet à un Étudiant ou un Enseignant de créer lui-même son compte, en réponse à l'exigence du sujet *« Mettre en place l'inscription et la connexion des utilisateurs »*. Le formulaire ne demande que : rôle (Étudiant/Enseignant — **Administrateur n'est jamais proposé, et une tentative de le forcer par requête manipulée est rejetée côté serveur**), email, mot de passe.

Contrainte de sécurité : l'email saisi doit correspondre exactement à celui d'une fiche `Etudiant`/`Enseignant` **déjà créée par un Administrateur**, et cette fiche ne doit pas déjà être liée à un autre compte. Le nom/prénom du compte sont recopiés depuis cette fiche (jamais saisis librement), pour qu'un utilisateur ne puisse pas s'inventer une identité — il ne fait que réclamer un compte pour une fiche déjà vérifiée. Sans correspondance, l'inscription est refusée avec un message explicite invitant à contacter l'administration.

## Choix assumés (divergences documentées vs. le sujet)

- **FluentValidation plutôt que Data Annotations** : le sujet impose *« Validation des données côté serveur obligatoire (Data Annotations) »*. Les DTOs de `GeSchool.Application` sont validés par FluentValidation (invoqué explicitement dans les controllers), pour garder la validation métier dans la couche Application plutôt que sur des attributs attachés aux objets. Les ViewModels purement UI (`LoginViewModel`, `RegisterViewModel`, `CreateUserViewModel`) utilisent, eux, des Data Annotations classiques. Conséquence : la validation côté client ne couvre que les champs texte obligatoires (effet des types non-nullables C#), pas les règles métier (`Credits > 0`, `Valeur` 0–20, etc.), qui sont vérifiées côté serveur après soumission.

## État actuel

Le projet est fonctionnellement complet au regard du cahier des charges : CRUD Départements/Enseignants/Étudiants/Cours/Inscriptions/Notes, calcul et consultation du bulletin (moyenne par cours + moyenne générale pondérée par crédits), authentification et trois rôles avec permissions différenciées (y compris l'Enseignant, limité à ses cours), interface soignée avec design system réutilisable, thème clair/sombre.
