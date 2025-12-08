- Guide d'utilisation

**Version : 2.0**
Application professionnelle de calcul de marge commerciale permettant de déterminer rapidement la rentabilité d'un projet en tenant compte du déboursé sec, des frais généraux, du prix de vente et de la TVA.

## Fonctionnalités principales

### 1. Calcul de marge
- **Déboursé sec** : Coût direct du projet (matériaux, main d'œuvre directe, etc.)
- **Frais généraux** : Coûts indirects (% ou montant fixe en €)
  - Mode % : Pourcentage du déboursé sec
  - Mode € : Montant fixe en euros
- **Prix de vente HT** : Prix de vente hors taxes
- **TVA** : Taux de TVA applicable (par défaut 20%)

### 2. Calcul inversé (Nouveau !)
- **Calcul à rebours** : Déterminez le déboursé sec maximum à partir d'une marge nette cible
- **Contraintes de marge** : Définissez une marge brute minimale et une marge nette souhaitée
- **Optimisation automatique** : Le système calcule le prix de revient optimal pour atteindre vos objectifs

### 3. Résultats affichés

#### Récap rapide
- **Déboursé sec** : Coût direct du projet
- **Prix HT** : Prix de vente hors taxes
- **Prix TTC** : Prix toutes taxes comprises

#### Résultats détaillés
- **Prix de revient (avec frais)** : Coût total du projet incluant les frais généraux
- **Marge brute** : Différence entre prix HT et déboursé sec (en € et %)
- **Marge nette** : Différence entre prix HT et prix de revient (en € et %)

### 4. Export et statistiques (Nouveau !)
- **Export CSV** : Exportez vos calculs au format CSV pour Excel
- **Rapport HTML** : Générez un rapport détaillé avec graphiques et statistiques
- **Statistiques avancées** : Analyse complète des marges (moyenne, min, max, écart-type)
- **Chiffre d'affaires** : Suivi du CA total et des marges cumulées

### 5. Presets (Préconfigurations)
Trois configurations rapides disponibles :
- **Standard** : TVA 20%, Frais 25%
- **Réduit** : TVA 5.5%, Frais 8%
- **Service** : TVA 10%, Frais 15%

### 6. Historique intelligent
- **Sauvegarde automatique** : Chaque calcul avec un titre unique est automatiquement sauvegardé
- **Double-clic pour recharger** : Double-cliquez sur une ligne pour recharger les valeurs du calcul (Déboursé, Frais, Prix HT, TVA)
- **Suppression** : Supprimez les entrées individuelles (les champs de la calculatrice se réinitialisent automatiquement)
- **Nettoyer tout** : Supprimez l'historique complet en une seule action
- **Export** : Sauvegardez l'historique dans un fichier texte (Menu > Fichier > Sauvegarder l'historique)

### 7. Personnalisation et raccourcis
- **Mode sombre/clair** : Basculez entre les thèmes (Menu > Affichage > Mode sombre)
- **Titre personnalisé** : Donnez un nom unique à chaque calcul pour le retrouver facilement
- **Undo/Redo** : Annulez ou rétablissez vos calculs (Ctrl+Z / Ctrl+Y)
- **Calcul rapide** : Appuyez sur Entrée pour lancer le calcul

## Utilisation pas à pas

### Calcul standard
1. **Entrez un titre** (optionnel mais recommandé) pour identifier votre calcul
2. **Saisissez le déboursé sec** : Coût direct du projet
3. **Définissez les frais généraux** :
   - Choisissez le mode (% ou €)
   - Entrez la valeur
4. **Indiquez le prix de vente HT** : Prix auquel vous vendez
5. **Vérifiez/Ajustez la TVA** si nécessaire
6. Cliquez sur **"Calculer"** pour voir les résultats
7. Les résultats s'affichent dans "Récap rapide" et "Résultats"

### Calcul inversé
1. Laissez le **déboursé sec vide**
2. Entrez le **prix de vente HT** souhaité
3. Configurez les **frais généraux** et la **TVA**
4. Cliquez sur **"Calcul Inversé"**
5. Définissez vos contraintes :
   - Marge brute minimale souhaitée (%)
   - Marge nette cible (%)
6. Le système calcule automatiquement le déboursé sec maximum possible

## Raccourcis et astuces

- **Entrée** : Lance le calcul automatiquement
- **Ctrl+Z / Ctrl+Y** : Annuler / Rétablir les calculs
- **Réinitialiser** : Efface tous les champs pour un nouveau calcul
- **Presets** : Utilisez les préconfigurations pour gagner du temps
- **Historique** : Double-cliquez sur une ligne pour recharger un calcul précédent
- **Export** : Utilisez les boutons d'export pour sauvegarder vos analyses
- **Statistiques** : Consultez les statistiques globales de tous vos calculs

## Format de l'historique

Chaque ligne contient :
- Date et heure
- Titre du calcul
- DS (Déboursé sec)
- FG (Frais généraux)
- PR (Prix de revient)
- HT (Prix hors taxes)
- TTC (Prix TTC)
- TVA (Taux de TVA)
- MB (Marge brute)
- MN (Marge nette)

## Emplacement des fichiers

L'historique est automatiquement sauvegardé dans :
`%AppData%\CalculatriceMarge\Historique\historique.txt`

Les exports sont sauvegardés à l'emplacement de votre choix.

## Support

Pour toute question ou suggestion d'amélioration, contactez le développeur ou consultez le repository GitHub.

## Historique des versions

### Version 2.0 (Actuelle)
- ✨ **Calcul inversé** : Déterminez le déboursé sec à partir d'une marge cible
- ✨ **Export CSV** : Exportez tous vos calculs au format CSV
- ✨ **Rapport HTML** : Générez des rapports professionnels avec statistiques
- ✨ **Statistiques avancées** : Analyse complète (moyenne, min, max, écart-type)
- ✨ **Undo/Redo** : Navigation dans l'historique des calculs (Ctrl+Z/Ctrl+Y)
- ✨ **Séparateurs de milliers** : Formatage professionnel des montants
- ✨ **Interface améliorée** : Nouveau design avec cartes et ombres portées
- 🔧 Refactoring complet de l'architecture (MVVM, Services, Helpers)
- 🔧 Moteur de calcul robuste avec gestion d'erreurs avancée

### Version 1.0.6
- ✓ Amélioration du Récap rapide : Remplacé "Prix de revient" par "Déboursé sec" (plus intuitif)
- ✓ Interface harmonisée avec l'ordre logique des données

### Version 1.0.5
- ✓ Refonte de l'affichage Récap rapide : Prix de revient, Prix HT, Prix TTC
- ✓ Refonte de l'affichage Résultats : Prix de revient, Marge brute, Marge nette (3 colonnes)
- ✓ Amélioration du message "À propos" avec détails complets
- ✓ **Correction critique** : Historique stocké dans AppData au lieu de Program Files (résolution des problèmes de permissions)
- ✓ Gestion d'erreur améliorée avec try-catch dans le calcul

### Version 1.0.2
- ✓ Correction du rechargement des frais généraux depuis l'historique
- ✓ Support rétrocompatible de l'ancien format d'historique (sans champ MODE)
- ✓ Conversion automatique des frais en ancienne format (euros → pourcentage)

### Version 1.0.1
- ✓ Ajout du champ MODE pour stocker le type de frais (% ou EUR) dans l'historique
- ✓ Séparateurs de milliers pour tous les montants
- ✓ Formatage professionnel du Récap rapide et des Résultats
- ✓ Double-clic sur l'historique pour recharger les calculs
- ✓ Sauvegarde automatique avec vérification des doublons

### Version 1.0 (Initiale)
- ✓ Calcul de marges brute et nette
- ✓ Gestion des frais généraux (% ou €)
- ✓ Historique avec sauvegarde automatique
- ✓ Presets de configuration
- ✓ Export des calculs
- ✓ Mode sombre/clair
- ✓ Icône personnalisée

---
© 2025 C. Lecomte - Développé avec WPF (.NET 10)
