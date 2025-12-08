# 📦 Guide Installation V2.0 - Installer Inno Setup

## 🎯 Qu'est-ce que le fichier .iss?

**CalculatriceMargeInstaller.iss** est un script Inno Setup qui crée l'installateur Windows exécutable (.exe).

**Avantages:**
- ✅ Installation professionnelle avec wizard
- ✅ Intégration Windows (Start Menu, Desktop icons)
- ✅ Vérification .NET Runtime
- ✅ Historique utilisateur préservé
- ✅ Déinstallation propre

---

## 🔧 Générer l'Installateur

### Prérequis
1. **Inno Setup 6.0+** - Télécharger depuis: https://www.innosetup.com/
2. **.NET 10.0 Runtime** - Installé sur la machine de build

### Étapes

#### Étape 1: Compiler l'Application
```powershell
cd CalculatriceMargeWPF
dotnet build -c Release -f net10.0-windows
```

**Résultat:** Fichiers compilés dans `bin\Release\net10.0-windows\`

#### Étape 2: Générer l'Installateur
```bash
# Ouvrir Inno Setup
# File → Open → CalculatriceMargeInstaller.iss

# Ou en ligne de commande:
"C:\Program Files (x86)\Inno Setup 6\ISCC.exe" CalculatriceMargeInstaller.iss
```

**Résultat:** Installateur dans `bin\Release\Installer\CalculatriceMargeInstaller_v2.0.0.exe`

---

## 📋 Contenu du Script .iss

### Configuration Générale
```ini
[Setup]
AppId={{9A8E4F9B-7C3D-4E2A-B1F6-3C9D8E7A2B4}}
AppVersion=2.0.0
AppName=Calculatrice de Marge
DefaultDirName={autopf}\Calculatrice de Marge
```

### Fichiers Inclus
```
✓ Application: .exe + .dll
✓ Assets: Images/ Resources/
✓ Documentation: README_V2.md, CHANGELOG_V2.md, LICENSE
```

### Langues Supportées
- 🇫🇷 Français (défaut)
- 🇬🇧 Anglais

### Tâches d'Installation
- ✓ Icône Bureau (optionnel)
- ✓ Icône Quick Launch (optionnel)
- ✓ Lancer au démarrage (optionnel)

### Vérification .NET Runtime
```pascal
// Détection automatique
if IsDotNetRuntimeInstalled() then
  // Lancer l'application
else
  // Proposer téléchargement .NET 10
```

---

## 🚀 Installation par l'Utilisateur

### Vue d'ensemble
```
1. Exécuter: CalculatriceMargeInstaller_v2.0.0.exe
2. Accepter licence (MIT)
3. Choisir répertoire (par défaut: C:\Program Files\Calculatrice de Marge)
4. Choisir options (Desktop icon, Startup, etc.)
5. Installation automatique
6. Application lancée automatiquement
```

### Premier Lancement
```
✓ Historique v1.0.6 automatiquement détecté
✓ Presets chargés depuis AppData
✓ Interface prête à calculer
```

### Chemins de Stockage
```
Installation:  C:\Program Files\Calculatrice de Marge\
Données user:  C:\Users\{user}\AppData\Roaming\CalculatriceMarge\
  ├── Historique\historique.txt
  └── Presets\presets.json
```

---

## 🔄 Mise à Jour de l'Installateur

### Pour modifier le script .iss:

1. **Ouvrir** `CalculatriceMargeInstaller.iss` dans un éditeur texte
2. **Modifier les variables** en haut:
   ```ini
   #define MyAppVersion "2.1.0"      // Nouvelle version
   #define MyAppURL "https://..."     // Nouveau URL
   ```
3. **Ajouter/supprimer des fichiers** dans la section `[Files]`:
   ```ini
   Source: "MonFichier.txt"; DestDir: "{app}"; Flags: ignoreversion
   ```
4. **Générer** avec Inno Setup

### Pour une nouvelle version:

```powershell
# 1. Augmenter version dans .csproj
# 2. Rebuild application
dotnet build -c Release

# 3. Générer installer
"C:\Program Files (x86)\Inno Setup 6\ISCC.exe" CalculatriceMargeInstaller.iss

# 4. Tester installateur
.\bin\Release\Installer\CalculatriceMargeInstaller_v2.0.0.exe

# 5. Committer
git add CalculatriceMargeInstaller.iss CalculatriceMargeWPF.csproj
git commit -m "Release v2.0.0 - Updated installer"
```

---

## ✅ Checklist Avant Déploiement

- [ ] Application compile sans erreurs (Release)
- [ ] Tests passent (dotnet test)
- [ ] Historique v1.0.6 compatible testé
- [ ] Présets chargent correctement
- [ ] Installer exécutable généré sans erreurs
- [ ] Installation sur machine test réussie
- [ ] Application fonctionne post-install
- [ ] Déinstallation propre (AppData préservé)
- [ ] Version métadonnée à jour (.csproj)
- [ ] Release Notes publiées
- [ ] Fichier .iss commité

---

## 🐛 Dépannage

### Problème: "Installer nécessite .NET Runtime"
**Solution:**
```
- Installer .NET 10.0 Runtime depuis https://dotnet.microsoft.com/
- Ou modifier .iss pour inclure une option de téléchargement auto
```

### Problème: "Fichiers not found lors de l'installation"
**Solution:**
```
- Vérifier MySourceDir={#MySourceDir} pointe vers bin\Release\net10.0-windows
- Vérifier tous les fichiers compilés existent
```

### Problème: "Installation dans Program Files impossible"
**Solution:**
```
- Inno Setup propose DefaultDirName={autopf} (auto-détecte Program Files/Program Files (x86))
- Ou autoriser installation locale si permissions refusées
```

---

## 📊 Informations Installation

### Taille Installateur
- Base: ~80 MB (inclus Runtime .NET si nécessaire)
- Post-install: ~200 MB disque utilisé

### Temps Installation
- Typique: 30-60 secondes
- Avec .NET Runtime: 2-3 minutes

### Espace Requis
- Minimum: 500 MB (pour .NET Runtime)
- Recommandé: 1 GB

---

## 🔐 Sécurité Installation

### Certificat de Signature (Optionnel)
Pour signer l'installateur:
```ini
[Setup]
SignTool=signtool /f "C:\cert.pfx" /p password /t http://timestamp.server /d "AppName" $f
```

### Vérification Intégrité
```powershell
# Calculer hash SHA256
Get-FileHash CalculatriceMargeInstaller_v2.0.0.exe -Algorithm SHA256

# Publier le hash sur GitHub Releases pour vérification
```

---

## 📝 Template Nouvelles Versions

Pour créer une version v2.1.0:

```diff
[Setup]
- #define MyAppVersion "2.0.0"
+ #define MyAppVersion "2.1.0"

# Modifier aussi dans:
- OutputBaseFilename=CalculatriceMargeInstaller_v2.0.0
+ OutputBaseFilename=CalculatriceMargeInstaller_v2.1.0

# Et dans .csproj:
- <Version>2.0.0</Version>
+ <Version>2.1.0</Version>
```

---

## 🎯 Résumé

| Élément | Détail |
|--------|--------|
| **Fichier** | CalculatriceMargeInstaller.iss |
| **Outil** | Inno Setup 6.0+ |
| **Sortie** | CalculatriceMargeInstaller_v2.0.0.exe |
| **Taille** | ~80-200 MB |
| **Langues** | Français, Anglais |
| **Compatibilité** | Windows 10+ x64/arm64 |
| **License** | MIT (incluse) |

---

**Documenté:** 8 Décembre 2025
**Version:** 2.0.0
**Status:** ✅ Prêt pour distribution
