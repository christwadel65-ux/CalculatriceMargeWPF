# CalculatriceMarge v2.2.0 - Release Notes

## 🎉 Nouveautés Principales

### Optimisations Performance & Taille
- **Réduction drastique**: 280 MB → 140 MB (-49%) ✨
- **Compilation optimisée**: PublishReadyToRun (AOT) activée
- **Sans fichiers de débogage**: Suppression des .pdb
- **Installation rapide**: Compression maximale (LZMA2)

### Installateur Windows
- **Setup complet**: Inno Setup 6.x
- **Installation simplifiée**: Sans permissions admin requises (option)
- **Multi-langue**: Support 12 langues (EN, FR, DE, ES, IT, JA, KO, PL, PT, RU, TR, ZH)
- **Intégration Windows**: Menu Démarrer, Raccourcis bureau
- **Localisation**: `C:\Program Files (x86)\CalculatriceMarge`

### Distributions
1. **Installateur** (`CalculatriceMargeWPF-2.2.0-Setup.exe`)
   - Taille: ~43 MB
   - Installation automatique avec toutes dépendances
   
2. **Release Standalone** (disponible séparément)
   - Dossier complet portable
   - Aucune installation requise
   - Double-cliquez sur .exe pour lancer

## 📋 Configuration Requise

### Windows
- **OS**: Windows 10/11 (64-bit)
- **Espace disque**: 150 MB (installation)
- **.NET Runtime**: Inclus (auto-contenu v10.0)
- **Permissions**: Standard user (pas d'admin requis par défaut)

## ✨ Caractéristiques Principales

### Calcul des Marges
✅ Marge brute et nette en temps réel  
✅ Calcul inversé (cible de marge)  
✅ Frais généraux configurables  
✅ TVA adaptable (multiples taux prédéfinis)  
✅ Remises applicables  

### Gestion des Données
✅ Historique automatique des 50 derniers calculs  
✅ Présets rapides (Standard, Réduit, Service)  
✅ Export CSV complète  
✅ Statistiques mensuelles/annuelles  

### Interface
✅ Design moderne (WPF/Fluent)  
✅ Résultats en temps réel  
✅ Support clavier complet  
✅ Thème clair/sombre  

## 🐛 Correctifs v2.2.0
- Optimisation taille fichiers
- Amélioration performances au démarrage
- Nettoyage dépendances inutiles
- Mise à jour documentation

## 🔄 Migration depuis v2.0

L'installation de v2.2.0 met à jour automatiquement v2.1.
Vos paramètres et historique sont conservés!

## 📖 Documentation

Consultez [docs/](docs/) pour:
- Guide d'installation complet
- Guide utilisateur détaillé
- FAQ et dépannage
- Historique complet des versions

## 🔗 Ressources

- **GitHub**: https://github.com/christwadel65-ux/CalculatriceMargeWPF
- **Issues**: https://github.com/christwadel65-ux/CalculatriceMargeWPF/issues
- **Licence**: MIT (voir LICENSE.txt)

## 📞 Support

Pour les bugs ou suggestions:
1. Ouvrez une **Issue** sur GitHub
2. Décrivez le problème clairement
3. Incluez les étapes pour reproduire

## 📝 Licence

CalculatriceMarge v2.2.0 est sous licence **MIT**.
Gratuit pour un usage personnel et commercial.

---

**Publié**: 4 Janvier 2026  
**Auteur**: C. Lecomte  
**Plateforme**: Windows 64-bit  
**Runtime**: .NET 10.0 (inclus)
