using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace CalculatriceMargeWPF.Models
{
    /// <summary>
    /// Gestionnaire d'export des données
    /// </summary>
    public class ExportManager
    {
        /// <summary>
        /// Exporte l'historique en CSV
        /// </summary>
        public static void ExportToCSV(IEnumerable<CalculationEngine.CalculationResult> results, string filePath)
        {
            try
            {
                var csv = new StringBuilder();
                
                // En-têtes
                csv.AppendLine("Date,Titre,Déboursé sec,Frais généraux,Mode frais,Prix de revient,Prix HT,Prix TTC,TVA %,Marge brute €,Marge brute %,Marge nette €,Marge nette %");

                // Données
                foreach (var result in results)
                {
                    csv.AppendLine($"\"{result.CalculDateTime:dd/MM/yyyy HH:mm:ss}\"," +
                        $"\"{result.Titre}\"," +
                        $"{result.DebourseSec:F2}," +
                        $"{result.FraisGeneraux:F2}," +
                        $"\"{(result.FraisEnPourcent ? "%" : "EUR")}\"," +
                        $"{result.PrixRevientTotal:F2}," +
                        $"{result.PrixVenteHT:F2}," +
                        $"{result.PrixVenteTTC:F2}," +
                        $"{result.TVAPct:F2}," +
                        $"{result.MargeBruteEuro:F2}," +
                        $"{result.MargeBrutePct:F2}," +
                        $"{result.MargeNetteEuro:F2}," +
                        $"{result.MargeNettePct:F2}");
                }

                File.WriteAllText(filePath, csv.ToString(), Encoding.UTF8);
            }
            catch (Exception ex)
            {
                throw new IOException($"Erreur lors de l'export CSV: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Exporte les statistiques en CSV
        /// </summary>
        public static void ExportStatistics(CalculationEngine.Statistics stats, IEnumerable<CalculationEngine.CalculationResult> results, string filePath)
        {
            try
            {
                var csv = new StringBuilder();

                csv.AppendLine("STATISTIQUES GÉNÉRALES");
                csv.AppendLine($"Nombre de calculs,{stats.NombreCalculs}");
                csv.AppendLine($"Chiffre d'affaires total (TTC),{stats.ChiffreAffairesTotal:F2}");
                csv.AppendLine($"Marge nette totale,{stats.MargeNetteEuroTotal:F2}");
                csv.AppendLine();

                csv.AppendLine("MARGE BRUTE (%)");
                csv.AppendLine($"Moyenne,{stats.MoyenneMargeBrutePct:F2}");
                csv.AppendLine($"Minimum,{stats.MinMargeBrutePct:F2}");
                csv.AppendLine($"Maximum,{stats.MaxMargeBrutePct:F2}");
                csv.AppendLine($"Écart-type,{stats.EcartTypeMargeBrutePct:F2}");
                csv.AppendLine();

                csv.AppendLine("MARGE NETTE (%)");
                csv.AppendLine($"Moyenne,{stats.MoyenneMargeNettePct:F2}");
                csv.AppendLine($"Minimum,{stats.MinMargeNettePct:F2}");
                csv.AppendLine($"Maximum,{stats.MaxMargeNettePct:F2}");
                csv.AppendLine($"Écart-type,{stats.EcartTypeMargeNettePct:F2}");
                csv.AppendLine();

                csv.AppendLine("DONNÉES DÉTAILLÉES");
                csv.AppendLine("Date,Titre,Marge nette %,Marge nette €,Prix HT");
                foreach (var result in results.OrderByDescending(r => r.CalculDateTime))
                {
                    csv.AppendLine($"\"{result.CalculDateTime:dd/MM/yyyy HH:mm:ss}\"," +
                        $"\"{result.Titre}\"," +
                        $"{result.MargeNettePct:F2}," +
                        $"{result.MargeNetteEuro:F2}," +
                        $"{result.PrixVenteHT:F2}");
                }

                File.WriteAllText(filePath, csv.ToString(), Encoding.UTF8);
            }
            catch (Exception ex)
            {
                throw new IOException($"Erreur lors de l'export statistiques: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Exporte en format HTML formaté
        /// </summary>
        public static void ExportToHTML(CalculationEngine.Statistics stats, IEnumerable<CalculationEngine.CalculationResult> results, string filePath, string titre = "Rapport de Marges")
        {
            try
            {
                var html = new StringBuilder();

                html.AppendLine("<!DOCTYPE html>");
                html.AppendLine("<html lang=\"fr\">");
                html.AppendLine("<head>");
                html.AppendLine("<meta charset=\"utf-8\">");
                html.AppendLine($"<title>{titre}</title>");
                html.AppendLine("<style>");
                html.AppendLine("body { font-family: Arial, sans-serif; margin: 20px; background-color: #f5f5f5; }");
                html.AppendLine("h1 { color: #1D3557; }");
                html.AppendLine("table { border-collapse: collapse; width: 100%; margin: 20px 0; background: white; }");
                html.AppendLine("th, td { border: 1px solid #ddd; padding: 10px; text-align: left; }");
                html.AppendLine("th { background-color: #2D9CDB; color: white; }");
                html.AppendLine("tr:nth-child(even) { background-color: #f9f9f9; }");
                html.AppendLine(".stat-box { background: white; padding: 15px; margin: 10px 0; border-left: 4px solid #2D9CDB; }");
                html.AppendLine(".positive { color: #27AE60; }");
                html.AppendLine(".negative { color: #E74C3C; }");
                html.AppendLine("</style>");
                html.AppendLine("</head>");
                html.AppendLine("<body>");

                html.AppendLine($"<h1>{titre}</h1>");
                html.AppendLine($"<p>Généré le {DateTime.Now:dd/MM/yyyy HH:mm:ss}</p>");

                html.AppendLine("<h2>Statistiques Générales</h2>");
                html.AppendLine("<div class=\"stat-box\">");
                html.AppendLine($"<p><strong>Nombre de calculs:</strong> {stats.NombreCalculs}</p>");
                html.AppendLine($"<p><strong>Chiffre d'affaires total (TTC):</strong> <span class=\"positive\">{stats.ChiffreAffairesTotal:F2}€</span></p>");
                html.AppendLine($"<p><strong>Marge nette totale:</strong> <span class=\"positive\">{stats.MargeNetteEuroTotal:F2}€</span></p>");
                html.AppendLine("</div>");

                html.AppendLine("<h2>Analyse des Marges</h2>");
                html.AppendLine("<table>");
                html.AppendLine("<tr><th colspan=\"2\">Marge Brute</th><th colspan=\"2\">Marge Nette</th></tr>");
                html.AppendLine("<tr>");
                html.AppendLine($"<th>Moyenne</th><td>{stats.MoyenneMargeBrutePct:F2}%</td>");
                html.AppendLine($"<th>Moyenne</th><td>{stats.MoyenneMargeNettePct:F2}%</td>");
                html.AppendLine("</tr>");
                html.AppendLine("<tr>");
                html.AppendLine($"<th>Min - Max</th><td>{stats.MinMargeBrutePct:F2}% - {stats.MaxMargeBrutePct:F2}%</td>");
                html.AppendLine($"<th>Min - Max</th><td>{stats.MinMargeNettePct:F2}% - {stats.MaxMargeNettePct:F2}%</td>");
                html.AppendLine("</tr>");
                html.AppendLine("<tr>");
                html.AppendLine($"<th>Écart-type</th><td>{stats.EcartTypeMargeBrutePct:F2}%</td>");
                html.AppendLine($"<th>Écart-type</th><td>{stats.EcartTypeMargeNettePct:F2}%</td>");
                html.AppendLine("</tr>");
                html.AppendLine("</table>");

                html.AppendLine("<h2>Détail des Calculs</h2>");
                html.AppendLine("<table>");
                html.AppendLine("<tr><th>Date</th><th>Titre</th><th>Déboursé</th><th>Prix HT</th><th>Marge Nette %</th><th>Marge Nette €</th></tr>");

                foreach (var result in results.OrderByDescending(r => r.CalculDateTime))
                {
                    var margeClass = result.MargeNettePct >= 25 ? "positive" : "negative";
                    html.AppendLine("<tr>");
                    html.AppendLine($"<td>{result.CalculDateTime:dd/MM/yyyy HH:mm}</td>");
                    html.AppendLine($"<td>{result.Titre}</td>");
                    html.AppendLine($"<td>{result.DebourseSec:F2}€</td>");
                    html.AppendLine($"<td>{result.PrixVenteHT:F2}€</td>");
                    html.AppendLine($"<td class=\"{margeClass}\">{result.MargeNettePct:F2}%</td>");
                    html.AppendLine($"<td class=\"{margeClass}\">{result.MargeNetteEuro:F2}€</td>");
                    html.AppendLine("</tr>");
                }

                html.AppendLine("</table>");
                html.AppendLine("</body>");
                html.AppendLine("</html>");

                File.WriteAllText(filePath, html.ToString(), Encoding.UTF8);
            }
            catch (Exception ex)
            {
                throw new IOException($"Erreur lors de l'export HTML: {ex.Message}", ex);
            }
        }
    }
}
