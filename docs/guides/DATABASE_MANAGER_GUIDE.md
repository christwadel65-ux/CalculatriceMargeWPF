# Gestionnaire de base de données SQLite

## Vue d'ensemble

La nouvelle fenêtre de gestion de la base de données SQLite permet de gérer, sauvegarder, restaurer et maintenir la base de données d'historique de manière intuitive.

## Accès

**Menu** : `Outils` > `🗄️ Gestion base de données`

## Fonctionnalités

### 📊 Onglet Informations

Affiche les informations générales et statistiques de la base de données.

#### Informations générales
- **Emplacement** : Chemin complet du fichier historique.db
- **Taille du fichier** : Taille actuelle de la base
- **Dernière modification** : Date et heure de la dernière mise à jour
- **Nombre d'entrées** : Total de calculs enregistrés
- **Version SQLite** : Version du moteur SQLite utilisé

#### Statistiques
- **Cumul marges brutes** : Somme totale des marges brutes
- **Cumul marges nettes** : Somme totale des marges nettes
- **Marge brute moyenne** : Pourcentage moyen de marge brute
- **Marge nette moyenne** : Pourcentage moyen de marge nette

#### Actions
- 🔄 **Actualiser** : Met à jour toutes les informations affichées

---

### 💾 Onglet Sauvegarde

Permet de créer des copies de sauvegarde de la base de données.

#### Sauvegarde manuelle
- Crée une copie horodatée de la base de données
- Format : `historique_backup_YYYYMMDD_HHmmss.db`
- Emplacement : `%APPDATA%\CalculatriceMarge\Historique\Backups\`
- Option d'ouvrir le dossier après création

#### Sauvegardes automatiques (en développement)
- Configuration de la fréquence (quotidienne, hebdomadaire, mensuelle)
- Nombre de sauvegardes à conserver
- Rotation automatique des anciennes sauvegardes

---

### 📥 Onglet Restauration

Permet de restaurer la base depuis une sauvegarde ou importer des données.

#### Restaurer depuis une sauvegarde
1. Sélectionnez le fichier de sauvegarde (*.db)
2. Une copie de sécurité de la base actuelle est créée automatiquement
3. La base est restaurée
4. **Important** : Redémarrez l'application pour appliquer les changements

**⚠️ Attention** : La restauration remplace complètement la base actuelle.

#### Importer depuis historique.txt
- Permet d'importer un ancien fichier texte `historique.txt`
- Les données sont ajoutées à la base existante (pas de remplacement)
- Idéal pour fusionner plusieurs historiques

---

### 🔧 Onglet Maintenance

Outils de maintenance et d'optimisation de la base de données.

#### Optimiser la base (VACUUM)
- Compacte la base de données
- Récupère l'espace disque inutilisé
- Défragmente les données
- Affiche l'espace récupéré après l'opération

**Quand l'utiliser** : Après avoir supprimé beaucoup d'entrées

#### Vérifier l'intégrité
- Exécute `PRAGMA integrity_check`
- Détecte les corruptions éventuelles
- Recommande une restauration si problème détecté

**Quand l'utiliser** : En cas de comportement anormal de l'application

#### Analyser la base
- Met à jour les statistiques internes SQLite
- Améliore les performances des requêtes
- Optimise les plans d'exécution

**Quand l'utiliser** : Après import massif de données ou périodiquement

#### Zone dangereuse : Vider l'historique
- Supprime **TOUTES** les entrées de l'historique
- ⚠️ **Action irréversible**
- Double confirmation requise
- Une sauvegarde automatique est créée avant la suppression
- Format backup : `historique_avant_suppression_YYYYMMDD_HHmmss.db`

---

## Cas d'usage

### Scénario 1 : Sauvegarde régulière
**Objectif** : Protéger vos données

1. Menu `Outils` > `Gestion base de données`
2. Onglet `💾 Sauvegarde`
3. Cliquer sur `Créer une sauvegarde`
4. Conserver le fichier sur un support externe ou cloud

**Fréquence recommandée** : Hebdomadaire ou avant mise à jour importante

### Scénario 2 : Restauration après problème
**Objectif** : Récupérer des données perdues

1. Menu `Outils` > `Gestion base de données`
2. Onglet `📥 Restauration`
3. Cliquer sur `Restaurer depuis un fichier`
4. Sélectionner la sauvegarde à restaurer
5. Redémarrer l'application

### Scénario 3 : Fusion d'historiques
**Objectif** : Combiner plusieurs historiques

1. Menu `Outils` > `Gestion base de données`
2. Onglet `📥 Restauration`
3. Cliquer sur `Importer historique.txt`
4. Sélectionner le fichier texte à importer
5. Les données sont ajoutées sans écraser l'existant

### Scénario 4 : Base de données lente
**Objectif** : Améliorer les performances

1. Menu `Outils` > `Gestion base de données`
2. Onglet `🔧 Maintenance`
3. Cliquer sur `Optimiser la base (VACUUM)`
4. Puis `Analyser la base`

### Scénario 5 : Repartir à zéro
**Objectif** : Vider complètement l'historique

1. Menu `Outils` > `Gestion base de données`
2. Onglet `💾 Sauvegarde` (créer une backup par précaution)
3. Onglet `🔧 Maintenance`
4. Cliquer sur `Vider complètement l'historique`
5. Confirmer deux fois

---

## Emplacements des fichiers

### Base de données principale
```
%APPDATA%\CalculatriceMarge\Historique\historique.db
```

### Dossier de sauvegardes
```
%APPDATA%\CalculatriceMarge\Historique\Backups\
```

### Exemples de chemins Windows
```
C:\Users\VotreNom\AppData\Roaming\CalculatriceMarge\Historique\
├── historique.db (base principale)
├── historique.txt.backup (ancien format si migration effectuée)
└── Backups\
    ├── historique_backup_20260114_153045.db
    ├── historique_backup_20260107_120030.db
    └── historique_avant_suppression_20260101_094512.db
```

---

## Bonnes pratiques

### Sauvegardes
- ✅ Créer une sauvegarde avant toute opération importante
- ✅ Conserver plusieurs sauvegardes (rotation)
- ✅ Stocker les sauvegardes sur un support externe
- ✅ Tester régulièrement la restauration

### Maintenance
- ✅ Optimiser la base mensuellement
- ✅ Vérifier l'intégrité en cas de doute
- ✅ Analyser après import massif

### Sécurité
- ⚠️ Ne jamais modifier directement le fichier .db avec un éditeur externe
- ⚠️ Toujours utiliser les outils intégrés de l'application
- ⚠️ Vérifier l'intégrité après une coupure électrique

---

## Dépannage

### "Erreur lors du chargement des informations"
**Cause** : Base corrompue ou inaccessible  
**Solution** : 
1. Vérifier l'intégrité
2. Si corruption détectée, restaurer une sauvegarde

### "Fichier en cours d'utilisation"
**Cause** : Plusieurs instances de l'application ouvertes  
**Solution** : Fermer toutes les instances sauf une

### "Espace disque insuffisant"
**Cause** : Disque plein  
**Solution** : 
1. Libérer de l'espace
2. Optimiser la base (VACUUM)

### Perte de données après restauration
**Cause** : Sauvegarde obsolète restaurée  
**Solution** : 
1. Vérifier la date de la sauvegarde avant restauration
2. L'ancienne base est sauvegardée dans `historique_avant_restauration_*.db`

---

## Limitations

- Les sauvegardes automatiques sont actuellement en développement
- La fusion d'historiques ne détecte pas les doublons
- La restauration nécessite un redémarrage de l'application

---

## Développements futurs

- ☐ Sauvegardes automatiques planifiées
- ☐ Synchronisation cloud (Google Drive, Dropbox)
- ☐ Export sélectif (par période, par critères)
- ☐ Compression des sauvegardes
- ☐ Chiffrement de la base de données
- ☐ Historique des opérations de maintenance

---

**Version** : 2.2.0  
**Date** : Janvier 2026  
**Module** : DatabaseManagerDialog
