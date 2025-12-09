using System;
using System.Collections.Generic;
using System.Globalization;

namespace CalculatriceMargeWPF.Helpers
{
    public class LocalizationManager
    {
        private static LocalizationManager _instance;
        private Dictionary<string, Dictionary<string, string>> _translations;
        private string _currentLanguage = "fr";

        public static LocalizationManager Instance => _instance ??= new LocalizationManager();

        public event Action OnLanguageChanged;

        public LocalizationManager()
        {
            InitializeTranslations();
        }

        private void InitializeTranslations()
        {
            _translations = new Dictionary<string, Dictionary<string, string>>
            {
                { "fr", new Dictionary<string, string>
                    {
                        { "menu.fichier", "Fichier" },
                        { "menu.historique", "Historique" },
                        { "menu.affichage", "Affichage" },
                        { "menu.aide", "Aide" },
                        { "menu.sauvegarder", "Sauvegarder l'historique" },
                        { "menu.quitter", "Quitter" },
                        { "menu.afficher", "Afficher/Masquer" },
                        { "menu.mode_sombre", "Mode sombre" },
                        { "menu.langue", "Langue" },
                        { "menu.guide", "Guide d'utilisation" },
                        { "menu.implementation", "Résumé d'implémentation" },
                        { "menu.apropos", "À propos" },
                        { "btn.calculer", "Calculer" },
                        { "btn.calcul_inverse", "Calcul Inversé" },
                        { "btn.reinitialiser", "Réinitialiser" },
                        { "btn.statistiques", "📊 Statistiques" },
                        { "btn.export_csv", "📥 Export CSV" },
                        { "btn.export_html", "📄 Rapport HTML" },
                        { "btn.appliquer", "Appliquer" },
                        { "btn.nettoyer", "Nettoyer l'historique" },
                        { "btn.supprimer", "Supprimer la sélection" },
                        { "label.titre", "Titre du calcul:" },
                        { "label.debourse", "Déboursé sec (€):" },
                        { "label.revient", "Prix de revient (€):" },
                        { "label.vente", "Prix de vente HT (€):" },
                        { "label.frais", "Frais generaux:" },
                        { "label.tva", "TVA (%):" },
                        { "label.entrees", "Entrées" },
                        { "label.presets", "Presets" },
                        { "label.recap", "Récap rapide" },
                        { "label.resultats", "Résultats" },
                        { "label.historique", "Historique (double-clic pour recharger)" },
                    }
                },
                { "en", new Dictionary<string, string>
                    {
                        { "menu.fichier", "File" },
                        { "menu.historique", "History" },
                        { "menu.affichage", "View" },
                        { "menu.aide", "Help" },
                        { "menu.sauvegarder", "Save History" },
                        { "menu.quitter", "Exit" },
                        { "menu.afficher", "Show/Hide" },
                        { "menu.mode_sombre", "Dark Mode" },
                        { "menu.langue", "Language" },
                        { "menu.guide", "User Guide" },
                        { "menu.implementation", "Implementation Summary" },
                        { "menu.apropos", "About" },
                        { "btn.calculer", "Calculate" },
                        { "btn.calcul_inverse", "Reverse Calculation" },
                        { "btn.reinitialiser", "Reset" },
                        { "btn.statistiques", "📊 Statistics" },
                        { "btn.export_csv", "📥 Export CSV" },
                        { "btn.export_html", "📄 HTML Report" },
                        { "btn.appliquer", "Apply" },
                        { "btn.nettoyer", "Clear History" },
                        { "btn.supprimer", "Delete Selection" },
                        { "label.titre", "Calculation title:" },
                        { "label.debourse", "Cost of materials (€):" },
                        { "label.revient", "Cost price (€):" },
                        { "label.vente", "Selling price HT (€):" },
                        { "label.frais", "Overhead:" },
                        { "label.tva", "VAT (%):" },
                        { "label.entrees", "Inputs" },
                        { "label.presets", "Presets" },
                        { "label.recap", "Quick summary" },
                        { "label.resultats", "Results" },
                        { "label.historique", "History (double-click to reload)" },
                    }
                },
                { "es", new Dictionary<string, string>
                    {
                        { "menu.fichier", "Archivo" },
                        { "menu.historique", "Historial" },
                        { "menu.affichage", "Vista" },
                        { "menu.aide", "Ayuda" },
                        { "menu.sauvegarder", "Guardar Historial" },
                        { "menu.quitter", "Salir" },
                        { "menu.afficher", "Mostrar/Ocultar" },
                        { "menu.mode_sombre", "Modo Oscuro" },
                        { "menu.langue", "Idioma" },
                        { "menu.guide", "Guía de Uso" },
                        { "menu.implementation", "Resumen de Implementación" },
                        { "menu.apropos", "Acerca de" },
                        { "btn.calculer", "Calcular" },
                        { "btn.calcul_inverse", "Cálculo Inverso" },
                        { "btn.reinitialiser", "Reiniciar" },
                        { "btn.statistiques", "📊 Estadísticas" },
                        { "btn.export_csv", "📥 Exportar CSV" },
                        { "btn.export_html", "📄 Informe HTML" },
                        { "btn.appliquer", "Aplicar" },
                        { "btn.nettoyer", "Limpiar Historial" },
                        { "btn.supprimer", "Eliminar Selección" },
                        { "label.titre", "Título del cálculo:" },
                        { "label.debourse", "Costo de materiales (€):" },
                        { "label.revient", "Precio de costo (€):" },
                        { "label.vente", "Precio de venta IVA excluido (€):" },
                        { "label.frais", "Gastos generales:" },
                        { "label.tva", "IVA (%):" },
                        { "label.entrees", "Entradas" },
                        { "label.presets", "Presets" },
                        { "label.recap", "Resumen rápido" },
                        { "label.resultats", "Resultados" },
                        { "label.historique", "Historial (doble clic para recargar)" },
                    }
                },
                { "de", new Dictionary<string, string>
                    {
                        { "menu.fichier", "Datei" },
                        { "menu.historique", "Verlauf" },
                        { "menu.affichage", "Ansicht" },
                        { "menu.aide", "Hilfe" },
                        { "menu.sauvegarder", "Verlauf speichern" },
                        { "menu.quitter", "Beenden" },
                        { "menu.afficher", "Anzeigen/Verbergen" },
                        { "menu.mode_sombre", "Dunkler Modus" },
                        { "menu.langue", "Sprache" },
                        { "menu.guide", "Benutzerhandbuch" },
                        { "menu.implementation", "Implementierungszusammenfassung" },
                        { "menu.apropos", "Über" },
                        { "btn.calculer", "Berechnen" },
                        { "btn.calcul_inverse", "Rückwärtsberechnung" },
                        { "btn.reinitialiser", "Zurücksetzen" },
                        { "btn.statistiques", "📊 Statistiken" },
                        { "btn.export_csv", "📥 CSV exportieren" },
                        { "btn.export_html", "📄 HTML-Bericht" },
                        { "btn.appliquer", "Anwenden" },
                        { "btn.nettoyer", "Verlauf löschen" },
                        { "btn.supprimer", "Auswahl löschen" },
                        { "label.titre", "Berechnungstitel:" },
                        { "label.debourse", "Materialkosten (€):" },
                        { "label.revient", "Selbstkosten (€):" },
                        { "label.vente", "Verkaufspreis netto (€):" },
                        { "label.frais", "Gemeinkosten:" },
                        { "label.tva", "MwSt. (%):" },
                        { "label.entrees", "Eingaben" },
                        { "label.presets", "Presets" },
                        { "label.recap", "Schnellanzeige" },
                        { "label.resultats", "Ergebnisse" },
                        { "label.historique", "Verlauf (Doppelklick zum Neu laden)" },
                    }
                }
            };
        }

        public string GetString(string key)
        {
            if (_translations.ContainsKey(_currentLanguage) && 
                _translations[_currentLanguage].ContainsKey(key))
            {
                return _translations[_currentLanguage][key];
            }

            // Fallback to French
            if (_translations["fr"].ContainsKey(key))
            {
                return _translations["fr"][key];
            }

            return key;
        }

        public void SetLanguage(string languageCode)
        {
            if (_translations.ContainsKey(languageCode))
            {
                _currentLanguage = languageCode;
                OnLanguageChanged?.Invoke();
            }
        }

        public string CurrentLanguage => _currentLanguage;

        public List<string> AvailableLanguages => new List<string> { "fr", "en", "es", "de" };
    }
}
