# Base de données SQLite pour l'historique

## Vue d'ensemble

L'application CalculatriceMarge utilise désormais SQLite pour stocker l'historique des calculs de manière structurée et performante. Cette implémentation remplace l'ancien système basé sur un fichier texte.

## Architecture

### Nouveaux fichiers

1. **src/Models/HistoryEntry.cs**
   - Modèle représentant une entrée d'historique
   - Contient toutes les données du calcul (DS, FG, HT, marges, etc.)
   - Inclut la date du calcul pour le suivi temporel

2. **src/Models/DatabaseService.cs**
   - Service de gestion de la base de données SQLite
   - Fournit des méthodes CRUD pour l'historique
   - Gère l'initialisation et la création des tables

3. **src/Models/HistoryMigrationService.cs**
   - Service de migration des données
   - Convertit l'ancien fichier historique.txt vers SQLite
   - Préserve l'ancien fichier en backup

## Schéma de la base de données

### Table History

```sql
CREATE TABLE History (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Titre TEXT NOT NULL,
    DebourseSec REAL NOT NULL,
    FraisGeneraux REAL NOT NULL,
    FraisModeIndex INTEGER NOT NULL,      -- 0 = %, 1 = EUR
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

## Fonctionnalités

### DatabaseService

Le service `DatabaseService` offre les méthodes suivantes :

- **AddEntry(entry)** : Ajoute une nouvelle entrée dans l'historique
- **GetAllEntries()** : Récupère toutes les entrées triées par date
- **GetEntryById(id)** : Récupère une entrée spécifique
- **DeleteEntry(id)** : Supprime une entrée
- **ClearHistory()** : Supprime tout l'historique
- **SearchByTitle(searchTerm)** : Recherche par titre (LIKE)
- **GetStatistics()** : Calcule les statistiques de l'historique

### Migration automatique

Au premier lancement après la mise à jour :

1. Le système détecte l'ancien fichier `historique.txt`
2. Parse chaque ligne avec `HistoryParser`
3. Recalcule les valeurs manquantes (marges, prix)
4. Insère les données dans SQLite
5. Renomme l'ancien fichier en `historique.txt.backup`

## Emplacement de la base de données

La base de données est stockée dans :
```
%APPDATA%\CalculatriceMarge\Historique\historique.db
```

## Avantages de SQLite

1. **Performance** : Requêtes indexées et optimisées
2. **Intégrité** : Validation des données et transactions ACID
3. **Recherche** : Recherche rapide par titre, date, etc.
4. **Statistiques** : Calculs agrégés (SUM, AVG, COUNT)
5. **Évolutivité** : Support de milliers d'entrées sans problème
6. **Fiabilité** : Moins de risques de corruption qu'un fichier texte

## Modifications du code

### MainWindow.xaml.cs

- Ajout du champ `_databaseService` et `_historyEntryMap`
- Modification de `ChargerHistorique()` pour lire depuis SQLite
- Modification de `btnCalculer_Click()` pour sauvegarder dans SQLite
- Modification de `btnDeleteSelected_Click()` pour supprimer de SQLite
- Modification de `btnClearHistory_Click()` pour vider SQLite
- Modification de `lstHistorique_MouseDoubleClick()` pour charger depuis SQLite

### Compatibilité

Le système reste compatible avec l'ancien format :
- `HistoryParser` est toujours utilisé pour la migration
- L'affichage dans la ListBox conserve le même format
- Les utilisateurs existants ne perdent aucune donnée

## Développements futurs possibles

1. Ajout d'un système de filtrage avancé (par date, montant, etc.)
2. Export direct depuis la base vers CSV/Excel
3. Statistiques détaillées et graphiques
4. Backup automatique de la base de données
5. Synchronisation cloud (optionnelle)
6. Historique des modifications (audit trail)

## Package NuGet utilisé

- **Microsoft.Data.Sqlite** version 9.0.0
  - Package officiel Microsoft
  - Compatible avec .NET 10.0
  - Inclut les bibliothèques natives SQLite

## Tests recommandés

1. Vérifier la migration depuis un ancien historique.txt
2. Tester l'ajout, modification et suppression d'entrées
3. Vérifier le rechargement d'un calcul par double-clic
4. Tester le vidage complet de l'historique
5. Vérifier les statistiques du dashboard
6. Tester avec un grand nombre d'entrées (>1000)
