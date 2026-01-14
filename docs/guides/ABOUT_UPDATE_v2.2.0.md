# Mise à jour v2.2.0 - "À propos" révisé

## Changements apportés

La fenêtre "À propos" (accessible via `Aide > À propos`) a été mise à jour pour refléter les nouvelles fonctionnalités de la version 2.2.0.

### Ancien contenu
```
Calculatrice de Marge

Version : 2.2.0
Développé avec WPF (.NET 10)

Application professionnelle de calcul de marge commerciale.

Fonctionnalités :
• Calcul automatique des marges brute et nette
• Calcul inversé : déterminer le prix de vente à partir de la marge souhaitée
• Gestion flexible des frais généraux (% ou €)
• Historique intelligent avec rechargement et rétrocompatibilité
• Préconfigurations rapides (Standard, Réduit, Service)
• Récap rapide : Déboursé sec, Prix HT, Prix TTC
• Statistiques détaillées et analyses
• Export CSV et génération de rapports HTML
• Undo/Redo (Ctrl+Z, Ctrl+Y)
• Interface moderne avec thème sombre/clair
• Séparateurs de milliers et formatage professionnel

© 2025 C. Lecomte - Tous droits réservés
```

### Nouveau contenu
```
Calculatrice de Marge

Version : 2.2.0
Développé avec WPF (.NET 10.0)
Base de données : SQLite

Application professionnelle de calcul de marge commerciale.

Fonctionnalités principales :
• Calcul automatique des marges brute et nette
• Calcul inversé : déterminer le prix de vente à partir de la marge souhaitée
• Gestion flexible des frais généraux (% ou €)
• Remises commerciales avec recalcul instantané

Historique & Données :
• Base de données SQLite robuste et performante
• Sauvegarde et restauration de l'historique
• Migration automatique depuis l'ancien format
• Statistiques avancées et recherche par titre
• Gestionnaire intégré (Outils > Gestion base de données)

Préconfigurations & Export :
• Préconfigurations rapides (Standard, Réduit, Service)
• Création de présets personnalisés
• Export CSV et génération de rapports HTML
• Récap rapide : Déboursé sec, Prix HT, Prix TTC

Interface :
• Interface moderne avec thème sombre/clair
• Séparateurs de milliers et formatage professionnel
• Undo/Redo (Ctrl+Z, Ctrl+Y)
• Raccourcis clavier intuitifs
• Support complet du clavier (Entrée pour calculer)

© 2025-2026 C. Lecomte - Tous droits réservés
GitHub : https://github.com/christwadel65-ux/CalculatriceMarge
```

## Changements clés

### ✅ Ajouts
- Mention explicite de **SQLite** comme base de données
- Nouvelle section **"Historique & Données"** avec :
  - Sauvegarde et restauration
  - Migration automatique
  - Gestionnaire intégré
- Nouvelle section **"Préconfigurations & Export"** réorganisée
- **Remises commerciales** mentionnées
- **GitHub URL** incluse
- Mise à jour de l'année du copyright (**2025-2026**)

### 📋 Réorganisation
- Regroupement par catégories logiques :
  1. Fonctionnalités principales (calculs)
  2. Historique & Données (nouvelle)
  3. Préconfigurations & Export
  4. Interface

### 🎯 Bénéfices
- ✅ Information plus claire et structurée
- ✅ Mise en avant du nouveau gestionnaire de BDD
- ✅ Meilleur aperçu des fonctionnalités
- ✅ Lien vers le GitHub
- ✅ Plus professionnel et complet

## Fichier modifié
- **[src/Views/MainWindow.xaml.cs](src/Views/MainWindow.xaml.cs)** - Méthode `MenuAbout_Click()`

## Compilation et test
- ✅ Compilation réussie sans erreurs
- ✅ Application se lance correctement
- ✅ Fenêtre "À propos" accessible via `Aide > À propos`

## Impact
- Aucun impact sur la fonctionnalité de l'application
- Amélioration cosmétique et informationnelle
- Reflète fidèlement les capacités de v2.2.0

---

**Date de mise à jour** : 14 janvier 2026  
**Version** : 2.2.0
