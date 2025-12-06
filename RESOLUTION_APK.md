# 🔧 Résolution - Génération APK Android

## ❌ Problèmes identifiés

1. **ANDROID_HOME non configuré** - Le SDK Android n'est pas installé/configuré
2. **Erreur NETSDK1136** - Conflit de dépendances Windows dans le projet MAUI
3. **Workloads incomplets** - Les workloads MAUI Android ne sont pas complètement installés

## ✅ Solution recommandée

### Option 1: Installer Android SDK via Visual Studio (RECOMMANDÉ)

1. **Ouvrir Visual Studio Installer**
   ```
   Démarrer → Visual Studio Installer
   ```

2. **Modifier l'installation**
   - Cliquer sur "Modifier" pour Visual Studio 2022
   - Cocher "Développement mobile avec .NET"
   - Vérifier que ces composants sont inclus:
     - SDK Android (API 34 minimum)
     - NDK Android
     - Outils de build Android

3. **Appliquer les modifications** (temps: ~30-45 min)

4. **Vérifier l'installation**
   ```powershell
   $env:ANDROID_HOME
   # Devrait afficher: C:\Program Files (x86)\Android\android-sdk
   ```

### Option 2: Installer Android Command Line Tools

1. **Télécharger Android Command Line Tools**
   - URL: https://developer.android.com/studio#command-line-tools-only
   - Télécharger: `commandlinetools-win-*.zip`

2. **Extraire et configurer**
   ```powershell
   # Créer le répertoire
   New-Item -Path "C:\Android" -ItemType Directory -Force
   
   # Extraire le ZIP dans C:\Android\cmdline-tools\latest
   
   # Configurer ANDROID_HOME
   [System.Environment]::SetEnvironmentVariable('ANDROID_HOME', 'C:\Android', 'User')
   [System.Environment]::SetEnvironmentVariable('ANDROID_SDK_ROOT', 'C:\Android', 'User')
   
   # Ajouter au PATH
   $path = [System.Environment]::GetEnvironmentVariable('Path', 'User')
   [System.Environment]::SetEnvironmentVariable('Path', "$path;C:\Android\cmdline-tools\latest\bin;C:\Android\platform-tools", 'User')
   ```

3. **Installer les packages Android**
   ```powershell
   # Redémarrer PowerShell, puis:
   sdkmanager --install "platform-tools" "platforms;android-34" "build-tools;34.0.0"
   sdkmanager --licenses
   ```

### Option 3: Utiliser un émulateur en ligne (RAPIDE)

Si tu veux juste tester rapidement sans installer tout l'environnement:

1. **Utiliser Appetize.io**
   - URL: https://appetize.io
   - Upload l'APK (une fois généré)
   - Test directement dans le navigateur

---

## 🚀 Après installation Android SDK

Une fois ANDROID_HOME configuré, revenir et compiler:

```powershell
cd c:\Users\c.lecomte\Documents\dev_pyt\CalculatriceMargeMAUI

# Restaurer les workloads
dotnet workload restore

# Compiler l'APK
dotnet publish -f net6.0-android -c Release

# L'APK sera dans:
# bin\Release\net6.0-android\publish\
```

---

## 📱 Fichiers APK générés

Structure attendue:
```
CalculatriceMargeMAUI/
└── bin/Release/net6.0-android/publish/
    ├── com.calculatrice.marge-Signed.apk  (APK signé - installer celui-ci)
    └── com.calculatrice.marge.apk         (APK non signé)
```

---

## 🔍 Vérification rapide

Pour vérifier que tout est OK avant de compiler:

```powershell
# Vérifier ANDROID_HOME
$env:ANDROID_HOME
# Doit afficher le chemin vers le SDK Android

# Vérifier dotnet
dotnet --version
# Doit afficher 10.0.100 ou supérieur

# Vérifier les workloads MAUI
dotnet workload list
# Doit afficher: android, ios, maccatalyst, etc.

# Vérifier Java
java -version
# Doit afficher version 1.8+ (tu as 1.8.0_441 ✓)
```

---

## 💡 Alternative: Compiler depuis Visual Studio

Si tu préfères une interface graphique:

1. Ouvrir `CalculatriceMargeMAUI.sln` dans Visual Studio 2022
2. Sélectionner **Android Emulator** ou **Android Local Device**
3. Clic droit sur le projet → **Publier**
4. Suivre l'assistant de publication
5. L'APK apparaîtra automatiquement dans l'Explorateur

---

## ⚠️ Notes importantes

- **net6.0-android** n'est plus supporté (EOL), mais fonctionne encore
- Pour production, migrer vers **net8.0-android** après installation complète
- L'APK non signé ne peut être installé qu'en mode développeur
- Pour distribution Play Store, signer l'APK avec un keystore

---

**Temps estimé total: 45-60 minutes** (principalement pour télécharger Android SDK ~3GB)
