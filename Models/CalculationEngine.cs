using System;

namespace CalculatriceMargeWPF.Models
{
    /// <summary>
    /// Moteur de calcul pour les marges commerciales
    /// </summary>
    public class CalculationEngine
    {
        /// <summary>
        /// Résultat d'un calcul de marge
        /// </summary>
        public class CalculationResult
        {
            public double DebourseSec { get; set; }
            public double FraisGeneraux { get; set; }
            public bool FraisEnPourcent { get; set; }
            public double FraisEnEuro { get; set; }
            public double PrixRevientTotal { get; set; }
            public double PrixVenteHT { get; set; }
            public double TVAPct { get; set; }
            public double PrixVenteTTC { get; set; }
            public double MargeBruteEuro { get; set; }
            public double MargeBrutePct { get; set; }
            public double MargeNetteEuro { get; set; }
            public double MargeNettePct { get; set; }
            public DateTime CalculDateTime { get; set; }
            public string Titre { get; set; }

            public CalculationResult()
            {
                CalculDateTime = DateTime.Now;
                Titre = "Sans titre";
            }

            public override string ToString()
            {
                return $"{CalculDateTime:dd/MM/yyyy HH:mm:ss} | {Titre} | DS:{DebourseSec:N2} | FG:{FraisGeneraux:N2} | MODE:{(FraisEnPourcent ? "%" : "EUR")} | PR:{PrixRevientTotal:C} | HT:{PrixVenteHT:C} | TTC:{PrixVenteTTC:C} | TVA:{TVAPct:F2}% | MB:{MargeBruteEuro:C} ({MargeBrutePct:F2}%) | MN:{MargeNetteEuro:C} ({MargeNettePct:F2}%)";
            }
        }

        /// <summary>
        /// Calcule les marges à partir des saisies
        /// </summary>
        public CalculationResult Calculate(double debourseSec, double prixVenteHT, double tva, double fraisGeneraux, bool fraisEnPourcent)
        {
            ValidateInputs(debourseSec, prixVenteHT, tva, fraisGeneraux);

            var result = new CalculationResult
            {
                DebourseSec = debourseSec,
                PrixVenteHT = prixVenteHT,
                TVAPct = tva,
                FraisGeneraux = fraisGeneraux,
                FraisEnPourcent = fraisEnPourcent
            };

            // Calcul des frais en euros
            result.FraisEnEuro = fraisEnPourcent ? debourseSec * (fraisGeneraux / 100) : fraisGeneraux;
            result.PrixRevientTotal = debourseSec + result.FraisEnEuro;

            // Prix TTC
            result.PrixVenteTTC = prixVenteHT * (1 + tva / 100);

            // Marges
            result.MargeBruteEuro = prixVenteHT - debourseSec;
            result.MargeNetteEuro = prixVenteHT - result.PrixRevientTotal;

            result.MargeBrutePct = prixVenteHT == 0 ? 0 : (result.MargeBruteEuro / prixVenteHT) * 100;
            result.MargeNettePct = prixVenteHT == 0 ? 0 : (result.MargeNetteEuro / prixVenteHT) * 100;

            return result;
        }

        /// <summary>
        /// Calcul inversé: connaissant le prix de vente souhaité et une marge nette cible,
        /// calcule le déboursé sec maximum acceptable
        /// </summary>
        public CalculationResult CalculateInverse(double prixVenteSouhaite, double margeNetteTargetPct, double tva, bool fraisEnPourcent, double fraisGeneraux = 10)
        {
            ValidateInputs(0, prixVenteSouhaite, tva, fraisGeneraux);

            if (margeNetteTargetPct < 0 || margeNetteTargetPct > 100)
                throw new ArgumentException("La marge nette cible doit être entre 0 et 100%");

            // Calcul inversé:
            // MN% = (PV - PR) / PV
            // PR = PV * (1 - MN%)
            double prixRevientMax = prixVenteSouhaite * (1 - margeNetteTargetPct / 100);

            // Déboursé = PR - Frais
            double fraisEnEuro;
            double debourseSec;

            if (fraisEnPourcent)
            {
                // DS * (1 + FG%) = PR
                debourseSec = prixRevientMax / (1 + fraisGeneraux / 100);
            }
            else
            {
                // DS + FG = PR
                debourseSec = prixRevientMax - fraisGeneraux;
            }

            // Recalculer pour confirmer
            return Calculate(debourseSec, prixVenteSouhaite, tva, fraisGeneraux, fraisEnPourcent);
        }

        /// <summary>
        /// Valide les entrées utilisateur
        /// </summary>
        private void ValidateInputs(double debourseSec, double prixVenteHT, double tva, double fraisGeneraux)
        {
            if (debourseSec < 0)
                throw new ArgumentException("Le déboursé sec ne peut pas être négatif");
            
            if (prixVenteHT < 0)
                throw new ArgumentException("Le prix de vente HT ne peut pas être négatif");
            
            if (tva < 0 || tva > 100)
                throw new ArgumentException("La TVA doit être entre 0 et 100%");
            
            if (fraisGeneraux < 0)
                throw new ArgumentException("Les frais généraux ne peuvent pas être négatifs");
        }

        /// <summary>
        /// Calcule des statistiques sur une liste de résultats
        /// </summary>
        public class Statistics
        {
            public double MoyenneMargeBrutePct { get; set; }
            public double MoyenneMargeNettePct { get; set; }
            public double MinMargeBrutePct { get; set; }
            public double MaxMargeBrutePct { get; set; }
            public double MinMargeNettePct { get; set; }
            public double MaxMargeNettePct { get; set; }
            public double EcartTypeMargeBrutePct { get; set; }
            public double EcartTypeMargeNettePct { get; set; }
            public int NombreCalculs { get; set; }
            public double ChiffreAffairesTotal { get; set; }
            public double MargeNetteEuroTotal { get; set; }
        }

        public Statistics ComputeStatistics(CalculationResult[] results)
        {
            if (results == null || results.Length == 0)
                throw new ArgumentException("Aucun résultat à analyser");

            var stats = new Statistics
            {
                NombreCalculs = results.Length,
                ChiffreAffairesTotal = 0,
                MargeNetteEuroTotal = 0
            };

            // Sommes
            double sumMargeBrutePct = 0, sumMargeNettePct = 0;
            double sumSqMargeBrutePct = 0, sumSqMargeNettePct = 0;
            
            stats.MinMargeBrutePct = results[0].MargeBrutePct;
            stats.MaxMargeBrutePct = results[0].MargeBrutePct;
            stats.MinMargeNettePct = results[0].MargeNettePct;
            stats.MaxMargeNettePct = results[0].MargeNettePct;

            foreach (var result in results)
            {
                sumMargeBrutePct += result.MargeBrutePct;
                sumMargeNettePct += result.MargeNettePct;
                sumSqMargeBrutePct += result.MargeBrutePct * result.MargeBrutePct;
                sumSqMargeNettePct += result.MargeNettePct * result.MargeNettePct;

                stats.MinMargeBrutePct = Math.Min(stats.MinMargeBrutePct, result.MargeBrutePct);
                stats.MaxMargeBrutePct = Math.Max(stats.MaxMargeBrutePct, result.MargeBrutePct);
                stats.MinMargeNettePct = Math.Min(stats.MinMargeNettePct, result.MargeNettePct);
                stats.MaxMargeNettePct = Math.Max(stats.MaxMargeNettePct, result.MargeNettePct);

                stats.ChiffreAffairesTotal += result.PrixVenteTTC;
                stats.MargeNetteEuroTotal += result.MargeNetteEuro;
            }

            // Moyennes
            stats.MoyenneMargeBrutePct = sumMargeBrutePct / results.Length;
            stats.MoyenneMargeNettePct = sumMargeNettePct / results.Length;

            // Écart-type
            stats.EcartTypeMargeBrutePct = Math.Sqrt((sumSqMargeBrutePct / results.Length) - (stats.MoyenneMargeBrutePct * stats.MoyenneMargeBrutePct));
            stats.EcartTypeMargeNettePct = Math.Sqrt((sumSqMargeNettePct / results.Length) - (stats.MoyenneMargeNettePct * stats.MoyenneMargeNettePct));

            return stats;
        }
    }
}
