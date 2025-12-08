# 📁 Structure du Projet CalculatriceMarge v2.0

## Aperçu

```
CalculatriceMargeWPF/
├── src/                           # Code source principal
│   ├── App.xaml                   # Point d'entrée de l'application WPF
│   ├── App.xaml.cs                # Code-behind de l'application
│   ├── Views/                     # Vues (interfaces utilisateur)
│   │   ├── MainWindow.xaml        # Interface principale
│   │   └── MainWindow.xaml.cs     # Logique de l'interface principale
│   ├── ViewModels/                # Modèles de vue (MVVM) - Pour v3.0
│   ├── Models/                    # Modèles de données et logique métier
│   │   ├── CalculationEngine.cs   # Moteur de calcul principal
│   │   ├── ExportManager.cs       # Gestion des exports (CSV, HTML)
│   │   ├── PresetManager.cs       # Gestion des présets utilisateur
│   │   └── UndoRedoManager.cs     # Gestion de l'historique undo/redo
│   ├── MVVM/                      # Patterns MVVM
│   │   ├── RelayCommand.cs        # Implémentation de ICommand
│   │   └── ViewModelBase.cs       # Base pour les ViewModel
│   └── Resources/
│       ├── Images/                # Images et icônes
│       │   ├── app_icon.ico
│       │   ├── app_icon.png
│       │   ├── debourse.png
│       │   ├── frais.png
│       │   ├── revient.png
│       │   ├── tva.png
│       │   └── create_icon.py     # Script Python pour générer les icônes
│       └── Styles/                # Ressources de style (à développer)
│
├── docs/                          # Documentation du projet
│   ├── README.md                  # Guide principal
│   ├── guides/                    # Guides détaillés
│   │   ├── README_V2.md           # Architecture et fonctionnalités v2.0
│   │   ├── IMPLEMENTATION_SUMMARY.md  # Résumé de l'implémentation
│   │   ├── INSTALLER_GUIDE.md     # Guide d'installation & Inno Setup
│   │   ├── INSTALLER.md           # Documentation ancienne
│   │   └── INSTRUCTIONS_APK_ANDROID.md  # Guide MAUI (v3.0)
│   └── releases/                  # Notes de version
│       ├── CHANGELOG_V2.md        # Historique des changements v2.0
│       ├── RELEASE_NOTES_V2.md    # Notes de release v2.0.0
│       └── RELEASE_SUMMARY.md     # Résumé de la release
│
├── installer/                     # Scripts d'installation
│   └── CalculatriceMargeInstaller.iss  # Inno Setup script v2.0
│
├── tests/                         # Tests unitaires (à développer)
│   ├── CalculationEngineTests.cs  # Tests du moteur de calcul
│   └── ExportManagerTests.cs      # Tests des exports
│
├── bin/                           # Fichiers compilés (auto-généré)
│   ├── Debug/
│   └── Release/
│
├── obj/                           # Fichiers intermédiaires (auto-généré)
│
├── CalculatriceMargeWPF.csproj    # Fichier de projet C#
├── CalculatriceMargeWPF.sln       # Solution Visual Studio
├── LICENSE.txt                    # Licence MIT
└── .git/                          # Historique Git
```

## 📂 Description des Dossiers

### `src/` - Code Source
Le cœur de l'application. Suit l'architecture MVVM.

- **App.xaml/cs** : Point d'entrée WPF
- **Views/** : Interfaces utilisateur XAML
- **ViewModels/** : Logique de présentation (futur)
- **Models/** : Logique métier et structures de données
- **MVVM/** : Patterns et base classes pour MVVM
- **Resources/** : Images, icônes et ressources visuelles

### `docs/` - Documentation
Documentation complète du projet, guides et notes de version.

- **guides/** : Guides d'implémentation, installation, architecture
- **releases/** : Notes de version et historique des changements

### `installer/` - Installation Windows
Script Inno Setup pour générer l'installateur Windows (.exe).

### `tests/` - Tests
Tests unitaires du projet (à développer).

### `bin/` et `obj/` - Artefacts Build
Générés automatiquement par dotnet build. À ignorer en git.

---

## 🔧 Chemins Importants

| Resource | Chemin | Utilisation |
|----------|--------|------------|
| **Icône app** | `src/Resources/Images/app_icon.ico` | Icône fenêtre WPF |
| **Logo** | `src/Resources/Images/app_icon.png` | Logo application |
| **Historique** | `%APPDATA%/CalculatriceMarge/historique.txt` | Historique utilisateur |
| **Présets** | `%APPDATA%/CalculatriceMarge/Presets/presets.json` | Presets utilisateur |
| **Installer** | `installer/CalculatriceMargeInstaller.iss` | Script Inno Setup |

---

## 📦 Compilation

```bash
# Build Debug
dotnet build -c Debug

# Build Release
dotnet build -c Release

# Lancer l'app
dotnet run -c Debug

# Générer l'installateur
"C:\Program Files (x86)\Inno Setup 6\ISCC.exe" installer/CalculatriceMargeInstaller.iss
```

---

## 📋 Configuration du Projet (.csproj)

Points clés de configuration:

```xml
<OutputType>WinExe</OutputType>           <!-- Application desktop -->
<TargetFramework>net10.0-windows</TargetFramework> <!-- .NET 10 Windows -->
<UseWPF>true</UseWPF>                     <!-- Utiliser WPF -->
<EnableDefaultCompileItems>false</EnableDefaultCompileItems>
<EnableDefaultPageItems>false</EnableDefaultPageItems>
<ApplicationIcon>src\Resources\Images\app_icon.ico</ApplicationIcon>
```

---

## 🔄 Évolution de la Structure

### v1.0 - Structure Simple
```
CalculatriceMargeWPF/
├── *.xaml/*.cs (root)
├── Models/
├── MVVM/
├── Images/
└── Documentation (root)
```

### v2.0 - Structure Professionnelle (Actuelle)
```
CalculatriceMargeWPF/
├── src/              # Code organisé
├── docs/             # Documentation centralisée
├── installer/        # Installation
├── tests/            # Tests
└── Fichiers root     # Config
```

### v3.0 - Structure Prévue
```
CalculatriceMargeWPF/
├── src/
│   ├── WPF/         # Application WPF desktop
│   ├── MAUI/        # Application MAUI (Android/iOS)
│   └── API/         # API REST (optionnel)
├── tests/
│   ├── Unit/
│   ├── Integration/
│   └── UI/
├── docs/
└── deploy/
```

---

## ✨ Avantages de cette Structure

✅ **Séparation des responsabilités** : Code, docs, config clairs
✅ **Scalabilité** : Facile d'ajouter Views, Models, Tests
✅ **Professionnel** : Suit les conventions .NET standard
✅ **Testabilité** : Dossier tests dédié
✅ **Documentation** : Docs centralisées et organisées
✅ **Maintenance** : Code plus facile à naviguer
✅ **MVVM-ready** : Structure prête pour ViewModels complexes

---

## 🚀 Prochaines Étapes

1. **Tests** : Ajouter tests unitaires dans `tests/`
2. **ViewModels** : Implémenter MVVM complet dans `src/ViewModels/`
3. **Styles** : Ajouter ressources d'apparence dans `src/Resources/Styles/`
4. **MAUI** : Créer version mobile (v3.0)
5. **API** : Ajouter API REST pour intégration (v3.0)

---

## 📝 Notes

- Les chemins dans le .csproj utilisent `\` (Windows style)
- Les chemins XAML utilisent `/` (URI style)
- Les fichiers .xaml.cs doivent avoir `DependentUpon` explicite
- Les images sont des `Resource` (embarquées dans l'exe)

---

**Dernière mise à jour** : 8 Décembre 2025
**Version structure** : 2.0
**Status** : Production
