# Correction du tableau de bord - SQLite v2.2.0

## Problème identifié

Le tableau de bord (dashboard) n'affichait pas correctement les statistiques chargées depuis la base de données SQLite. Les données manquantes causaient une mauvaise reconstruction des objets `CalculationResult` lors du chargement de l'historique.

### Symptôme
```
Nb calculs : 1
CA TTC total : --
Marge nette moyenne : --
...
```

Le nombre de calculs était incorrect ou incomplet, et les statistiques n'étaient pas mises à jour correctement.

## Root cause

Dans la méthode `ChargerHistorique()` (MainWindow.xaml.cs), lors du chargement des données depuis SQLite, les champs suivants du `CalculationResult` n'étaient pas remplis :

1. **FraisGeneraux** - Montant ou pourcentage des frais
2. **FraisEnPourcent** - Flag indiquant si les frais sont en %
3. **FraisEnEuro** - Valeur calculée en euros
4. **CalculDateTime** - Date et heure du calcul

Ces champs sont nécessaires pour :
- Afficher correctement les statistiques dans `UpdateDashboard()`
- Reconstruire fidèlement les résultats depuis la base SQLite
- Calculer correctement les marges via `ComputeStatistics()`

## Solution appliquée

### Avant
```csharp
var result = new CalculationEngine.CalculationResult
{
    Titre = entry.Titre,
    DebourseSec = entry.DebourseSec,
    PrixVenteHT = entry.PrixVenteHT,
    TVAPct = entry.TVA,
    PrixRevientTotal = entry.PrixRevient,
    MargeBruteEuro = entry.MargeBrute,
    MargeBrutePct = entry.MargeBrutePourcentage,
    MargeNetteEuro = entry.MargeNette,
    MargeNettePct = entry.MargeNettePourcentage,
    PrixVenteTTC = entry.PrixVenteTTC
};
```

### Après
```csharp
double fraisEnEuro = entry.FraisModeIndex == 0 
    ? entry.DebourseSec * (entry.FraisGeneraux / 100) 
    : entry.FraisGeneraux;

var result = new CalculationEngine.CalculationResult
{
    Titre = entry.Titre,
    DebourseSec = entry.DebourseSec,
    FraisGeneraux = entry.FraisGeneraux,                    // ✅ AJOUTÉ
    FraisEnPourcent = entry.FraisModeIndex == 0,            // ✅ AJOUTÉ
    FraisEnEuro = fraisEnEuro,                              // ✅ AJOUTÉ
    PrixVenteHT = entry.PrixVenteHT,
    TVAPct = entry.TVA,
    PrixRevientTotal = entry.PrixRevient,
    MargeBruteEuro = entry.MargeBrute,
    MargeBrutePct = entry.MargeBrutePourcentage,
    MargeNetteEuro = entry.MargeNette,
    MargeNettePct = entry.MargeNettePourcentage,
    PrixVenteTTC = entry.PrixVenteTTC,
    CalculDateTime = entry.DateCalcul                       // ✅ AJOUTÉ
};
```

### Améliorations supplémentaires
1. **Ajout de `_resultsList.Clear()`** au début de `ChargerHistorique()` pour éviter les accumulations de données lors de rechargements multiples

## Fichier modifié

- **[src/Views/MainWindow.xaml.cs](src/Views/MainWindow.xaml.cs)** - Méthode `ChargerHistorique()`

## Résultat

Le tableau de bord affiche maintenant correctement :
- ✅ **Nb calculs** : Nombre exact d'entrées dans la base
- ✅ **CA TTC total** : Somme correcte de tous les prix de vente TTC
- ✅ **Marge nette moyenne** : Pourcentage moyen exact
- ✅ **Marge nette min/max** : Plage correcte des marges nettes
- ✅ **Marge brute moyenne** : Pourcentage moyen exact
- ✅ **Dernier titre** : Titre du dernier calcul effectué

## Vérifications

- ✅ Compilation réussie sans erreurs
- ✅ Application se lance correctement
- ✅ Dashboard remis à jour automatiquement au chargement
- ✅ Compatible avec le gestionnaire de base de données

## Impact

- ✅ Tableau de bord 100% synchronisé avec SQLite
- ✅ Statistiques fiables et à jour
- ✅ Pas de perte de données
- ✅ Pas de impact sur les autres fonctionnalités

---

**Date de correction** : 14 janvier 2026  
**Version** : 2.2.0  
**Modules affectés** : MainWindow.xaml.cs (ChargerHistorique)
