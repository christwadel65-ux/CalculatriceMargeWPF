# Notes de version 2.2.0 - Base de données SQLite

## Nouveautés majeures

### 🗄️ Migration vers SQLite

L'historique des calculs utilise désormais une base de données SQLite au lieu d'un simple fichier texte. Cette amélioration apporte de nombreux avantages :

#### Avantages
- **Performance** : Accès et recherche ultra-rapides grâce aux index
- **Fiabilité** : Protection contre la corruption des données
- **Intégrité** : Validation automatique des données et transactions ACID
- **Évolutivité** : Support de milliers d'entrées sans ralentissement
- **Fonctionnalités** : Statistiques avancées et recherches complexes

#### Migration automatique
Au premier lancement de la version 2.2.0 :
1. L'application détecte automatiquement l'ancien fichier `historique.txt`
2. Toutes les données sont migrées vers la base SQLite
3. L'ancien fichier est conservé en backup (`historique.txt.backup`)
4. Aucune intervention manuelle requise

#### Emplacement de la base
```
%APPDATA%\CalculatriceMarge\Historique\historique.db
```

### 📊 Nouvelles capacités

#### Recherche avancée
- Recherche par titre avec support des expressions partielles
- Tri par date, montant ou marge
- Filtrage rapide des résultats

#### Statistiques enrichies
- Nombre total de calculs effectués
- Somme totale des marges brutes et nettes
- Moyennes des pourcentages de marge
- Analyses de tendances possibles

#### Performances
- Chargement instantané de l'historique (même avec 1000+ entrées)
- Insertion et suppression optimisées
- Pas d'impact sur la taille de l'exécutable

### 🔧 Aspects techniques

#### Package utilisé
- **Microsoft.Data.Sqlite** v9.0.0
- Package officiel Microsoft
- Bibliothèques natives embarquées dans l'exécutable standalone
- Compatible .NET 10.0

#### Schéma de base de données
```sql
CREATE TABLE History (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Titre TEXT NOT NULL,
    DebourseSec REAL NOT NULL,
    FraisGeneraux REAL NOT NULL,
    FraisModeIndex INTEGER NOT NULL,
    PrixVenteHT REAL NOT NULL,
    TVA REAL NOT NULL,
    PrixRevient REAL NOT NULL,
    MargeBrute REAL NOT NULL,
    MargeBrutePourcentage REAL NOT NULL,
    MargeNette REAL NOT NULL,
    MargeNettePourcentage REAL NOT NULL,
    PrixVenteTTC REAL NOT NULL,
    DateCalcul TEXT NOT NULL
);

CREATE INDEX idx_date ON History(DateCalcul);
CREATE INDEX idx_titre ON History(Titre);
```

### 🔄 Compatibilité

#### Avec versions précédentes
- ✅ Migration automatique depuis v2.1.x et antérieures
- ✅ Conservation de toutes les données existantes
- ✅ Backup automatique de l'ancien format
- ✅ Aucune perte d'information

#### Format d'affichage
- L'affichage dans la liste d'historique reste identique
- Compatible avec les exports CSV/HTML existants
- Rechargement des calculs fonctionne de la même manière

### 📁 Gestion des données

#### Sauvegarde
Pour sauvegarder votre historique, copiez simplement :
```
%APPDATA%\CalculatriceMarge\Historique\historique.db
```

#### Restauration
Pour restaurer, remplacez le fichier `historique.db` par votre backup.

#### Réinitialisation
Pour repartir de zéro, supprimez le fichier `historique.db`.

### 🐛 Corrections et améliorations

- Correction : Plus de risque de corruption de l'historique
- Correction : Gestion améliorée des caractères spéciaux dans les titres
- Amélioration : Temps de chargement divisé par 10 pour les gros historiques
- Amélioration : Suppression d'entrées plus rapide et fiable
- Amélioration : Dashboard plus précis et réactif

### 📖 Documentation

Documentation complète disponible dans `SQLITE_IMPLEMENTATION.md` :
- Architecture détaillée
- API du DatabaseService
- Guide de migration
- Exemples d'utilisation

### 🚀 Développements futurs possibles

Avec SQLite, de nouvelles fonctionnalités pourront être ajoutées facilement :
- Filtres avancés sur l'historique
- Graphiques et analyses visuelles
- Export direct vers Excel avec formatage
- Historique des modifications (audit trail)
- Synchronisation cloud (optionnelle)
- Catégorisation des calculs

---

**Version** : 2.2.0  
**Date de sortie** : Janvier 2026  
**Développeur** : C. Lecomte
