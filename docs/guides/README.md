# Calculatrice de Marge v2.2.0

**Une application WPF professionnelle pour calculer les marges commerciales avec précision.**

## 🎯 Fonctionnalités principales

### Calcul des marges
- ✅ Calcul de marge brute et nette en temps réel
- ✅ Gestion des frais généraux (en % ou €)
- ✅ TVA configurable (20%, 5.5%, 10% ou personnalisée)
- ✅ Remises applicables avec recalcul des marges
- ✅ Calcul inversé (trouver le prix de vente pour une marge cible)

### Gestion des données
- ✅ Historique automatique de tous les calculs
- ✅ Présets rapides (Standard, Réduit, Service)
- ✅ Modification/mise à jour des calculs existants
- ✅ Export CSV de l'historique
- ✅ Statistiques complètes (moyenne, min, max, écart-type)

### Interface
- ✅ Design moderne et compact (780x720px)
- ✅ Entrées et paramètres côte à côte
- ✅ Résultats en temps réel avec code couleur
- ✅ Affichage des remises appliquées
- ✅ Support du clavier (Enter pour calculer)

## 📋 Détails des calculs

### Formules principales
```
Prix de revient = Déboursé sec + Frais généraux
Price After Discount = Prix vente HT × (1 - Remise%)
Marge brute = Prix vente HT - Prix revient
Marge nette = Marge brute - TVA
```

### Exemple de calcul
```
Déboursé sec:      1 000 €
Frais généraux:    25% = 250 €
Prix revient:      1 250 €
─────────────────────────────
Prix vente HT:    10 000 €
Remise:            10% = 1 000 €
Prix après remise: 9 000 €
─────────────────────────────
Marge brute:       7 750 € (77,50%)
TVA (20%):         1 500 €
Marge nette:       6 250 € (62,50%)
─────────────────────────────
Prix TTC:         10 800 €
```

## 🚀 Installation

### Prérequis
- Windows 10/11
- .NET 10.0 ou supérieur

### Téléchargement
1. Télécharger la dernière version depuis les releases
2. Extraire le dossier `publish`
3. Double-cliquer sur `CalculatriceMargeWPF.exe`

### À partir des sources
```powershell
# Cloner le repository
git clone <url-repo>
cd CalculatriceMargeWPF

# Build et lancement
dotnet build
dotnet run
```

## ⌨️ Raccourcis clavier

| Raccourci | Action |
|-----------|--------|
| **Enter** | Calculer |
| **Ctrl+Z** | Annuler (Undo) |
| **Ctrl+Y** | Rétablir (Redo) |
| **Ctrl+S** | Sauvegarder l'historique |
| **Ctrl+N** | Nouveau calcul (Reset) |

## 📊 Données affichées

### Entrées
- Titre du calcul
- Déboursé sec (€)
- Prix revient (€) *automatique*
- Prix de vente HT (€)
- Remise (%)
- Prix après remise (€) *automatique*

### Paramètres
- Frais généraux (€ ou %)
- TVA (%)
- Presets rapides

### Résultats
- **Déboursé sec** - Montant initial
- **Prix revient** - Coût total (déboursé + frais)
- **Prix HT** - Prix de vente avant remise/TVA
- **Prix TTC** - Prix final avec TVA
- **Marge brute** - Différence brute (€ et %)
- **Marge nette** - Différence nette après TVA (€ et %)
- **Remise appliquée** - Pourcentage et montant de remise

## 💾 Historique

Les calculs sont **automatiquement sauvegardés** dans:
```
%AppData%\CalculatriceMarge\Historique\historique.txt
```

### Gestion de l'historique
- Double-cliquer sur une ligne pour charger le calcul
- Supprimer une ligne individuellement
- Nettoyer tout l'historique
- Export CSV disponible

## 📈 Statistiques

Affiche pour tous les calculs:
- Nombre total de calculs
- Chiffre d'affaires TTC total
- Marge nette totale
- Moyennes, min, max et écart-type (marge brute et nette)

## ⚙️ Paramètres avancés

### Présets disponibles
1. **Standard** - TVA 20%, Frais 25%
2. **Réduit** - TVA 5.5%, Frais 8%
3. **Service** - TVA 10%, Frais 15%

### Frais généraux
- Flexible: en pourcentage (%) ou montant fixe (€)
- Par défaut: 25%

### TVA
- Configurable de 0% à 99%
- Affectée uniquement à l'affichage du prix TTC

## 🔄 Modification de calculs

Si vous entrez un titre existant:
1. L'application demande confirmation
2. **Oui** → Met à jour le calcul à la même position dans l'historique
3. **Non** → Annule

L'historique conserve son ordre chronologique.

## 📁 Structure des fichiers

```
CalculatriceMargeWPF/
├── src/
│   ├── Views/           # Interfaces XAML
│   ├── ViewModels/      # Logique de présentation
│   ├── Models/          # Moteurs de calcul
│   ├── Helpers/         # Utilitaires
│   └── Resources/       # Ressources (images, styles)
├── tests/               # Tests unitaires
├── bin/                 # Fichiers compilés
├── README.md            # Documentation principale
└── CalculatriceMargeWPF.sln
```

## 🐛 Troubleshooting

### L'application ne se lance pas
- Vérifier que .NET 10.0 est installé: `dotnet --version`
- Relancer depuis `bin/Debug/net10.0-windows/CalculatriceMargeWPF.exe`

### L'historique ne se sauvegarde pas
- Vérifier les permissions sur `%AppData%\CalculatriceMarge\`
- Redémarrer l'application

### Les marges ne sont pas correctes
- Vérifier que le "Prix de vente HT" est rempli
- Vérifier que la TVA n'est pas à 0%
- Appuyer sur **Enter** pour recalculer

## 📝 Notes de version

### v2.2.0 (04/01/2026)
- ✨ Interface complètement refactorisée (design compact et moderne)
- ✨ Affichage des remises appliquées côte à côte avec Prix TTC
- ✨ Recalcul des marges avec remises
- ✨ Modification in-place de calculs existants (même position historique)
- ✨ Validation souple des champs (valeurs par défaut)
- ✨ Support complet du clavier (Enter pour calculer)
- 🐛 Correction du calcul du montant de remise (sur prix initial)
- 🐛 Amélioration de la gestion de l'historique (UTF-8)

### v2.0.0
- Refonte complète avec MVVM
- Historique persistant
- Undo/Redo
- Statistiques
- Export CSV

## 📧 Support

Pour toute question ou bug, consultez:
- [Documentation](./docs/README.md)
- [Guide d'implémentation](./docs/guides/IMPLEMENTATION_SUMMARY.md)

## 📄 Licence

Voir [LICENSE.txt](LICENSE.txt)

---

**Développée avec ❤️ en C# et WPF**

Version: **2.2.0**  
Date: 4 janvier 2026
