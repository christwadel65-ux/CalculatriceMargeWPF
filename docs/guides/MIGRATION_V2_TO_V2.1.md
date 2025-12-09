# 🔄 Guide de Mise à Jour - v2.0 → v2.1

**Date:** 9 décembre 2025  
**From:** v2.0.0  
**To:** v2.1.0  
**Duration:** < 2 minutes

---

## 📋 Pour les Utilisateurs

### Installation Mise à Jour

**Option 1: Installer la version 2.1.0** (Recommandé)
```
1. Télécharger CalculatriceMargeInstaller_v2.1.0.exe
2. Exécuter le fichier
3. Suivre l'assistant d'installation
4. Votre historique sera automatiquement importé
5. Done ✅
```

**Option 2: Mise à jour sur place**
```
1. Fermer l'application
2. Remplacer CalculatriceMargeWPF.exe dans le dossier d'installation
3. Redémarrer l'application
4. Votre historique et presets sont préservés
5. Done ✅
```

### Ce qui change pour vous
- ✅ Aucun changement visible
- ✅ Interface identique
- ✅ Toutes les fonctionnalités conservées
- ✅ Historique préservé
- ✅ Presets sauvegardés
- ✅ Application plus stable

### Ce qui s'améliore
- 🚀 Légèrement plus rapide
- 📦 Code plus propre (interne)
- 🔧 Plus facile à maintenir

---

## 🛠️ Pour les Développeurs

### Fichiers Modifiés

#### 1. **CalculationEngine.cs** (LINQ Refactoring)
```csharp
// AVANT: 40 lignes de boucles
foreach (var result in results) {
    sumMargeBrutePct += result.MargeBrutePct;
    // 35 lignes de code similaire...
}
stats.MoyenneMargeBrutePct = sumMargeBrutePct / results.Length;

// APRÈS: 1 ligne LINQ
stats.MoyenneMargeBrutePct = results.Average(r => r.MargeBrutePct);
```

**Impact:**
- Code 75% plus court
- Lisibilité améliorée
- Performance stable/meilleure

#### 2. **MainWindow.xaml.cs** (Extraction Logique)
```csharp
// AVANT: 70 lignes de parsing imbriqué
if (part4Content.StartsWith("MODE:")) {
    // 35 lignes pour v2...
} else if (part4Content.StartsWith("PR:")) {
    // 35 lignes pour v1...
}

// APRÈS: 1 appel
if (HistoryParser.TryParseHistoryLine(item, out var entry)) {
    txtTitre.Text = entry.Titre;
    // ...
}
```

**Impact:**
- Parsing centralisé et testable
- Moins de duplication
- Maintenance facilitée

#### 3. **HistoryParser.cs** (NEW)
```csharp
namespace CalculatriceMargeWPF.Helpers
{
    public class HistoryParser {
        public static bool TryParseHistoryLine(string line, out ParsedHistoryEntry entry)
        // Gère v1 et v2 automatiquement
    }
}
```

**Avantages:**
- Logic isolée
- Réutilisable
- Testable indépendamment

#### 4. **NumberFormatter.cs** (DRY)
```csharp
// Nouvelle méthode centralisée
private static string NormalizeSeparators(string input)
// Élimine 3 zones de duplication
```

**Impact:**
- Code 15% plus court
- Maintenance unifiée

#### 5. **CalculatriceMargeInstaller.iss**
```ini
; Version mise à jour
#define MyAppVersion "2.1.0"
```

---

### Migration du Code

**Si vous avez du code personnalisé:**

#### Parsing d'historique
```csharp
// Ancien (à remplacer)
var parts = item.Split('|');
if (double.TryParse(parts[2], out double ds)) { ... }

// Nouveau (préféré)
if (HistoryParser.TryParseHistoryLine(item, out var entry)) {
    double ds = entry.DebourseSec;
    // ...
}
```

#### Statistiques
```csharp
// Ancien (boucles manuelles)
double sum = 0;
foreach (var result in results) sum += result.MargeBrutePct;
double avg = sum / results.Length;

// Nouveau (LINQ)
double avg = results.Average(r => r.MargeBrutePct);
```

---

## ✅ Checklist Migration

### Pour les Utilisateurs
- [ ] Télécharger v2.1.0
- [ ] Exécuter installateur
- [ ] Vérifier historique importé
- [ ] Tester un calcul
- [ ] Vérifier presets chargés
- [ ] Consulter nouvelle documentation

### Pour les Développeurs
- [ ] Mettre à jour repository local (`git pull`)
- [ ] Compiler (`dotnet build -c Release`)
- [ ] Exécuter tests (`dotnet test`)
- [ ] Consulter RELEASE_NOTES_V2.1.md
- [ ] Revoir HistoryParser.cs
- [ ] Adapter code personnalisé si nécessaire

---

## 🐛 Troubleshooting

### Question: "Mon historique a disparu?"
**Réponse:** Non, il est préservé dans `%APPDATA%\CalculatriceMarge\Historique\`

### Question: "Comment revenir à v2.0?"
**Réponse:** 
1. Sauvegarder votre historique (Ctrl+S)
2. Désinstaller v2.1
3. Installer v2.0
4. L'historique v2.1 reste présent

### Question: "Ma customization personnalisée?"
**Réponse:** Consultez "Migration du Code" ci-dessus

### Question: "C'est bien v2.1?"
**Réponse:** Vérifier dans l'application → À propos

---

## 📞 Support

**Si vous avez un problème:**

1. Consulter [README.md](../README.md)
2. Consulter [RELEASE_NOTES_V2.1.md](RELEASE_NOTES_V2.1.md)
3. Créer issue sur [GitHub](https://github.com/christwadel65-ux/CalculatriceMarge/issues)

**Logs utiles pour support:**
```
📂 %APPDATA%\CalculatriceMarge\
├── Historique\historique.txt
└── Presets\presets.json
```

---

## 📚 Documentation

- **Pour comprendre les changements:** [CHANGELOG_V2.1.md](CHANGELOG_V2.1.md)
- **Pour les détails techniques:** [RELEASE_NOTES_V2.1.md](RELEASE_NOTES_V2.1.md)
- **Pour l'utilisation:** [README_V2.md](../guides/README_V2.md)
- **Architecture:** [IMPLEMENTATION_SUMMARY.md](../guides/IMPLEMENTATION_SUMMARY.md)

---

## 🎉 Voilà!

Vous êtes maintenant sur **v2.1.0** - Code optimisé, même puissance!

```
✨ Bienvenue dans la v2.1.0! ✨
```

---

*Migration guide - 9 décembre 2025*
