using System;
using System.Collections.Generic;
using System.IO;
using CalculatriceMargeWPF.Helpers;

namespace CalculatriceMargeWPF.Models
{
    /// <summary>
    /// Service pour migrer l'historique existant (fichier texte) vers SQLite
    /// </summary>
    public class HistoryMigrationService
    {
        /// <summary>
        /// Migre les données de l'ancien fichier historique.txt vers la base SQLite
        /// </summary>
        public static void MigrateFromTextFile(string textFilePath, DatabaseService databaseService)
        {
            if (!File.Exists(textFilePath))
            {
                return; // Rien à migrer
            }

            try
            {
                var lines = File.ReadAllLines(textFilePath);
                var migratedCount = 0;

                foreach (var line in lines)
                {
                    if (string.IsNullOrWhiteSpace(line))
                        continue;

                    // Parser la ligne avec l'ancien format
                    if (HistoryParser.TryParseHistoryLine(line, out var parsedEntry))
                    {
                        // Extraire la date si présente
                        DateTime dateCalcul = DateTime.Now;
                        var parts = line.Split('|');
                        if (parts.Length > 0)
                        {
                            var datePart = parts[0].Trim();
                            if (DateTime.TryParse(datePart, out var parsedDate))
                            {
                                dateCalcul = parsedDate;
                            }
                        }

                        // Créer une entrée complète pour la base de données
                        // Il faut recalculer les marges car l'ancien format ne les stockait pas toutes
                        var engine = new CalculationEngine();
                        double tvaDecimal = parsedEntry.TVA / 100.0;
                        double fraisGenerauxValue = parsedEntry.FraisGeneraux;
                        
                        // Recalculer pour avoir toutes les valeurs
                        double prixRevient;
                        if (parsedEntry.FraisModeIndex == 0) // Mode %
                        {
                            prixRevient = parsedEntry.DebourseSec * (1 + fraisGenerauxValue / 100.0);
                        }
                        else // Mode EUR
                        {
                            prixRevient = parsedEntry.DebourseSec + fraisGenerauxValue;
                        }

                        double prixVenteTTC = parsedEntry.PrixVente * (1 + tvaDecimal);
                        double margeBrute = parsedEntry.PrixVente - parsedEntry.DebourseSec;
                        double margeBrutePourcentage = parsedEntry.DebourseSec > 0 
                            ? (margeBrute / parsedEntry.DebourseSec) * 100 
                            : 0;
                        double margeNette = parsedEntry.PrixVente - prixRevient;
                        double margeNettePourcentage = prixRevient > 0 
                            ? (margeNette / prixRevient) * 100 
                            : 0;

                        var entry = new HistoryEntry
                        {
                            Titre = parsedEntry.Titre,
                            DebourseSec = parsedEntry.DebourseSec,
                            FraisGeneraux = parsedEntry.FraisGeneraux,
                            FraisModeIndex = parsedEntry.FraisModeIndex,
                            PrixVenteHT = parsedEntry.PrixVente,
                            TVA = parsedEntry.TVA,
                            PrixRevient = prixRevient,
                            MargeBrute = margeBrute,
                            MargeBrutePourcentage = margeBrutePourcentage,
                            MargeNette = margeNette,
                            MargeNettePourcentage = margeNettePourcentage,
                            PrixVenteTTC = prixVenteTTC,
                            DateCalcul = dateCalcul
                        };

                        databaseService.AddEntry(entry);
                        migratedCount++;
                    }
                }

                // Renommer l'ancien fichier pour le conserver en backup
                if (migratedCount > 0)
                {
                    string backupPath = textFilePath + ".backup";
                    if (File.Exists(backupPath))
                    {
                        File.Delete(backupPath);
                    }
                    File.Move(textFilePath, backupPath);
                }
            }
            catch (Exception ex)
            {
                // Logger l'erreur mais ne pas bloquer l'application
                System.Diagnostics.Debug.WriteLine($"Erreur lors de la migration: {ex.Message}");
            }
        }
    }
}
