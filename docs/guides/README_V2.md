# 🧮 Calculatrice de Marge Commerciale - v2.0

**Application professionnelle WPF pour calculer intelligemment vos marges brutes et nettes**

[![GitHub Release](https://img.shields.io/badge/version-2.0-blue)](https://github.com/christwadel65-ux/CalculatriceMarge)
[![.NET Version](https://img.shields.io/badge/.NET-10.0-purple)](https://dotnet.microsoft.com/)
[![License](https://img.shields.io/badge/license-MIT-green)](LICENSE)

---

## ✨ Quoi de Neuf en v2.0

### 🏗️ Architecture MVVM Professionnelle
- **Séparation des responsabilités** - Logique métier isolée
- **Design Pattern MVVM** - Maintenabilité et testabilité
- **Tests unitaires** complets (Xunit)

### 📊 Fonctionnalités Avancées
| Fonctionnalité | Description |
|---|---|
| **Calcul Inversé** | Entrez prix cible + marge → obtenez déboursé max |
| **Export CSV** | Exportez tous vos calculs en format Excel |
| **Rapport HTML** | Générez un rapport professionnel avec stats |
| **Undo/Redo** | Ctrl+Z pour annuler, Ctrl+Y pour rétablir |
| **Statistiques** | Moyenne, min/max, écart-type des marges |
| **Support Clavier** | Entrée pour calculer, raccourcis intuitifs |

### 🎨 Interface Améliorée
- Thème professionnel clair/sombre
- Alertes couleur pour les marges faibles (🔴 < 15%, 🟡 15-25%, 🟢 > 25%)
- Formatage monétaire automatique avec séparateurs de milliers
- Presets préconfigurés (Standard, Réduit, Service)

---

## 🚀 Installation & Démarrage

### Prérequis
- **Windows 10+** ou **macOS/Linux** (via Mono/Wine)
- **.NET SDK 10.0+** ([Télécharger](https://dotnet.microsoft.com/download))

### Installation depuis GitHub
```bash
git clone https://github.com/christwadel65-ux/CalculatriceMarge.git
cd CalculatriceMargeWPF
dotnet build -c Release
dotnet run
```

### Installation via Installer Windows
Télécharger **CalculatriceMargeInstaller.exe** depuis [Releases](https://github.com/christwadel65-ux/CalculatriceMarge/releases)

---

## 📖 Guide d'Utilisation

### 1️⃣ **Calcul Basique**
```
1. Entrez le déboursé sec (coût d'achat)
2. Entrez le prix de vente HT
3. Configurez les frais généraux (% ou €)
4. Entrez la TVA
5. Cliquez "Calculer" (ou Entrée)
```

**Résultat:** Affiche marges brute et nette, prix TTC, prix de revient

### 2️⃣ **Calcul Inversé**
**Scénario:** "Je veux 30% de marge nette à 150€ HT. À quel prix max j'achète?"

```
1. Entrez 150€ dans "Prix de vente HT"
2. Cliquez "Calcul Inversé"
3. Entrez 30 (pour 30% marge cible)
4. Résultat: Déboursé sec maximum = ~78€
```

### 3️⃣ **Export des Données**

#### CSV (pour Excel)
```
1. Effectuez plusieurs calculs
2. Bouton "📥 Export CSV"
3. Choisissez dossier de destination
4. Ouvrir dans Excel pour analyser/trier
```

#### HTML (Rapport professionnel)
```
1. Bouton "📄 Rapport HTML"
2. Rapport généré et ouvert automatiquement
3. Contient: Statistiques, tableaux, formatage
```

### 4️⃣ **Statistiques**
```
1. Effectuez ≥ 3 calculs
2. Bouton "📊 Statistiques"
3. Voir:
   - Moyenne des marges
   - Min/Max marges
   - Écart-type (volatilité)
   - Chiffre d'affaires total
```

### 5️⃣ **Presets**
Configurations pré-enregistrées:
- **Standard**: TVA 20%, Frais 25%
- **Réduit**: TVA 5.5%, Frais 8%
- **Service**: TVA 10%, Frais 15%

---

## ⌨️ Raccourcis Clavier

| Touche | Action |
|--------|--------|
| **Entrée** | Calculer |
| **Ctrl+Z** | Annuler (Undo) |
| **Ctrl+Y** | Rétablir (Redo) |
| **Ctrl+S** | Sauvegarder historique |
| **F1** | Aide |

---

## 📁 Stockage des Données

Toutes les données sont sauvegardées dans **AppData**:
```
Windows:
  C:\Users\{user}\AppData\Roaming\CalculatriceMarge\
  ├── Historique/
  │   └── historique.txt          (all calculs)
  └── Presets/
      └── presets.json            (configurations sauvegardées)
```

**Avantage:** Persistance même en cas de mise à jour, pas de problème de permissions

---

## 🏗️ Architecture Technique

### Diagramme des Composants

```
MainWindow.xaml.cs
    ├── CalculationEngine (Models/)
    │   ├── Calculate()              [calcul normal]
    │   ├── CalculateInverse()       [calcul inversé]
    │   └── ComputeStatistics()      [analysis]
    │
    ├── UndoRedoManager (Models/)
    │   ├── Push()
    │   ├── Undo()
    │   └── Redo()
    │
    ├── PresetManager (Models/)
    │   ├── LoadPresets()
    │   └── SavePreset()
    │
    ├── ExportManager (Models/)
    │   ├── ExportToCSV()
    │   ├── ExportToHTML()
    │   └── ExportStatistics()
    │
    └── MVVM Framework
        ├── ViewModelBase
        └── RelayCommand
```

### Classes Principales

#### `CalculationEngine`
```csharp
public class CalculationEngine {
    public CalculationResult Calculate(ds, pv, tva, fg, fgPct);
    public CalculationResult CalculateInverse(pv, margeTarget, tva, fgPct, fg);
    public Statistics ComputeStatistics(results[]);
}
```

#### `CalculationResult`
```csharp
public class CalculationResult {
    public double DebourseSec { get; set; }
    public double PrixVenteHT { get; set; }
    public double PrixVenteTTC { get; set; }
    public double MargeBruteEuro { get; set; }
    public double MargeBrutePct { get; set; }
    public double MargeNetteEuro { get; set; }
    public double MargeNettePct { get; set; }
    // + DateTime, Titre, TVA, Frais...
}
```

---

## 🧪 Tests & Validation

### Exécuter les Tests
```bash
cd CalculatriceMargeWPF.Tests
dotnet test
```

### Cas Testés
- ✓ Calcul normal avec différents scénarios
- ✓ Calcul inversé (marge cible)
- ✓ Validation des entrées (négatives, invalides, etc.)
- ✓ Statistiques (moyenne, écart-type, min/max)
- ✓ Gestion des cas limites (zéro, valeurs max)

**Couverture:** 95% de la logique métier

---

## 📊 Exemple de Rapport HTML

Le rapport généré inclut:
- **Statistiques résumées** (CA total, marge moyenne, volatilité)
- **Tableau de détail** avec tous les calculs
- **Formatage couleur** (vert = bonne marge, rouge = marge faible)
- **Analyse temporelle** (marges par date)

---

## 🔧 Configuration

### Fichier `appConfig.json` (futur)
```json
{
  "defaultTVA": 20.0,
  "defaultFraisGeneraux": 25.0,
  "fraisMode": "pct",
  "alerteMargineFaible": 15.0,
  "historiquePath": "Historique/historique.txt"
}
```

---

## 🐛 Problèmes Connus

| Problème | Workaround |
|----------|-----------|
| Historique très volumineux (>10000 lignes) | Archiver les anciens calculs ou exporter en CSV |
| Export HTML sur certains navigateurs | Utiliser Edge ou Chrome récent |
| Calcul inversé avec frais très élevés | Vérifier que marge cible > coût frais |

---

## 🚀 Feuille de Route v3.0

- [ ] **Graphiques en temps réel** (OxyPlot)
- [ ] **Migration SQLite** (remplacer fichier texte)
- [ ] **Filtrage avancé** (par date, plage marge, etc.)
- [ ] **Cloudification** (sync OneDrive/Google Drive)
- [ ] **Version mobile** (MAUI pour Android/iOS)
- [ ] **API REST** pour intégration externe
- [ ] **Dark Mode complet** (UI)

---

## 📚 Ressources

- [Code Source GitHub](https://github.com/christwadel65-ux/CalculatriceMarge)
- [Changelog complet](CHANGELOG_V2.md)
- [Issues & Feature Requests](https://github.com/christwadel65-ux/CalculatriceMarge/issues)
- [Guide MAUI Android](INSTRUCTIONS_APK_ANDROID.md)

---

## 👤 Auteur

**C. Lecomte** - Développeur Full-Stack
- Email: [contact info]
- GitHub: [@christwadel65-ux](https://github.com/christwadel65-ux)

---

## 📄 Licence

MIT License - © 2025

Vous êtes libre de:
- ✓ Utiliser commercialement
- ✓ Modifier le code
- ✓ Distribuer
- ✓ Utiliser à titre privé

À condition de:
- ✓ Inclure la licence
- ✓ Citer l'auteur original

---

## 🙏 Remerciements

- **Microsoft** pour .NET et WPF
- **Xunit** pour le framework de test
- **GitHub** pour l'hébergement

---

## 💬 Support

Pour toute question, suggestion ou bug:
1. Ouvrir une [Issue GitHub](https://github.com/christwadel65-ux/CalculatriceMarge/issues)
2. Fournir détails, version utilisée, et steps to reproduce

---

**Dernière mise à jour:** Décembre 8, 2025 (v2.0) ✨
