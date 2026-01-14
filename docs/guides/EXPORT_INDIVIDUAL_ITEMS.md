# Export d'éléments individuels de l'historique

## Vue d'ensemble

La nouvelle fonctionnalité d'export permet d'exporter un seul élément de l'historique vers un fichier CSV ou HTML, sans avoir à exporter tout l'historique.

## Accès à la fonctionnalité

1. **Clic droit** sur un élément dans la liste d'historique
2. Menu contextuel avec 3 options :
   - 📋 **Copier dans le presse-papiers**
   - 📄 **Exporter CSV**
   - 📊 **Exporter HTML**

## Fonctionnalités

### 📋 Copier dans le presse-papiers

- Copie le texte complet de l'élément sélectionné
- Format : `DD/MM/YYYY HH:MM:SS | Titre | DS:... | FG:... | ...`
- Utile pour partager rapidement un calcul par email ou chat

**Raccourci** :
1. Clic droit sur l'élément
2. Sélectionner "Copier dans le presse-papiers"
3. Collage (`Ctrl+V`) dans l'application de destination

### 📄 Exporter CSV

Exporte l'élément sélectionné dans un fichier CSV (Excel-compatible).

**Fichier généré** :
```
Nom : marge_{Titre}_{YYYYMMDD_HHmmss}.csv
Emplacement : À définir par l'utilisateur
```

**Contenu du CSV** :
```csv
Titre,Date,DebourseSec,Frais,FraisMode,PrixVenteHT,TVA,PrixRevient,MargeBrute,MargeBrutePercent,MargeNette,MargeNettePercent,PrixVenteTTC
MAO-2026-93-NLS-002 - Ajout fonction...,14/01/2026 14:30:45,13784.27,25%,Pourcentage,13784.27,20,13784.27,0,0,0,0,16541.12
```

**Cas d'usage** :
- Transmettre un calcul spécifique à un collègue
- Archiver les calculs importants dans Excel
- Analyser les données avec des outils externes

### 📊 Exporter HTML

Exporte l'élément sélectionné dans un rapport HTML professionnel.

**Fichier généré** :
```
Nom : marge_{Titre}_{YYYYMMDD_HHmmss}.html
Emplacement : À définir par l'utilisateur
```

**Contenu du rapport** :
- En-tête avec titre et date
- Tableau détaillé des calculs
- Formatage professionnel avec couleurs
- Peut être ouvert dans n'importe quel navigateur

**Cas d'usage** :
- Générer un devis à partager avec le client
- Créer une documentation formelle
- Imprimer un rapport professional

## Fichiers modifiés

### MainWindow.xaml
- Ajout d'un **ContextMenu** à la ListBox d'historique
- 3 items de menu :
  1. Copier dans le presse-papiers
  2. Exporter CSV
  3. Exporter HTML

### MainWindow.xaml.cs
- Ajout de 3 nouvelles méthodes :
  1. `lstHistorique_CopyToClipboard()` - Copie le texte
  2. `lstHistorique_ExportCSV()` - Exporte en CSV
  3. `lstHistorique_ExportHTML()` - Exporte en HTML

## Flux de travail

### Étape 1 : Sélectionner l'élément
```
Clic gauche sur l'élément dans la liste
```

### Étape 2 : Ouvrir le menu contextuel
```
Clic droit sur l'élément sélectionné
```

### Étape 3 : Choisir l'action

#### Option 1 : Copier
```
Menu > Copier dans le presse-papiers
Message de confirmation
Ctrl+V dans l'application de destination
```

#### Option 2 : Exporter CSV
```
Menu > Exporter CSV
Boîte de dialogue "Enregistrer sous"
Sélectionner le dossier et le nom de fichier
Cliquer "Enregistrer"
Message de confirmation avec le chemin
```

#### Option 3 : Exporter HTML
```
Menu > Exporter HTML
Boîte de dialogue "Enregistrer sous"
Sélectionner le dossier et le nom de fichier
Cliquer "Enregistrer"
Message de confirmation avec le chemin
Ouvrir le rapport dans le navigateur
```

## Noms de fichiers

Les fichiers générés ont un format cohérent :
```
marge_{Titre}_{YYYYMMDD_HHmmss}.{csv|html}
```

**Exemple** :
```
marge_MAO-2026-93-NLS-002_20260114_143045.csv
marge_Devis client ABC_20260114_150230.html
```

### Caractères spéciaux

Les caractères spéciaux dans le titre sont conservés et échappés si nécessaire pour respecter les conventions de noms de fichiers.

## Avantages de cette fonctionnalité

✅ **Sélectif** - Exporte seulement l'élément choisi
✅ **Rapide** - Directement depuis le menu contextuel
✅ **Flexible** - CSV pour Excel, HTML pour les rapports
✅ **Pratique** - Copie directe au presse-papiers
✅ **Noms intelligents** - Inclut le titre et la date
✅ **Professionnel** - Rapports HTML formatés

## Limites

- Impossible d'exporter plusieurs éléments à la fois (utiliser l'export global pour cela)
- Le format CSV est simplifié (une ligne par élément)
- Le HTML duplique le format du rapport global mais pour un seul élément

## Cas d'usage réels

### Cas 1 : Partage rapide par email
```
1. Clic droit sur le calcul
2. "Copier dans le presse-papiers"
3. Coller dans l'email
→ Transmission immédiate et simple
```

### Cas 2 : Devis client
```
1. Effectuer le calcul
2. Clic droit > "Exporter HTML"
3. Enregistrer sous "Devis_Client_ABC.html"
4. Envoyer le fichier au client
5. Client ouvre dans navigateur
→ Rapport professionnel et formalisé
```

### Cas 3 : Archivage Excel
```
1. Clic droit > "Exporter CSV"
2. Enregistrer dans le dossier "Calculs_2026"
3. Importer dans une feuille Excel consolidée
→ Historique exploitable dans Excel
```

### Cas 4 : Documentation
```
1. Calculer plusieurs variantes
2. Exporter chaque HTML
3. Créer un dossier "Variantes"
4. Imprimer les rapports en PDF
→ Documentation complète et tracée
```

## Dépannage

### "Veuillez sélectionner un élément"
- Assurez-vous d'avoir cliqué sur un élément avant le clic droit
- La sélection doit être visible (surlignée)

### "Élément introuvable"
- L'élément a peut-être été supprimé
- Rafraîchissez la liste en fermant/ouvrant le gestionnaire de BDD

### Erreur lors de l'export
- Vérifiez les permissions du dossier de destination
- Assurez-vous que le chemin n'est pas trop long
- Vérifiez l'espace disque disponible

---

**Version** : 2.2.0  
**Date** : 14 janvier 2026  
**Module** : MainWindow (Export contextuel)
