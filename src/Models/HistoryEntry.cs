using System;

namespace CalculatriceMargeWPF.Models
{
    /// <summary>
    /// Modèle représentant une entrée d'historique dans la base de données
    /// </summary>
    public class HistoryEntry
    {
        public int Id { get; set; }
        public string Titre { get; set; }
        public double DebourseSec { get; set; }
        public double FraisGeneraux { get; set; }
        public int FraisModeIndex { get; set; } // 0 = %, 1 = EUR
        public double PrixVenteHT { get; set; }
        public double RemisePourcentage { get; set; } // % de remise appliquée
        public double TVA { get; set; }
        public double PrixRevient { get; set; }
        public double MargeBrute { get; set; }
        public double MargeBrutePourcentage { get; set; }
        public double MargeNette { get; set; }
        public double MargeNettePourcentage { get; set; }
        public double PrixVenteTTC { get; set; }
        public DateTime DateCalcul { get; set; }

        public HistoryEntry()
        {
            DateCalcul = DateTime.Now;
            Titre = "Sans titre";
        }

        /// <summary>
        /// Formatte l'entrée pour l'affichage dans la ListBox (compatible avec l'ancien format)
        /// </summary>
        public override string ToString()
        {
            string fraisMode = FraisModeIndex == 0 ? "%" : "EUR";
            return $"{DateCalcul:dd/MM/yyyy HH:mm} | {Titre} | DS: {DebourseSec:F2} € | FG: {FraisGeneraux:F2} {fraisMode} | MODE: {fraisMode} | PR: {PrixRevient:F2} € | HT: {PrixVenteHT:F2} € | TTC: {PrixVenteTTC:F2} € | TVA: {TVA:F2} % | MB: {MargeBrute:F2} € ({MargeBrutePourcentage:F2} %) | MN: {MargeNette:F2} € ({MargeNettePourcentage:F2} %)";
        }
    }
}
