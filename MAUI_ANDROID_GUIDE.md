# Calculatrice de Marge MAUI - Guide de création APK Android

## Vue d'ensemble

Ce guide explique comment créer une version multiplateforme de la Calculatrice de Marge en utilisant **.NET MAUI** pour générer l'APK Android.

## Prérequis

- Visual Studio 2022 avec workload MAUI
- .NET 8.0 ou supérieur
- Android SDK (niveau 21+)
- Java Development Kit (JDK)

## Étapes de création

### 1. Créer le projet MAUI

```bash
dotnet new maui -n CalculatriceMargeMAUI
cd CalculatriceMargeMAUI
```

### 2. Structure du projet

```
CalculatriceMargeMAUI/
├── MainPage.xaml         # Interface utilisateur
├── MainPage.xaml.cs      # Logique de l'interface
├── AppShell.xaml         # Navigation
├── App.xaml              # Configuration app
├── Platforms/            # Code spécifique plateforme
│   ├── Android/
│   ├── Windows/
│   ├── MacCatalyst/
│   └── iOS/
└── Resources/            # Ressources (icônes, images)
```

### 3. Fichiers à créer/modifier

#### **MainPage.xaml** (UI MAUI)
L'interface MAUI est très similaire à WPF mais avec quelques ajustements pour mobile :
- `Entry` au lieu de `TextBox`
- `Picker` au lieu de `ComboBox`
- `Label` pour l'affichage
- `Frame` pour les sections (remplace Border)

#### **MainPage.xaml.cs** (Logique C#)
Reprend 90% du code WPF :
- Même logique de calcul
- Même gestion de l'historique
- Stockage dans `AppDataDirectory` pour permissions mobile

### 4. Points clés de conversion WPF → MAUI

| WPF | MAUI |
|-----|------|
| TextBox | Entry |
| ComboBox | Picker |
| Button | Button (même) |
| Border | Frame |
| Grid | Grid (syntaxe identique) |
| StackPanel | VerticalStackLayout / HorizontalStackLayout |
| Application.Current | Application.Current |
| File I/O | SecureStorage ou AppDataDirectory |

### 5. Générer l'APK Android

#### **Option 1 : Ligne de commande**

```bash
# Build Android Release
dotnet publish -f net8.0-android -c Release

# APK généré à :
# bin/Release/net8.0-android/publish/
```

#### **Option 2 : Visual Studio**
1. Ouvrir le projet dans Visual Studio 2022
2. Sélectionner "Android" en haut
3. Build > Publish pour Android
4. APK généré automatiquement

### 6. Configuration Android

Modifier `Platforms/Android/AndroidManifest.xml` si besoin :

```xml
<manifest ... >
    <application 
        android:label="@string/app_name"
        android:icon="@mipmap/appicon"
        android:allowBackup="true"
        android:theme="@style/AppTheme">
        <!-- Permissions si besoin -->
        <uses-permission android:name="android.permission.WRITE_EXTERNAL_STORAGE" />
    </application>
</manifest>
```

### 7. Avantages MAUI

✅ Code C# partagé (~95%)
✅ Interface adaptable par plateforme
✅ Génère : Android (.apk), iOS (.ipa), Windows, macOS
✅ Performance native
✅ Accès APIs natives via Essentials

### 8. Limitations actuelles

⚠️ MAUI est encore en préversion pour certaines fonctionnalités
⚠️ Quelques APIs manquent comparé à WPF
✅ Mais pour une calculatrice, tout fonctionne parfaitement

## Fichiers de référence

- MainPage.xaml - UI MAUI complète
- MainPage.xaml.cs - Logique métier
- MauiProgram.cs - Configuration app

## Temps d'implémentation

⏱️ ~1-2 jours pour une conversion complète

## Support

- [Docs Microsoft MAUI](https://learn.microsoft.com/dotnet/maui)
- [Community MAUI](https://github.com/dotnet/maui)

---

**Voulez-vous que je :**
1. Crée les fichiers MAUI dans le workspace ?
2. Teste la génération d'APK ?
3. Ajoute des fonctionnalités mobiles (caméra, notifications, etc.) ?
