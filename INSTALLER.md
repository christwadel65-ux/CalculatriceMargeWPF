# Instructions pour créer l'installateur

## Prérequis

1. **Inno Setup** : Téléchargez et installez Inno Setup depuis https://jrsoftware.org/isdl.php
   - Version recommandée : Inno Setup 6.x ou supérieur

## Étapes pour générer l'installateur

### 1. Préparer l'application
Assurez-vous que l'application est publiée :
```powershell
dotnet publish CalculatriceMargeWPF.csproj -c Release -o bin/Release/publish
```

### 2. Compiler le script Inno Setup

#### Option A : Via l'interface graphique
1. Ouvrez Inno Setup Compiler
2. Ouvrez le fichier `setup.iss`
3. Cliquez sur "Build" > "Compile" (ou appuyez sur F9)
4. L'installateur sera créé dans `bin\Release\Installer\`

#### Option B : En ligne de commande
```powershell
"C:\Program Files (x86)\Inno Setup 6\ISCC.exe" setup.iss
```

### 3. Résultat
Un fichier `CalculatriceMarge_Setup_v1.0.exe` sera créé dans le dossier `bin\Release\Installer\`

## Personnalisation du script

Modifiez les valeurs dans `setup.iss` :

```pascal
#define MyAppName "Calculatrice de Marge"        // Nom de l'application
#define MyAppVersion "1.0"                        // Numéro de version
#define MyAppPublisher "Votre Nom"                // Nom du développeur
```

## Fonctionnalités de l'installateur

- ✓ Installation dans Program Files
- ✓ Création de raccourcis dans le menu Démarrer
- ✓ Option pour créer un raccourci sur le bureau
- ✓ Vérification de la présence de .NET Runtime
- ✓ Désinstallation propre
- ✓ Inclusion du README et de tous les fichiers nécessaires
- ✓ Icône personnalisée

## Distribution

Une fois l'installateur créé, vous pouvez distribuer le fichier `.exe` généré.
Les utilisateurs n'auront qu'à l'exécuter pour installer l'application.

## Mise à jour de version

Pour créer une nouvelle version :
1. Modifiez `#define MyAppVersion` dans `setup.iss`
2. Recompilez le script
3. L'installateur pourra mettre à jour l'installation existante
