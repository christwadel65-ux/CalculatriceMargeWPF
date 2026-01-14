# Guide de création de l'installeur avec Inno Setup

## Prérequis

1. **Inno Setup 6** installé
   - Télécharger depuis : https://jrsoftware.org/isinfo.php
   - Chemin par défaut : `C:\Program Files (x86)\Inno Setup 6\`

2. **Application compilée** en mode Release Standalone
   ```batch
   build.bat standalone
   ```

## Compilation de l'installeur

### Méthode 1 : Interface graphique

1. Ouvrir Inno Setup Compiler
2. Menu `File` > `Open` 
3. Sélectionner : `installer\CalculatriceMargeWPF.iss`
4. Menu `Build` > `Compile` (ou F9)
5. L'installeur sera créé dans : `installer\Output\`

### Méthode 2 : Ligne de commande

```batch
cd installer
"C:\Program Files (x86)\Inno Setup 6\ISCC.exe" CalculatriceMargeWPF.iss
```

Ou depuis PowerShell :
```powershell
cd installer
& "C:\Program Files (x86)\Inno Setup 6\ISCC.exe" CalculatriceMargeWPF.iss
```

### Méthode 3 : Script automatisé

Créer un fichier `build_installer.bat` :
```batch
@echo off
echo 📦 Compilation de l'installeur...
cd /d "%~dp0"

REM Vérifier qu'Inno Setup est installé
if not exist "C:\Program Files (x86)\Inno Setup 6\ISCC.exe" (
    echo ❌ Inno Setup n'est pas installé
    exit /b 1
)

REM Compiler l'application d'abord
echo 🔨 Compilation de l'application...
call ..\build.bat standalone
if %errorlevel% neq 0 (
    echo ❌ Erreur de compilation
    exit /b 1
)

REM Compiler l'installeur
echo 📦 Création de l'installeur...
"C:\Program Files (x86)\Inno Setup 6\ISCC.exe" CalculatriceMargeWPF.iss

if %errorlevel% equ 0 (
    echo ✅ Installeur créé avec succès !
    echo 📁 Emplacement : installer\Output\
    dir Output\*.exe
) else (
    echo ❌ Erreur lors de la création de l'installeur
    exit /b 1
)
```

## Fichier de sortie

L'installeur sera créé dans :
```
installer\Output\CalculatriceMargeWPF-2.2.0-Setup.exe
```

## Contenu de l'installeur

L'installeur inclut :
- ✅ Exécutable standalone (.NET 10.0 embarqué)
- ✅ Bibliothèques SQLite natives (embarquées dans l'exe)
- ✅ Documentation principale (README.md)
- ✅ Notes de version 2.2.0 (RELEASE_NOTES_V2.2.0.md)
- ✅ Documentation SQLite (SQLITE_IMPLEMENTATION.md)
- ✅ Guides utilisateur (docs/guides/)
- ✅ Licence (LICENSE.txt)

## Configuration de l'installation

### Options par défaut
- **Dossier** : `C:\Program Files\CalculatriceMarge\`
- **Menu Démarrer** : Oui
- **Raccourci Bureau** : Non (optionnel)
- **Architecture** : x64 uniquement
- **Privilèges** : Admin requis

### Fichiers créés
- Application : `C:\Program Files\CalculatriceMarge\CalculatriceMargeWPF.exe`
- Base de données : `%APPDATA%\CalculatriceMarge\Historique\historique.db`

### Raccourcis créés
- Menu Démarrer > CalculatriceMarge
- Menu Démarrer > Documentation
- Menu Démarrer > Nouveautés v2.2.0
- Menu Démarrer > Désinstaller

## Message post-installation

L'utilisateur verra un message l'informant de :
- Version installée (2.2.0)
- Nouvelles fonctionnalités SQLite
- Emplacement de la base de données
- Migration automatique des données

## Vérification

Après compilation, vérifier :
```powershell
# Taille du fichier (environ 80-100 MB)
Get-Item installer\Output\*.exe | Select-Object Name, @{N="Size(MB)";E={[math]::Round($_.Length/1MB, 2)}}

# Version
Get-Item installer\Output\*.exe | Select-Object Name, VersionInfo
```

## Désinstallation

L'installeur crée automatiquement :
- Entrée dans "Programmes et fonctionnalités"
- Désinstalleur : `C:\Program Files\CalculatriceMarge\unins000.exe`

⚠️ **Note** : La base de données SQLite dans `%APPDATA%` n'est PAS supprimée lors de la désinstallation (préservation des données utilisateur).

## Signature de l'installeur (optionnel)

Pour signer l'installeur avec un certificat :
```
signtool sign /f "certificate.pfx" /p "password" /t http://timestamp.digicert.com installer\Output\CalculatriceMargeWPF-2.2.0-Setup.exe
```

## Dépannage

### Erreur : "Cannot open file"
- Vérifier que l'application a été compilée : `build.bat standalone`
- Vérifier le chemin dans le fichier .iss : `#define SourceDir`

### Erreur : "File not found"
- Vérifier que tous les fichiers documentaires existent
- Vérifier les chemins relatifs dans la section `[Files]`

### Installeur trop volumineux
- Normal pour un exécutable standalone avec .NET 10.0 embarqué
- SQLite ajoute environ 2-3 MB supplémentaires

---

**Version du script** : 2.2.0  
**Date** : Janvier 2026  
**Inno Setup** : Version 6.x requise
