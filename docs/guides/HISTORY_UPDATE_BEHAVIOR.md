# Modification des éléments de l'historique - Mise à jour SQLite

## ✅ Réponse : OUI, la base de données se met à jour automatiquement

Quand vous modifiez un élément de l'historique, la base de données SQLite est automatiquement mise à jour via un mécanisme de détection d'écriture.

## 🔄 Flux de modification

### Étape 1 : Charger un calcul depuis l'historique
```
Double-clic sur un élément de l'historique
↓
La méthode lstHistorique_MouseDoubleClick() est exécutée
↓
Le calcul est chargé dans les champs de la fenêtre principale
```

### Étape 2 : Modifier le calcul
```
Modifier un ou plusieurs champs :
- Déboursé sec
- Prix vente HT
- TVA
- Frais généraux
- Titre (optional)
↓
Appuyer sur "Calculer" ou "Enter"
```

### Étape 3 : Sauvegarde avec détection d'écriture
```
Quand le bouton "Calculer" est cliqué :

1. Nouveau calcul effectué
2. Titre du calcul récupéré
3. Vérification si ce titre existe déjà dans l'historique
4. SI LE TITRE EXISTE :
   ↓
   Message de confirmation :
   "Un calcul avec le titre 'XXX' existe déjà.
    Voulez-vous le mettre à jour ?"
   
   Si OUI :
   - Récupération de l'index de l'élément existant
   - Suppression de l'ancienne entrée de SQLite (DeleteEntry)
   - Ajout de la nouvelle entrée de SQLite (AddEntry)
   - Rechargement complet de l'historique depuis SQLite
   
   Si NON :
   - Annulation de l'opération
   - Calcul non sauvegardé

5. SI LE TITRE N'EXISTE PAS :
   - Ajout direct de la nouvelle entrée (AddEntry)
   - Rechargement de l'historique
```

## 📝 Code concerné

### Méthode : btnCalculer_Click()

```csharp
// Vérifier les doublons
var existingResult = _resultsList.FirstOrDefault(r => r.Titre == result.Titre);
int indexHistorique = -1;

if (existingResult != null)
{
    // Demander confirmation pour écraser
    var reponse = MessageBox.Show(
        $"Un calcul avec le titre \"{result.Titre}\" existe déjà.\n\nVoulez-vous le mettre à jour ?",
        "Confirmation",
        MessageBoxButton.YesNo,
        MessageBoxImage.Question);

    if (reponse == MessageBoxResult.Yes)
    {
        // Trouver l'index pour récupération
        indexHistorique = _resultsList.IndexOf(existingResult);
    }
    else
    {
        return; // Annuler
    }
}

// Créer la nouvelle entrée
var historyEntry = new HistoryEntry { ... };

// SI MODIFICATION (on a trouvé un existant)
if (indexHistorique >= 0 && _historyEntryMap.ContainsKey(indexHistorique))
{
    // Supprimer l'ancienne entrée de SQLite
    var existingEntry = _historyEntryMap[indexHistorique];
    _databaseService.DeleteEntry(existingEntry.Id);
}

// Ajouter la nouvelle entrée à SQLite
long newId = _databaseService.AddEntry(historyEntry);
historyEntry.Id = (int)newId;

// IMPORTANT : Recharger depuis SQLite pour synchroniser
ChargerHistorique();
```

## 🎯 Comportement selon le titre

### Cas 1 : Titre NOUVEAU
```
Calcul original : "Titre A" (sauvegardé)
Nouvelle modification : "Titre B" (nouveau)
↓
Résultat : DEUX entrées dans SQLite
- "Titre A" (original)
- "Titre B" (nouvelle)
```

### Cas 2 : Titre EXISTANT (modification)
```
Calcul original : "Titre A" (sauvegardé)
Modification : Changer le montant, garder "Titre A"
↓
Résultat : UNE SEULE entrée dans SQLite
- "Titre A" (MISE À JOUR avec nouvelles valeurs)
```

### Cas 3 : Annulation
```
Calcul original : "Titre A" (sauvegardé)
Modification : Changer montant, garder "Titre A"
Confirmation : Cliquer NON
↓
Résultat : UNE SEULE entrée dans SQLite
- "Titre A" (INCHANGÉ - l'original)
```

## ✅ Synchronisation

### Rechargement automatique après modification
```
Après chaque modification sauvegardée :

1. ChargerHistorique() est appelé automatiquement
2. Tous les éléments sont rechargés depuis SQLite
3. La ListBox d'historique est mise à jour
4. Le tableau de bord est rafraîchi
5. Les données en mémoire sont resynchronisées
```

### Points de synchronisation
- ✅ **Au démarrage** : Charge depuis SQLite
- ✅ **Après calcul** : Sauvegarde + reload
- ✅ **Via gestionnaire de BDD** : Rechargement au fermeture
- ✅ **Après sauvegarde export** : Données à jour
- ✅ **Après restauration** : Rechargement complet

## 🔐 Intégrité des données

### Prévention des duplicatas
- Le système détecte les titres identiques
- Une confirmation est demandée avant écrasement
- L'utilisateur peut annuler la modification

### Traçabilité
- La date de calcul est mise à jour à DateTime.Now
- L'ancien enregistrement est supprimé proprement
- Aucun calcul n'est "oublié" ou dupliqué

### Cohérence
- SQLite reste la source unique de vérité
- Rechargement après chaque modification
- Les listes en mémoire sont synchronisées

## ⚙️ Détails techniques

### Classes impliquées
- `DatabaseService` - Gestion SQLite (AddEntry, DeleteEntry)
- `HistoryEntry` - Modèle de données
- `MainWindow.xaml.cs` - Logique de modification

### Méthodes clés
1. `btnCalculer_Click()` - Détecte modifications et sauvegarde
2. `ChargerHistorique()` - Recharge depuis SQLite
3. `lstHistorique_MouseDoubleClick()` - Charge un élément
4. `UpdateDashboard()` - Met à jour les statistiques

## 🧪 Test du comportement

### Scénario 1 : Modification simple
1. Calculer avec titre "Test"
2. Double-cliquer sur "Test" dans l'historique
3. Modifier le montant de déboursé sec
4. Cliquer "Calculer"
5. Confirmer la mise à jour
6. **Résultat** : SQLite contient la version mise à jour

### Scénario 2 : Nouveau titre
1. Calculer avec titre "Test"
2. Double-cliquer sur "Test"
3. Modifier le titre en "Test v2"
4. Cliquer "Calculer"
5. **Résultat** : SQLite contient DEUX entrées ("Test" et "Test v2")

### Scénario 3 : Annulation
1. Calculer avec titre "Test"
2. Double-cliquer sur "Test"
3. Modifier le montant
4. Cliquer "Calculer"
5. Cliquer "Non" à la confirmation
6. **Résultat** : SQLite inchangé, calcul original intact

## 📊 État de la base après chaque action

| Action | Avant | Après | SQLite |
|--------|-------|-------|--------|
| Nouveau calcul | 1 entrée | 2 entrées | ✅ 2 entrées |
| Modification (même titre) | 2 entrées | 2 entrées | ✅ 2 entrées (1 mise à jour) |
| Modification (nouveau titre) | 2 entrées | 3 entrées | ✅ 3 entrées |
| Annulation | 3 entrées | 3 entrées | ✅ 3 entrées (inchangé) |

## 📌 Résumé

✅ **OUI, la base de données est mise à jour automatiquement**

- Détection intelligente des doublons par titre
- Confirmation avant écrasement
- Suppression de l'ancien enregistrement
- Ajout du nouveau enregistrement
- Rechargement complet de la base
- Synchronisation des statistiques et affichage

**La base SQLite reste toujours cohérente et à jour !**

---

**Version** : 2.2.0  
**Date** : 14 janvier 2026  
**Module** : MainWindow (btnCalculer_Click, ChargerHistorique)
