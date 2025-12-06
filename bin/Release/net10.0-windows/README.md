# Calculatrice de Marge - Guide d'utilisation

## Description
Application de calcul de marge commerciale permettant de déterminer rapidement la rentabilité d'un projet en tenant compte du déboursé sec, des frais généraux, du prix de vente et de la TVA.

## Fonctionnalités principales

### 1. Calcul de marge
- **Déboursé sec** : Coût direct du projet (matériaux, main d'œuvre directe, etc.)
- **Frais généraux** : Coûts indirects (% ou montant fixe en €)
  - Mode % : Pourcentage du déboursé sec
  - Mode € : Montant fixe en euros
- **Prix de vente HT** : Prix de vente hors taxes
- **TVA** : Taux de TVA applicable (par défaut 20%)

### 2. Résultats affichés

#### Récap rapide
- **Prix de revient** : Déboursé sec + Frais généraux
- **Prix HT** : Prix de vente hors taxes
- **Marge nette** : Bénéfice net en euros

#### Résultats détaillés
- **Prix de revient (avec frais)** : Coût total du projet
- **Prix TTC** : Prix toutes taxes comprises
- **Marge brute** : Différence entre prix HT et déboursé sec (en € et %)
- **Marge nette** : Différence entre prix HT et prix de revient (en € et %)

### 3. Presets (Préconfigurations)
Trois configurations rapides disponibles :
- **Standard** : TVA 20%, Frais 25%
- **Réduit** : TVA 5.5%, Frais 8%
- **Service** : TVA 10%, Frais 15%

### 4. Historique
- **Sauvegarde automatique** : Chaque calcul avec un titre unique est automatiquement sauvegardé
- **Double-clic pour recharger** : Cliquez deux fois sur une ligne pour recharger les valeurs
- **Gestion** : Supprimez les entrées individuelles ou nettoyez tout l'historique
- **Export** : Sauvegardez l'historique dans un fichier texte (Menu > Fichier > Sauvegarder l'historique)

### 5. Personnalisation
- **Mode sombre/clair** : Basculez entre les thèmes (Menu > Options > Mode sombre)
- **Titre personnalisé** : Donnez un nom unique à chaque calcul pour le retrouver facilement

## Utilisation pas à pas

1. **Entrez un titre** (optionnel mais recommandé) pour identifier votre calcul
2. **Saisissez le déboursé sec** : Coût direct du projet
3. **Définissez les frais généraux** :
   - Choisissez le mode (% ou €)
   - Entrez la valeur
4. **Indiquez le prix de vente HT** : Prix auquel vous vendez
5. **Vérifiez/Ajustez la TVA** si nécessaire
6. Cliquez sur **"Calculer"** pour voir les résultats
7. Les résultats s'affichent dans "Récap rapide" et "Résultats"

## Raccourcis et astuces

- **Réinitialiser** : Efface tous les champs pour un nouveau calcul
- **Presets** : Utilisez les préconfigurations pour gagner du temps
- **Historique** : Double-cliquez sur une ligne pour recharger un calcul précédent
- **Titres uniques** : L'application empêche les doublons dans l'historique

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
`[Dossier de l'application]/Historique/historique.txt`

## Support

Pour toute question ou suggestion d'amélioration, contactez le développeur.

Version : 1.0
Développé avec WPF (.NET)
