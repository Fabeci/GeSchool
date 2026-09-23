# Préparation soutenance — GeSchool

*Note perso : le sujet officiel s'appelle « Gestion de la Scolarité » mais mon projet s'appelle GeSchool — c'est le nom que j'ai choisi, le sujet n'imposait pas un nom de code précis. Si on me pose la question, je réponds ça simplement, sans stresser.*

---

## 1. Comprendre le projet

C'est quoi mon TP en une phrase : une appli web qui permet à une école de gérer ses départements, ses enseignants, ses étudiants, ses cours, les inscriptions, et les notes, avec un bulletin calculé automatiquement.

Il y a 3 types de comptes :

- **Administrateur** : il gère tout, y compris les comptes des autres utilisateurs.
- **Enseignant** : il ne voit que ses propres cours et les étudiants inscrits dedans. Il peut inscrire des étudiants et saisir des notes, mais seulement pour ses cours.
- **Étudiant** : il voit juste son profil, ses cours, et son bulletin.

Le sujet demande que l'Enseignant « consulte les étudiants et les cours » sans préciser « seulement les siens » — c'est moi qui ai ajouté cette restriction, parce que ça n'aurait pas de sens qu'un enseignant puisse modifier les notes d'un cours qui n'est pas le sien.

**À dire à l'oral :**
> « GeSchool, c'est une appli de gestion de scolarité avec trois profils : l'administrateur qui gère tout, l'enseignant qui gère uniquement ses cours, et l'étudiant qui consulte son propre espace. J'ai volontairement restreint l'enseignant à son périmètre parce que le sujet ne le précisait pas mais ça me semblait logique niveau sécurité. »

---

## 2. Comprendre l'architecture

Le projet est coupé en 4 projets .NET :

- **GeSchool.Domain** : juste les classes métier (Etudiant, Cours, etc.) et les enums. Rien d'autre. Ce projet ne dépend d'aucun autre projet.
- **GeSchool.Application** : les services, les DTOs, les interfaces des repositories, la validation. Il dépend seulement de Domain.
- **GeSchool.Infrastructure** : tout ce qui touche à la base de données (EF Core), et l'authentification (Identity). Il dépend de Application et Domain.
- **GeSchool.web** : les Controllers, les vues Razor. Il dépend de Application et Infrastructure.

Pourquoi j'ai fait ça comme ça : si demain je veux changer SQL Server pour autre chose, je ne touche quasiment qu'à Infrastructure. Le reste du code n'a même pas besoin de savoir qu'Entity Framework existe.

Un truc important à retenir : **le Controller ne parle jamais directement à la base de données**. Il appelle un Service (`IEtudiantService` par exemple), qui appelle un Repository (`IEtudiantRepository`), qui lui parle à Entity Framework.

Concrètement, dans `GeSchool.web/Controllers/EtudiantsController.cs` :
```csharp
private readonly IEtudiantService _etudiantService;
```
Le Controller ne connaît que l'interface. Il ne sait même pas comment c'est implémenté derrière.

**À dire à l'oral :**
> « J'ai découpé le projet en 4 couches : Domain pour les entités métier, Application pour les services et DTOs, Infrastructure pour Entity Framework et Identity, et Web pour les Controllers et les vues. Le Controller ne parle jamais directement à la base, il passe toujours par un Service. »

### Repository + Service, en vrai

Un **Repository** c'est juste une classe qui sait faire des opérations basiques sur la base (ajouter, récupérer, supprimer) pour une entité donnée. Tous les repositories utilisent la même classe générique `Repository<T>` (`GeSchool.Infrastructure/Repositories/Repository.cs`), avec des méthodes comme `GetByIdAsync`, `GetAllAsync`, etc.

Un **Service** utilise un ou plusieurs repositories pour faire le vrai travail, et renvoie des DTOs (pas les entités directement). Exemple : `EtudiantService.cs` utilise `IEtudiantRepository` pour aller chercher les étudiants, et AutoMapper pour transformer l'entité `Etudiant` en `EtudiantDto`.

**Si le prof demande « pourquoi un Repository alors qu'EF Core en fait déjà un » :** je réponds que sans repository, ma couche Application devrait connaître Entity Framework directement, ce qui casserait la séparation entre les couches. Le repository sert à cacher EF Core, pas à le remplacer.

---

## 3. Comprendre les entités et la base de données

6 entités, toutes dans `GeSchool.Domain/Entities/` :

| Entité | Champs principaux | Lien |
|---|---|---|
| Departement | Nom, Description | 1 département a plusieurs enseignants/étudiants/cours |
| Enseignant | Nom, Prenom, Email, Specialite, DepartementId | rattaché à 1 département, responsable de plusieurs cours |
| Etudiant | NumeroEtudiant, Nom, Prenom, Email, DateNaissance, Sexe, DepartementId | rattaché à 1 département, plusieurs inscriptions |
| Cours | Code, Intitule, Credits, Semestre, DepartementId, EnseignantId | rattaché à 1 département ET 1 enseignant |
| Inscription | EtudiantId, CoursId, DateInscription | fait le lien entre un étudiant et un cours |
| Note | InscriptionId, Valeur, TypeEvaluation, DateEvaluation | rattachée à une inscription |

**Pourquoi Inscription existe en tant que table à part**, et pas juste un `CoursId` directement dans Etudiant : parce qu'un étudiant peut suivre plusieurs cours, et un cours a plusieurs étudiants. C'est une relation plusieurs-à-plusieurs, donc il faut une table intermédiaire. Et en plus, c'est sur cette table Inscription que je mets la date d'inscription, et c'est elle qui porte les notes.

**Les contraintes importantes**, et où elles sont :

- Le `NumeroEtudiant` doit être unique → index unique dans `EtudiantConfiguration.cs`.
- Un étudiant ne peut pas s'inscrire deux fois au même cours → index unique composé sur `(EtudiantId, CoursId)` dans `InscriptionConfiguration.cs`.
- Les crédits d'un cours doivent être > 0, et une note doit être entre 0 et 20 → contraintes CHECK au niveau SQL, en plus de la validation côté C#.

**Pourquoi une contrainte en base ET une validation en C# pour la même règle :** parce que la validation C# peut être contournée (bug, appel direct à la base...), alors que la contrainte SQL, elle, protège la donnée quoi qu'il arrive.

**À dire à l'oral :**
> « J'ai 6 entités : Département, Enseignant, Étudiant, Cours, Inscription et Note. Inscription sert à représenter la relation plusieurs-à-plusieurs entre étudiant et cours, et c'est elle qui porte les notes. Les contraintes importantes comme l'unicité du numéro étudiant sont mises à la fois en validation C# et en contrainte SQL, pour être sûr que la donnée reste cohérente même si quelque chose contourne la validation applicative. »

---

## 4. Services et DTOs

**C'est quoi un DTO ?** Un objet simple qui ne sert qu'à transporter des données entre les couches, sans logique dedans. Exemple : `EtudiantDto` a les mêmes champs que l'entité `Etudiant`, mais ce n'est pas la même classe.

**Pourquoi je n'envoie pas directement l'entité Etudiant à la vue ?** Parce que l'entité contient des trucs qui ne concernent que la base de données (les relations EF Core, par exemple `Etudiant.Inscriptions`), et je ne veux pas que la couche Web dépende de la structure interne de mon modèle de données. Si je change un truc dans Domain, je ne veux pas que ça casse mes vues.

**C'est quoi un Service ?** C'est la classe qui contient la vraie logique. Le Controller ne fait quasiment rien, il appelle juste le Service. Exemple, dans `GeSchool.Application/Services/EtudiantService.cs` :
```csharp
public async Task<EtudiantDto> CreateAsync(CreateEtudiantDto dto)
{
    var etudiant = _mapper.Map<Etudiant>(dto);
    await _etudiantRepository.AddAsync(etudiant);
    await _etudiantRepository.SaveChangesAsync();
    return _mapper.Map<EtudiantDto>(etudiant);
}
```
Il transforme le DTO en entité, la sauvegarde via le repository, puis retourne un DTO.

J'ai 3 DTOs par entité : `Create{Entite}Dto`, `Update{Entite}Dto`, `{Entite}Dto` (pour la lecture). Ça évite d'exposer un champ `Id` sur un formulaire de création par exemple.

**À dire à l'oral :**
> « Le Service contient la logique, le Controller lui délègue le travail. Le Service utilise un DTO pour parler avec le Controller, jamais l'entité directement, pour ne pas exposer les détails internes de mon modèle de données à la couche Web. »

---

## 5. Entity Framework Core

**DbContext** : c'est la classe qui représente ma connexion à la base. La mienne s'appelle `ApplicationDbContext` (`GeSchool.Infrastructure/Data/ApplicationDbContext.cs`). Elle contient un `DbSet<T>` par entité (`DbSet<Etudiant> Etudiants`, etc.) — chaque DbSet correspond à une table.

**Fluent API** : c'est la façon dont je configure les règles de mes entités (unicité, longueur max, relations) sans mettre d'attributs directement sur les classes du Domain. Chaque entité a son fichier de configuration dans `GeSchool.Infrastructure/Data/Configurations/` (par exemple `EtudiantConfiguration.cs`), et EF Core les applique automatiquement via `OnModelCreating`.

**Les migrations** : à chaque fois que je change une entité, je fais `dotnet ef migrations add NomDeLaMigration`, ça génère un fichier qui décrit le changement (ajouter une colonne, etc.), et ensuite `dotnet ef database update` applique vraiment le changement sur la base. Tout est dans `GeSchool.Infrastructure/Migrations/`. Ça évite de devoir modifier la base à la main.

**Code First** : je pars de mes classes C# (le Domain), et c'est EF Core qui génère la base à partir de ça — pas l'inverse.

**À dire à l'oral :**
> « J'utilise Entity Framework Core en Code First : je définis mes entités en C#, et les migrations génèrent la base automatiquement. Les règles comme les contraintes d'unicité sont configurées avec la Fluent API dans des fichiers de configuration séparés, pas avec des attributs sur les entités, pour garder le Domain propre. »

---

## 6. ASP.NET Core MVC

Le principe : une URL correspond à une **action** dans un **Controller**, qui retourne une **Vue**.

Exemple concret avec `EtudiantsController.cs` :

- `GET /Etudiants` → l'action `Index()` → récupère la liste via le Service → renvoie la vue `Views/Etudiants/Index.cshtml`.
- `POST /Etudiants/Create` → l'action `Create(CreateEtudiantDto dto)` reçoit les données du formulaire automatiquement (c'est le **model binding**, ASP.NET Core remplit le DTO tout seul à partir des champs du formulaire), les valide, puis appelle le Service.

Les vues Razor (`.cshtml`) mélangent du HTML et du C# avec `@`. `asp-for`, `asp-action`, etc. sont des tag helpers qui génèrent le HTML (par exemple `asp-for="Nom"` génère un `<input>` relié au bon champ).

**À dire à l'oral :**
> « Chaque Controller correspond à une entité. Une action gère une requête HTTP, appelle le Service correspondant, et renvoie une vue Razor. Le model binding remplit automatiquement mes DTOs à partir des données du formulaire. »

---

## 7. Identity et rôles

**Authentification vs Autorisation**, en une phrase chacune :

- Authentification = « qui es-tu ? » (se connecter)
- Autorisation = « qu'est-ce que tu as le droit de faire ? » (les rôles)

J'utilise ASP.NET Core Identity pour gérer les comptes et les mots de passe. Mais j'ai créé `ApplicationUser` (`GeSchool.Infrastructure/Identity/ApplicationUser.cs`) qui hérite de `IdentityUser`, avec des champs en plus :
```csharp
public class ApplicationUser : IdentityUser
{
    public string Nom { get; set; }
    public string Prenom { get; set; }
    public int? EtudiantId { get; set; }
    public int? EnseignantId { get; set; }
}
```
`EtudiantId` et `EnseignantId` sont **optionnels** (le `?`) parce qu'un compte Administrateur n'est lié à aucune fiche métier — ces champs ne servent qu'à relier un compte Étudiant ou Enseignant à sa fiche dans le domaine.

Les 3 rôles sont créés au démarrage par `RoleSeeder.cs`, et un compte admin par défaut est créé par `AdminUserSeeder.cs` (ces deux seeders sont appelés dans `Program.cs`, juste avant `app.Run()`).

Dans les Controllers, je restreins l'accès avec `[Authorize(Roles = "Administrateur")]`. Pour l'Enseignant, en plus du rôle, je filtre les données manuellement dans le code pour qu'il ne voie que ses propres cours.

**À dire à l'oral :**
> « J'ai créé ApplicationUser qui hérite de IdentityUser, avec un lien optionnel vers une fiche Étudiant ou Enseignant. Ça me permet de savoir, quand un utilisateur se connecte, à quelle fiche du domaine son compte correspond, sans dupliquer les infos. »

---

## 8. Design et interface

Rien de compliqué : une sidebar à gauche pour naviguer entre les sections, une barre du haut avec le nom de la page et le compte connecté, et des pages avec des cartes/tableaux pour afficher les listes.

J'ai gardé Bootstrap (déjà présent dans le projet) pour la base (grille, modales), et j'ai ajouté mes propres variables CSS (couleurs, espacements) dans `wwwroot/css/variables.css` pour que tout le design soit cohérent sans avoir à changer chaque page si je veux modifier une couleur.

**À dire à l'oral :**
> « J'ai gardé Bootstrap pour la structure de base, et j'ai ajouté un design system avec des variables CSS pour la cohérence visuelle, sans ajouter de nouvelle dépendance JavaScript. »

---

## 9. Exemple d'un parcours complet dans l'application

Le parcours logique pour un étudiant, du début jusqu'au bulletin :

1. Un Administrateur crée le Département, l'Enseignant, le Cours.
2. Il crée la fiche Étudiant.
3. Il inscrit l'étudiant à un cours (`Inscription`).
4. L'Enseignant (ou l'admin) saisit des notes pour cette inscription.
5. Le bulletin de l'étudiant se calcule automatiquement, à chaque fois qu'on le consulte — la moyenne d'un cours, c'est 70 % sur la note d'examen et 30 % sur la moyenne des autres notes (contrôle continu, TP, projet).

Le bulletin n'est **jamais stocké** en base : `BulletinService.GenerateAsync()` recalcule tout à chaque fois qu'on ouvre la page, à partir des notes existantes. Donc si on rajoute une note après, le bulletin est immédiatement à jour.

**À dire à l'oral :**
> « Le bulletin n'est pas une table en base, il est recalculé en temps réel à chaque consultation à partir des notes existantes. La moyenne d'un cours pondère l'examen à 70 % et le reste à 30 %. »

---

## 10. Questions probables du jury

**Pourquoi ASP.NET Core MVC ?**
Courte : c'était imposé par le sujet, et c'est adapté pour une appli de gestion avec beaucoup de formulaires et de pages classiques.
Si insiste : MVC sépare bien le traitement (Controller), l'affichage (View) et les données, ce qui correspond bien à mon architecture en couches.

**Pourquoi une architecture en couches ?**
Courte : pour ne pas mélanger la logique métier, l'accès à la base, et l'affichage dans le même fichier.
Si insiste : ça rend le code plus facile à faire évoluer (changer la base sans toucher au reste) et plus facile à tester.

**Pourquoi Entity Framework Core ?**
Courte : c'était imposé, et ça évite d'écrire du SQL à la main.
Si insiste : le mode Code First me permet de définir mes entités en C# et de laisser EF Core générer et faire évoluer la base via les migrations.

**Pourquoi un DTO plutôt que d'envoyer l'entité directement ?**
Courte : pour ne pas exposer la structure interne de ma base à la couche Web.
Si insiste : ça évite aussi d'avoir des champs inutiles ou dangereux sur un formulaire (comme l'Id sur un formulaire de création).

**Pourquoi un Repository, alors qu'EF Core fait déjà un peu ce travail ?**
Courte : pour que ma couche Application ne dépende pas directement d'Entity Framework.
Si insiste : si un jour je change de techno d'accès aux données, je n'ai que le Repository à changer, pas les Services.

**Pourquoi un Service ?**
Courte : pour que le Controller reste simple et ne gère pas lui-même la logique métier.
Si insiste : ça permet aussi de tester la logique indépendamment de l'interface web.

**Pourquoi utiliser des interfaces (IEtudiantService, IEtudiantRepository) ?**
Courte : pour injecter une implémentation sans que le code appelant sache laquelle.
Si insiste : c'est ce qui permet l'injection de dépendances, et de remplacer facilement une implémentation par une autre.

**Comment les tables sont-elles liées ?**
Courte : avec des clés étrangères — par exemple Etudiant a un DepartementId.
Si insiste : Departement → Enseignant/Etudiant/Cours, Enseignant → Cours, Etudiant → Inscription, Cours → Inscription, Inscription → Note.

**Où est la contrainte d'unicité du numéro étudiant ?**
Courte : dans `EtudiantConfiguration.cs`, un index unique sur `NumeroEtudiant`.

**Comment fonctionne l'inscription d'un étudiant à un cours ?**
Courte : je crée une ligne dans la table Inscription avec l'EtudiantId et le CoursId. Une contrainte empêche la même paire d'exister deux fois.

**Comment fonctionne Identity ?**
Courte : Identity gère les comptes, les mots de passe (hashés), les rôles et la connexion via un cookie. J'ai étendu IdentityUser avec ApplicationUser pour ajouter le lien vers ma fiche métier.

**Quelle est la différence entre authentification et autorisation ?**
Courte : authentification = se connecter, autorisation = avoir le droit de faire une action précise.

**Comment les rôles sont-ils créés ?**
Courte : au démarrage de l'application, par `RoleSeeder.cs`, s'ils n'existent pas déjà.

**Pourquoi le Controller n'accède jamais directement au DbContext ?**
Courte : parce que ça casserait la séparation des couches — Application ne doit rien savoir d'Entity Framework.

**Qu'est-ce qui n'est pas encore fait dans le projet ?**
Courte, honnête : pas de tests automatisés, pas de modification d'un compte utilisateur après sa création, pas d'historique des actions. Je le dis clairement si on me demande, plutôt que de prétendre le contraire.

---

## 11. Fiche de révision rapide

| Notion | En une ligne |
|---|---|
| Domain | Entités métier + enums, ne dépend de rien |
| Application | Services, DTOs, interfaces, validation — dépend de Domain |
| Infrastructure | EF Core, Identity, repositories — dépend de Application |
| Web | Controllers, vues Razor — dépend de Application et Infrastructure |
| Repository | Accès basique à une table, cache EF Core à Application |
| Service | Contient la logique, utilisé par le Controller, renvoie des DTOs |
| DTO | Objet simple pour transporter des données, jamais l'entité directement |
| DbContext | Ma connexion à la base, un DbSet par table |
| Fluent API | Configuration des règles des entités, dans les fichiers Configuration |
| Migration | Fichier généré qui décrit un changement de structure de la base |
| Controller | Reçoit la requête HTTP, appelle le Service, renvoie une vue |
| Model binding | ASP.NET Core remplit automatiquement mon DTO depuis le formulaire |
| ApplicationUser | IdentityUser + Nom/Prenom + lien optionnel vers Etudiant/Enseignant |
| Authentification | Se connecter |
| Autorisation | Avoir le droit de faire une action |
| Bulletin | Recalculé à chaque consultation, jamais stocké |

---

*Rappel perso : si le jury ouvre un fichier et demande « explique-moi ça », je réponds toujours dans cet ordre : c'est quoi → pourquoi je l'ai fait comme ça → comment ça marche concrètement. Je ne pars jamais dans une théorie générale si on ne me la demande pas.*
