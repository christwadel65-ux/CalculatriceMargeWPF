# Instructions de création de l'APK Android avec MAUI

## 📱 Fichiers de référence MAUI créés

J'ai créé les fichiers de base MAUI dans ce répertoire :

- **MAUI_MainPage.xaml** - Interface utilisateur MAUI (à copier vers MainPage.xaml dans le projet MAUI)
- **MAUI_MainPage.xaml.cs** - Logique C# MAUI (à copier vers MainPage.xaml.cs)
- **MAUI_ANDROID_GUIDE.md** - Guide complet de configuration

## 🚀 Étapes rapides

### 1️⃣ Créer le projet MAUI

```bash
cd c:\Users\c.lecomte\Documents\dev_pyt
dotnet new maui -n CalculatriceMargeMAUI
cd CalculatriceMargeMAUI
```

### 2️⃣ Remplacer les fichiers principaux

```bash
# Copier les fichiers MAUI créés
Copy ..\CalculatriceMargeWPF\MAUI_MainPage.xaml MainPage.xaml
Copy ..\CalculatriceMargeWPF\MAUI_MainPage.xaml.cs MainPage.xaml.cs
```

Modifier le namespace dans les deux fichiers de `CalculatriceMargeMAUI` si nécessaire.

### 3️⃣ Vérifier la configuration Android

Fichier : `CalculatriceMargeMAUI.csproj`

```xml
<Project Sdk="Microsoft.Maui.Sdk">
    <PropertyGroup>
        <TargetFrameworks>net8.0-android;net8.0-windows;net8.0-maccatalyst;net8.0-ios</TargetFrameworks>
        <OutputType>Exe</OutputType>
        <UseMaui>true</UseMaui>
        <SingleProject>true</SingleProject>

        <!-- Android Configuration -->
        <SupportedOSPlatformVersion Condition="$([MSBuild]::GetTargetPlatformIdentifier('$(TargetFramework)')) == 'android'">21.0</SupportedOSPlatformVersion>
        <SupportedOSPlatformVersion Condition="$([MSBuild]::GetTargetPlatformIdentifier('$(TargetFramework)')) == 'windows'">10.0.17763.0</SupportedOSPlatformVersion>
    </PropertyGroup>

    <ItemGroup>
        <!-- NuGet packages required for MAUI -->
        <UseMaui Condition="'$(TargetFramework)' == 'net8.0-android'" Include="Microsoft.Maui.Controls" Version="8.0.*" />
    </ItemGroup>
</Project>
```

### 4️⃣ Générer l'APK

#### **Option A : Ligne de commande**

```bash
cd CalculatriceMargeMAUI

# Build Debug APK
dotnet build -f net8.0-android -c Debug

# Build Release APK (recommandé pour distribution)
dotnet publish -f net8.0-android -c Release

# APK générés à :
# bin/Release/net8.0-android/publish/
```

#### **Option B : Visual Studio 2022**

1. Ouvrir le projet dans Visual Studio
2. Sélectionner "Android" dans la barre d'outils
3. Cliquer sur "Build" → "Publish"
4. L'APK s'ouvre automatiquement dans l'Explorateur de fichiers

### 5️⃣ Installer sur téléphone/émulateur

```bash
# Avec ADB (Android Debug Bridge)
adb install bin/Release/net8.0-android/publish/com.maui.calculator*.apk

# Ou via Visual Studio
# Deploy → Debug sur émulateur
```

## 📦 Fichier APK final

Une fois généré, l'APK se trouve à :

```
CalculatriceMargeMAUI/
└── bin/Release/net8.0-android/publish/
    └── com.maui.CalculatriceMargeMAUI-Signed.apk
```

## ⚙️ Configuration importantes

### AndroidManifest.xml
Ajouter si besoin de permissions :

```xml
<uses-permission android:name="android.permission.INTERNET" />
<uses-permission android:name="android.permission.WRITE_EXTERNAL_STORAGE" />
<uses-permission android:name="android.permission.READ_EXTERNAL_STORAGE" />
```

### Properties/AndroidManifest.xml
```xml
<manifest xmlns:android="http://schemas.android.com/apk/res/android" 
          android:package="com.maui.calculatricemarge"
          android:versionCode="106"
          android:versionName="1.0.6">
    
    <uses-sdk android:minSdkVersion="21" android:targetSdkVersion="33" />
    
    <application android:label="Calculatrice de Marge" 
                 android:icon="@mipmap/appicon">
        <!-- Configuration de l'app -->
    </application>
</manifest>
```

## 🔑 Signature de l'APK (pour Play Store)

```bash
# Générer une clé de signature
keytool -genkey -v -keystore calc_keystore.jks -keyalg RSA -keysize 2048 -validity 10000 -alias calcmarge

# Configurer dans le .csproj ou publier via Visual Studio avec signature
```

## ✅ Points de contrôle

- ✓ Projet MAUI créé
- ✓ MainPage.xaml remplacé
- ✓ Android SDK installé (min API 21)
- ✓ JDK configuré
- ✓ APK généré et testé
- ✓ Icône et permissions configurées

## 📊 Avantages de cette approche

✅ **95% du code C# partagé** entre Windows et Android
✅ **Même logique métier** sans duplication
✅ **UI adaptative** pour tous les appareils
✅ **Performance native** sur Android
✅ **Facile à maintenir** - un seul codebase

## ⚠️ Limitations connues

- MAUI est encore en développement
- Quelques APIs manquent par rapport à WPF
- Pour cette calculatrice, aucun problème !

## 🆘 Troubleshooting

### "Android SDK not found"
→ Installer Android SDK Manager dans Visual Studio

### "APK not generated"
→ Vérifier que net8.0-android est dans TargetFrameworks

### "App crashes on startup"
→ Vérifier les namespaces dans les fichiers .xaml et .xaml.cs

---

**Questions ?** Consultez :
- [Microsoft MAUI Docs](https://learn.microsoft.com/dotnet/maui)
- [MAUI Android Guide](https://learn.microsoft.com/dotnet/maui/android)
- [Troubleshooting MAUI](https://learn.microsoft.com/dotnet/maui/troubleshooting)
