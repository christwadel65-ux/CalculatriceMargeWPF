using System;
using System.Text.RegularExpressions;

namespace CalculatriceMargeWPF.Helpers
{
    /// <summary>
    /// Parser pour l'historique avec rétrocompatibilité v1 et v2
    /// </summary>
    public class HistoryParser
    {
        /// <summary>
        /// Résultat du parsing d'une ligne d'historique
        /// </summary>
        public class ParsedHistoryEntry
        {
            public string Titre { get; set; }
            public double DebourseSec { get; set; }
            public double FraisGeneraux { get; set; }
            public int FraisModeIndex { get; set; } // 0 = %, 1 = EUR
            public double PrixVente { get; set; }
            public double TVA { get; set; }
        }

        /// <summary>
        /// Parse une ligne d'historique et retourne les valeurs
        /// Gère la rétrocompatibilité entre v1 et v2
        /// </summary>
        public static bool TryParseHistoryLine(string line, out ParsedHistoryEntry entry)
        {
            entry = new ParsedHistoryEntry();
            
            if (string.IsNullOrWhiteSpace(line))
                return false;

            var parts = line.Split('|');
            if (parts.Length < 8)
                return false;

            try
            {
                // Extraire le titre
                entry.Titre = parts[1].Trim();
                
                // Extraire DS (déboursé sec)
                string dsPart = parts[2].Trim().Replace("DS:", "").Trim();
                if (!double.TryParse(dsPart, out double ds))
                    return false;
                entry.DebourseSec = ds;
                
                // Extraire FG (frais généraux)
                string fgPart = parts[3].Trim().Replace("FG:", "").Trim();
                if (!double.TryParse(fgPart, out double fg))
                    return false;
                entry.FraisGeneraux = fg;
                
                // Déterminer le format (v1 ou v2)
                string part4Content = parts[4].Trim();
                
                if (part4Content.StartsWith("MODE:"))
                {
                    // Nouveau format v2 : FG est la valeur brute (%)
                    entry.FraisGeneraux = fg;
                    
                    // Extraire MODE
                    string modePart = part4Content.Replace("MODE:", "").Trim();
                    entry.FraisModeIndex = modePart == "%" ? 0 : 1;
                    
                    // Extraire HT depuis parts[6]
                    if (!ExtractPriceField(parts[6], "HT:", out double ht))
                        return false;
                    entry.PrixVente = ht;
                    
                    // Extraire TVA depuis parts[8]
                    if (!ExtractPercentageField(parts[8], "TVA:", out double tva))
                        return false;
                    entry.TVA = tva;
                }
                else if (part4Content.StartsWith("PR:"))
                {
                    // Ancien format v1 : FG est la valeur en euros, on doit retrouver le %
                    if (ds > 0)
                    {
                        entry.FraisGeneraux = (fg / ds) * 100;
                    }
                    entry.FraisModeIndex = 0; // Mode % par défaut pour ancien format
                    
                    // Extraire HT depuis parts[5]
                    if (!ExtractPriceField(parts[5], "HT:", out double ht))
                        return false;
                    entry.PrixVente = ht;
                    
                    // Extraire TVA depuis parts[7]
                    if (!ExtractPercentageField(parts[7], "TVA:", out double tva))
                        return false;
                    entry.TVA = tva;
                }
                else
                {
                    return false;
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Extrait un champ prix formaté (avec symbole €, etc.)
        /// </summary>
        private static bool ExtractPriceField(string field, string prefix, out double value)
        {
            value = 0;
            string cleaned = field.Trim().Replace(prefix, "").Trim();
            cleaned = Regex.Replace(cleaned, @"[^\d,\.]", "");
            cleaned = cleaned.Replace(",", ".");
            return double.TryParse(cleaned, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out value);
        }

        /// <summary>
        /// Extrait un champ pourcentage
        /// </summary>
        private static bool ExtractPercentageField(string field, string prefix, out double value)
        {
            value = 0;
            string cleaned = field.Trim().Replace(prefix, "").Replace("%", "").Trim();
            return double.TryParse(cleaned, out value);
        }
    }
}
