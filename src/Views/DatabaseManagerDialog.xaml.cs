using Microsoft.Data.Sqlite;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using CalculatriceMargeWPF.Models;

namespace CalculatriceMargeWPF.Views
{
    public partial class DatabaseManagerDialog : Window
    {
        private readonly DatabaseService _databaseService;
        private readonly string _databasePath;
        private readonly string _backupFolder;

        public DatabaseManagerDialog(DatabaseService databaseService, string databasePath)
        {
            InitializeComponent();
            _databaseService = databaseService;
            _databasePath = databasePath;
            _backupFolder = Path.Combine(Path.GetDirectoryName(_databasePath), "Backups");

            // Créer le dossier de backup s'il n'existe pas
            if (!Directory.Exists(_backupFolder))
            {
                Directory.CreateDirectory(_backupFolder);
            }

            LoadDatabaseInfo();
            LoadElementsInGrid();
        }

        private void LoadDatabaseInfo()
        {
            try
            {
                // Informations générales
                if (File.Exists(_databasePath))
                {
                    var fileInfo = new FileInfo(_databasePath);
                    txtDbPath.Text = _databasePath;
                    txtDbSize.Text = FormatFileSize(fileInfo.Length);
                    txtDbLastModified.Text = fileInfo.LastWriteTime.ToString("dd/MM/yyyy HH:mm:ss");
                }
                else
                {
                    txtDbPath.Text = _databasePath + " (non créé)";
                    txtDbSize.Text = "0 octets";
                    txtDbLastModified.Text = "N/A";
                }

                // Version SQLite
                using var connection = new SqliteConnection($"Data Source={_databasePath}");
                connection.Open();
                var cmd = connection.CreateCommand();
                cmd.CommandText = "SELECT sqlite_version()";
                txtSqliteVersion.Text = cmd.ExecuteScalar()?.ToString() ?? "Inconnue";

                // Statistiques
                var stats = _databaseService.GetStatistics();
                txtDbEntries.Text = stats.Count.ToString();
                txtTotalMargeBrute.Text = stats.TotalMargeBrute.ToString("C");
                txtTotalMargeNette.Text = stats.TotalMargeNette.ToString("C");
                txtAvgMargeBrute.Text = stats.AvgMargeBrutePct.ToString("F2") + " %";
                txtAvgMargeNette.Text = stats.AvgMargeNettePct.ToString("F2") + " %";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors du chargement des informations :\n{ex.Message}",
                    "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private string FormatFileSize(long bytes)
        {
            string[] sizes = { "octets", "Ko", "Mo", "Go" };
            double len = bytes;
            int order = 0;
            while (len >= 1024 && order < sizes.Length - 1)
            {
                order++;
                len = len / 1024;
            }
            return $"{len:0.##} {sizes[order]}";
        }

        private void btnRefreshInfo_Click(object sender, RoutedEventArgs e)
        {
            LoadDatabaseInfo();
            MessageBox.Show("Informations actualisées.", "Succès", 
                MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void btnBackup_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!File.Exists(_databasePath))
                {
                    MessageBox.Show("La base de données n'existe pas encore.", "Information",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                var backupFileName = $"historique_backup_{DateTime.Now:yyyyMMdd_HHmmss}.db";
                var backupPath = Path.Combine(_backupFolder, backupFileName);

                File.Copy(_databasePath, backupPath, true);

                var result = MessageBox.Show(
                    $"✅ Sauvegarde créée avec succès !\n\nEmplacement :\n{backupPath}\n\nTaille : {FormatFileSize(new FileInfo(backupPath).Length)}\n\nOuvrir le dossier de sauvegarde ?",
                    "Succès",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Information);

                if (result == MessageBoxResult.Yes)
                {
                    System.Diagnostics.Process.Start("explorer.exe", _backupFolder);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de la sauvegarde :\n{ex.Message}",
                    "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnRestore_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show(
                "⚠️ Attention : Cette opération va remplacer votre base de données actuelle.\n\n" +
                "Une sauvegarde automatique sera créée avant la restauration.\n\n" +
                "Voulez-vous continuer ?",
                "Confirmation",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result != MessageBoxResult.Yes)
                return;

            try
            {
                // Créer une sauvegarde de la base actuelle
                if (File.Exists(_databasePath))
                {
                    var autoBackupPath = Path.Combine(_backupFolder, 
                        $"historique_avant_restauration_{DateTime.Now:yyyyMMdd_HHmmss}.db");
                    File.Copy(_databasePath, autoBackupPath, true);
                }

                // Sélectionner le fichier à restaurer
                var dialog = new OpenFileDialog
                {
                    Filter = "Base de données SQLite (*.db)|*.db|Tous les fichiers (*.*)|*.*",
                    Title = "Sélectionner la sauvegarde à restaurer"
                };

                if (dialog.ShowDialog() == true)
                {
                    File.Copy(dialog.FileName, _databasePath, true);
                    LoadDatabaseInfo();

                    MessageBox.Show(
                        "✅ Restauration réussie !\n\n" +
                        "La base de données a été restaurée.\n" +
                        "Redémarrez l'application pour appliquer les changements.",
                        "Succès",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de la restauration :\n{ex.Message}",
                    "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnImportText_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                Filter = "Fichier historique (*.txt)|*.txt|Tous les fichiers (*.*)|*.*",
                Title = "Sélectionner le fichier historique.txt à importer"
            };

            if (dialog.ShowDialog() == true)
            {
                try
                {
                    var result = MessageBox.Show(
                        $"Importer les données depuis :\n{dialog.FileName}\n\n" +
                        "Les nouvelles entrées seront ajoutées à la base existante.\n\n" +
                        "Continuer ?",
                        "Confirmation",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Question);

                    if (result == MessageBoxResult.Yes)
                    {
                        HistoryMigrationService.MigrateFromTextFile(dialog.FileName, _databaseService);
                        LoadDatabaseInfo();

                        MessageBox.Show(
                            "✅ Import réussi !\n\n" +
                            "Les données ont été importées dans la base SQLite.",
                            "Succès",
                            MessageBoxButton.OK,
                            MessageBoxImage.Information);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Erreur lors de l'import :\n{ex.Message}",
                        "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void btnVacuum_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var sizeBefore = new FileInfo(_databasePath).Length;

                using var connection = new SqliteConnection($"Data Source={_databasePath}");
                connection.Open();
                var cmd = connection.CreateCommand();
                cmd.CommandText = "VACUUM";
                cmd.ExecuteNonQuery();

                var sizeAfter = new FileInfo(_databasePath).Length;
                var saved = sizeBefore - sizeAfter;

                LoadDatabaseInfo();

                MessageBox.Show(
                    $"✅ Optimisation terminée !\n\n" +
                    $"Taille avant : {FormatFileSize(sizeBefore)}\n" +
                    $"Taille après : {FormatFileSize(sizeAfter)}\n" +
                    $"Espace récupéré : {FormatFileSize(saved)}",
                    "Succès",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de l'optimisation :\n{ex.Message}",
                    "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnIntegrity_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using var connection = new SqliteConnection($"Data Source={_databasePath}");
                connection.Open();
                var cmd = connection.CreateCommand();
                cmd.CommandText = "PRAGMA integrity_check";
                var result = cmd.ExecuteScalar()?.ToString();

                if (result == "ok")
                {
                    MessageBox.Show(
                        "✅ Intégrité vérifiée !\n\n" +
                        "La base de données est en bon état, aucune corruption détectée.",
                        "Succès",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show(
                        $"⚠️ Problème détecté !\n\n{result}\n\n" +
                        "Il est recommandé de restaurer une sauvegarde.",
                        "Attention",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de la vérification :\n{ex.Message}",
                    "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnAnalyze_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using var connection = new SqliteConnection($"Data Source={_databasePath}");
                connection.Open();
                var cmd = connection.CreateCommand();
                cmd.CommandText = "ANALYZE";
                cmd.ExecuteNonQuery();

                MessageBox.Show(
                    "✅ Analyse terminée !\n\n" +
                    "Les statistiques de la base ont été mises à jour.\n" +
                    "Les requêtes devraient être plus rapides.",
                    "Succès",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de l'analyse :\n{ex.Message}",
                    "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnClearAll_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show(
                "⚠️ ATTENTION : Cette action est IRRÉVERSIBLE !\n\n" +
                "Toutes les entrées de l'historique seront définitivement supprimées.\n\n" +
                "Une sauvegarde automatique sera créée avant la suppression.\n\n" +
                "Êtes-vous absolument certain de vouloir continuer ?",
                "Confirmation critique",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result != MessageBoxResult.Yes)
                return;

            // Double confirmation
            var result2 = MessageBox.Show(
                "Dernière confirmation : Supprimer TOUTES les données ?",
                "Confirmation finale",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result2 != MessageBoxResult.Yes)
                return;

            try
            {
                // Créer une sauvegarde avant suppression
                var backupPath = Path.Combine(_backupFolder,
                    $"historique_avant_suppression_{DateTime.Now:yyyyMMdd_HHmmss}.db");
                File.Copy(_databasePath, backupPath, true);

                // Vider la base
                _databaseService.ClearHistory();
                LoadDatabaseInfo();

                MessageBox.Show(
                    "✅ Historique vidé avec succès !\n\n" +
                    $"Une sauvegarde a été créée :\n{backupPath}",
                    "Succès",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de la suppression :\n{ex.Message}",
                    "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void chkAutoBackup_Changed(object sender, RoutedEventArgs e)
        {
            pnlAutoBackupSettings.IsEnabled = chkAutoBackup.IsChecked == true;
        }

        private void LoadElementsInGrid()
        {
            try
            {
                var entries = _databaseService.GetAllEntries();
                dgElements.ItemsSource = entries;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors du chargement des éléments :\n{ex.Message}",
                    "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void txtSearchItems_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            // La recherche en direct peut être activée ici si désiré
        }

        private void btnSearchItems_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string searchTerm = txtSearchItems.Text.ToLower();
                if (string.IsNullOrWhiteSpace(searchTerm))
                {
                    LoadElementsInGrid();
                    return;
                }

                var entries = _databaseService.GetAllEntries();
                var filtered = entries.Where(x => x.Titre.ToLower().Contains(searchTerm)).ToList();
                dgElements.ItemsSource = filtered;

                MessageBox.Show($"✅ {filtered.Count} élément(s) trouvé(s).",
                    "Résultat recherche", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de la recherche :\n{ex.Message}",
                    "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnLoadElement_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (dgElements.SelectedItem == null)
                {
                    MessageBox.Show("Veuillez sélectionner un élément.",
                        "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                var entry = (HistoryEntry)dgElements.SelectedItem;
                MessageBox.Show(
                    $"Titre : {entry.Titre}\n" +
                    $"Date : {entry.DateCalcul:dd/MM/yyyy HH:mm:ss}\n" +
                    $"Déboursé sec : {entry.DebourseSec:F2}€\n" +
                    $"Frais : {entry.FraisGeneraux:F2}{(entry.FraisModeIndex == 0 ? "%" : "€")}\n" +
                    $"Prix revient : {entry.PrixRevient:F2}€\n" +
                    $"Prix vente HT : {entry.PrixVenteHT:F2}€\n" +
                    $"TVA : {entry.TVA:F2}%\n" +
                    $"Marge brute : {entry.MargeBrutePourcentage:F2}% ({entry.MargeBrute:F2}€)\n" +
                    $"Marge nette : {entry.MargeNettePourcentage:F2}% ({entry.MargeNette:F2}€)\n" +
                    $"Prix TTC : {entry.PrixVenteTTC:F2}€",
                    "Détails de l'élément",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur :\n{ex.Message}", "Erreur",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnExportElement_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (dgElements.SelectedItem == null)
                {
                    MessageBox.Show("Veuillez sélectionner un élément.",
                        "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                var entry = (HistoryEntry)dgElements.SelectedItem;

                SaveFileDialog dialog = new SaveFileDialog
                {
                    Filter = "CSV (*.csv)|*.csv",
                    DefaultExt = ".csv",
                    FileName = $"marge_{entry.Titre}_{DateTime.Now:yyyyMMdd_HHmmss}.csv"
                };

                if (dialog.ShowDialog() == true)
                {
                    var result = new CalculationEngine.CalculationResult
                    {
                        Titre = entry.Titre,
                        DebourseSec = entry.DebourseSec,
                        FraisGeneraux = entry.FraisGeneraux,
                        FraisEnPourcent = entry.FraisModeIndex == 0,
                        FraisEnEuro = entry.FraisModeIndex == 0 
                            ? entry.DebourseSec * (entry.FraisGeneraux / 100) 
                            : entry.FraisGeneraux,
                        PrixVenteHT = entry.PrixVenteHT,
                        TVAPct = entry.TVA,
                        PrixRevientTotal = entry.PrixRevient,
                        MargeBruteEuro = entry.MargeBrute,
                        MargeBrutePct = entry.MargeBrutePourcentage,
                        MargeNetteEuro = entry.MargeNette,
                        MargeNettePct = entry.MargeNettePourcentage,
                        PrixVenteTTC = entry.PrixVenteTTC,
                        CalculDateTime = entry.DateCalcul
                    };

                    ExportManager.ExportToCSV(new List<CalculationEngine.CalculationResult> { result }, dialog.FileName);
                    MessageBox.Show($"✅ Élément exporté :\n{dialog.FileName}",
                        "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de l'export :\n{ex.Message}",
                    "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnCopyElement_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (dgElements.SelectedItem == null)
                {
                    MessageBox.Show("Veuillez sélectionner un élément.",
                        "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                var entry = (HistoryEntry)dgElements.SelectedItem;
                string text = $"{entry.DateCalcul:dd/MM/yyyy HH:mm:ss} | {entry.Titre} | DS:{entry.DebourseSec:N2} | FG:{entry.FraisGeneraux:N2} | PR:{entry.PrixRevient:N2} | HT:{entry.PrixVenteHT:N2} | TTC:{entry.PrixVenteTTC:N2} | TVA:{entry.TVA:F2}% | MB:{entry.MargeBrute:N2} ({entry.MargeBrutePourcentage:F2}%) | MN:{entry.MargeNette:N2} ({entry.MargeNettePourcentage:F2}%)";
                
                System.Windows.Clipboard.SetText(text);
                MessageBox.Show("✅ Élément copié dans le presse-papiers.",
                    "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de la copie :\n{ex.Message}",
                    "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
