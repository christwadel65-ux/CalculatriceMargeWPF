using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using CalculatriceMargeWPF.Models;
using CalculatriceMargeWPF.Views;
using CalculatriceMargeWPF.Helpers;
using System.Globalization;

namespace CalculatriceMargeWPF
{
    public partial class MainWindow : Window
    {
        // Constantes
        private const string DEFAULT_TITLE = "Sans titre";
        private const string HISTORIQUE_FOLDER = "CalculatriceMarge";
        private const string HISTORIQUE_SUBFOLDER = "Historique";
        private const string HISTORIQUE_FILENAME = "historique.txt";

        private string historiquePath;
        private DatabaseService _databaseService;
        private Dictionary<int, HistoryEntry> _historyEntryMap; // Map index ListBox -> HistoryEntry
        private CalculationEngine _engine;
        private UndoRedoManager _undoRedoManager;
        private PresetManager _presetManager;
        private List<CalculationEngine.CalculationResult> _resultsList;
        private LocalizationManager _localizationManager;
        private ThemeManager _themeManager;
        private PreferencesManager _preferencesManager;
        private System.Windows.Threading.DispatcherTimer _autoSaveTimer;
        private bool _isInitialized = false;
        
        // Optimisation des performances
        private bool _isLoadingHistory = false;
        private CalculationEngine.Statistics _cachedStats = null;
        private bool _needsDashboardUpdate = false;

        public MainWindow()
        {
            InitializeComponent();
            _engine = new CalculationEngine();
            _undoRedoManager = new UndoRedoManager();
            _presetManager = new PresetManager();
            _resultsList = new List<CalculationEngine.CalculationResult>();
            _historyEntryMap = new Dictionary<int, HistoryEntry>();
            
            // Initialiser les gestionnaires
            _localizationManager = LocalizationManager.Instance;
            _themeManager = ThemeManager.Instance;
            _preferencesManager = PreferencesManager.Instance;
            
            // Charger les préférences
            LoadPreferences();
            
            EnsureHistoriqueFolder();
            ChargerHistorique();
            ChargerPresets();
            RefreshCustomPresetsList();
            
            // Initialiser les valeurs par défaut - Preset Standard
            txtTVA.Text = "20";
            txtFrais.Text = "25";
            cmbFraisMode.SelectedIndex = 0;
            cmbPresets.SelectedIndex = 0;
            
            // Vider explicitement les champs
            txtDebourse.Clear();
            txtVente.Clear();
            
            // Configurer les séparateurs de milliers pour les champs numériques
            NumberFormatter.SetupThousandsSeparatorTextBox(txtDebourse);
            NumberFormatter.SetupThousandsSeparatorTextBox(txtVente);
            NumberFormatter.SetupThousandsSeparatorTextBoxNoDecimals(txtFrais);  // Pas de décimales pour Frais
            NumberFormatter.SetupThousandsSeparatorTextBox(txtTVA);
            NumberFormatter.SetupThousandsSeparatorTextBox(txtRevient);
            NumberFormatter.SetupThousandsSeparatorTextBox(txtRemise);
            NumberFormatter.SetupThousandsSeparatorTextBox(txtPrixApresRemise);
            
            // Support clavier
            this.KeyDown += MainWindow_KeyDown;
            
            // Initialiser l'auto-save
            InitializeAutoSave();
            
            // Marquer comme initialisé
            _isInitialized = true;

            UpdateDashboard();
        }

        private void LoadPreferences()
        {
            string savedTheme = _preferencesManager.GetTheme();
            _themeManager.SetTheme(savedTheme);
            _themeManager.OnThemeChanged += () => ApplyTheme();
            ApplyTheme();

            string savedLanguage = _preferencesManager.GetLanguage();
            _localizationManager.SetLanguage(savedLanguage);
        }

        private void InitializeAutoSave()
        {
            if (!_preferencesManager.GetAutoSaveEnabled())
                return;

            _autoSaveTimer = new System.Windows.Threading.DispatcherTimer();
            _autoSaveTimer.Interval = TimeSpan.FromSeconds(_preferencesManager.GetAutoSaveInterval());
            _autoSaveTimer.Tick += (s, e) => AutoSaveHistorique();
            _autoSaveTimer.Start();
        }

        private void AutoSaveHistorique()
        {
            try
            {
                if (!string.IsNullOrEmpty(txtTitre.Text) && lstHistorique.Items.Count > 0)
                {
                    // Auto-save logic already handled in btnCalculer_Click
                }
            }
            catch { }
        }

        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return) // Entrée pour calculer
            {
                btnCalculer_Click(null, null);
                e.Handled = true;
            }
            else if (e.Key == Key.Z && (Keyboard.Modifiers & ModifierKeys.Control) != 0) // Ctrl+Z pour Undo
            {
                ExecuteUndo();
                e.Handled = true;
            }
            else if (e.Key == Key.Y && (Keyboard.Modifiers & ModifierKeys.Control) != 0) // Ctrl+Y pour Redo
            {
                ExecuteRedo();
                e.Handled = true;
            }
            else if (e.Key == Key.S && (Keyboard.Modifiers & ModifierKeys.Control) != 0) // Ctrl+S pour sauvegarder
            {
                MenuSauvegarder_Click(null, null);
                e.Handled = true;
            }
            else if (e.Key == Key.N && (Keyboard.Modifiers & ModifierKeys.Control) != 0) // Ctrl+N pour nouveau
            {
                btnReset_Click(null, null);
                e.Handled = true;
            }
        }

        private void ChargerPresets()
        {
            cmbPresets.Items.Clear();
            cmbPresets.Items.Add("Standard (20% TVA, 25% FG)");
            cmbPresets.Items.Add("Réduit (5.5% TVA, 8% FG)");
            cmbPresets.Items.Add("Service (10% TVA, 15% FG)");
            cmbPresets.SelectedIndex = 0;
        }

        private void RefreshCustomPresetsList()
        {
            lstCustomPresets.ItemsSource = null;
            lstCustomPresets.ItemsSource = _presetManager.Presets;

            if (_presetManager.Presets.Count > 0)
                lstCustomPresets.SelectedIndex = 0;
        }

        private void EnsureHistoriqueFolder()
        {
            // Utiliser AppData pour éviter les problèmes de permissions dans Program Files
            string appDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string dossier = Path.Combine(appDataFolder, HISTORIQUE_FOLDER, HISTORIQUE_SUBFOLDER);
            if (!Directory.Exists(dossier))
            {
                Directory.CreateDirectory(dossier);
            }
            historiquePath = Path.Combine(dossier, HISTORIQUE_FILENAME);

            // Initialiser la base de données SQLite
            _databaseService = new DatabaseService(dossier);

            // Migrer l'ancien historique si nécessaire
            if (File.Exists(historiquePath))
            {
                HistoryMigrationService.MigrateFromTextFile(historiquePath, _databaseService);
            }
        }

        private async void ChargerHistorique()
        {
            try
            {
                // Éviter les rechargements inutiles
                if (_isLoadingHistory)
                    return;

                _isLoadingHistory = true;

                lstHistorique.Items.Clear();
                _historyEntryMap.Clear();
                _resultsList.Clear();
                _cachedStats = null; // Invalider le cache
                _needsDashboardUpdate = true;

                var entries = await _databaseService.GetAllEntriesAsync();
                int index = 0;
                foreach (var entry in entries)
                {
                    lstHistorique.Items.Add(entry.ToString());
                    _historyEntryMap[index] = entry;
                    
                    // Reconstruire _resultsList pour les statistiques
                    double fraisEnEuro = entry.FraisModeIndex == 0 
                        ? entry.DebourseSec * (entry.FraisGeneraux / 100) 
                        : entry.FraisGeneraux;

                    var result = new CalculationEngine.CalculationResult
                    {
                        Titre = entry.Titre,
                        DebourseSec = entry.DebourseSec,
                        FraisGeneraux = entry.FraisGeneraux,
                        FraisEnPourcent = entry.FraisModeIndex == 0,
                        FraisEnEuro = fraisEnEuro,
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
                    _resultsList.Add(result);
                    index++;
                }

                _isLoadingHistory = false;
            }
            catch (Exception ex)
            {
                _isLoadingHistory = false;
                MessageBox.Show($"Erreur lors du chargement de l'historique:\n{ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private async void btnCalculer_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!TryLireSaisies(out double debourseSec, out double prixVenteHT, out double tva, out double fraisGenerauxPct, out bool fraisEnPourcent)) 
                    return;

                // Vérifier si une remise est appliquée et utiliser le prix après remise pour le calcul
                double prixPourCalcul = prixVenteHT;
                if (NumberFormatter.TryParseFormattedNumber(txtPrixApresRemise.Text, out double prixApresRemise) && prixApresRemise > 0)
                {
                    prixPourCalcul = prixApresRemise;
                }

                // Utiliser le CalculationEngine avec le prix ajusté
                var result = _engine.Calculate(debourseSec, prixPourCalcul, tva, fraisGenerauxPct, fraisEnPourcent);
                
                // Récupérer le titre
                result.Titre = string.IsNullOrWhiteSpace(txtTitre.Text) ? DEFAULT_TITLE : txtTitre.Text;
                
                // Vérifier les doublons
                var existingResult = _resultsList.FirstOrDefault(r => r.Titre == result.Titre);
                int indexHistorique = -1;
                
                if (existingResult != null)
                {
                    // Demander confirmation pour écraser
                    var reponse = MessageBox.Show(
                        $"Un calcul avec le titre \"{result.Titre}\" existe déjà.\n\nVoulez-vous le mettre à jour ?",
                        "Confirmation",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Question);
                    
                    if (reponse == MessageBoxResult.No)
                    {
                        return; // Annuler
                    }
                    
                    // Supprimer l'ancien de la liste
                    _resultsList.Remove(existingResult);
                    
                    // Trouver l'index dans l'historique pour mise à jour
                    for (int i = 0; i < lstHistorique.Items.Count; i++)
                    {
                        string item = lstHistorique.Items[i].ToString();
                        if (item.StartsWith(result.Titre + " |"))
                        {
                            indexHistorique = i;
                            break;
                        }
                    }
                }
                
                // Ajouter aux résultats
                _resultsList.Add(result);
                _undoRedoManager.Push(result);
                
                // Afficher les résultats
                AfficherResultats(result);
                
                // Sauvegarder dans l'historique SQLite
                try
                {
                    var historyEntry = new HistoryEntry
                    {
                        Titre = result.Titre,
                        DebourseSec = result.DebourseSec,
                        FraisGeneraux = fraisGenerauxPct,
                        FraisModeIndex = fraisEnPourcent ? 0 : 1,
                        PrixVenteHT = result.PrixVenteHT,
                        TVA = tva,
                        PrixRevient = result.PrixRevientTotal,
                        MargeBrute = result.MargeBruteEuro,
                        MargeBrutePourcentage = result.MargeBrutePct,
                        MargeNette = result.MargeNetteEuro,
                        MargeNettePourcentage = result.MargeNettePct,
                        PrixVenteTTC = result.PrixVenteTTC,
                        DateCalcul = DateTime.Now
                    };

                    if (indexHistorique >= 0 && _historyEntryMap.ContainsKey(indexHistorique))
                    {
                        // Mettre à jour l'entrée existante
                        var existingEntry = _historyEntryMap[indexHistorique];
                        await _databaseService.DeleteEntryAsync(existingEntry.Id);
                    }

                    long newId = await _databaseService.AddEntryAsync(historyEntry);
                    historyEntry.Id = (int)newId;

                    // Recharger l'historique depuis la base
                    ChargerHistorique();
                }
                catch (Exception exHist)
                {
                    MessageBox.Show($"Erreur lors de la sauvegarde dans l'historique:\n{exHist.Message}", "Avertissement", MessageBoxButton.OK, MessageBoxImage.Warning);
                }

                UpdateDashboard();
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show($"Erreur de validation:\n{ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors du calcul:\n{ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void AfficherResultats(CalculationEngine.CalculationResult result)
        {
            txtRevient.Text = result.PrixRevientTotal.ToString("F2");
            txtRecapDS.Text = result.DebourseSec.ToString("C");
            txtRecapHT.Text = result.PrixVenteHT.ToString("C");
            txtRecapTTC.Text = result.PrixVenteTTC.ToString("C");
            txtPrixRevient.Text = result.PrixRevientTotal.ToString("C");
            txtMargeBrute.Text = $"{result.MargeBruteEuro:C} ({result.MargeBrutePct:F2}%)";
            txtMargeNette.Text = $"{result.MargeNetteEuro:C} ({result.MargeNettePct:F2}%)";
            
            // Afficher la remise si elle existe
            if (NumberFormatter.TryParseFormattedNumber(txtRemise.Text, out double remisePct) && remisePct > 0)
            {
                // Utiliser le prix INITIAL (txtVente) pour calculer le montant de la remise
                if (NumberFormatter.TryParseFormattedNumber(txtVente.Text, out double prixInitial))
                {
                    double montantRemise = prixInitial * (remisePct / 100.0);
                    txtRecapRemise.Text = $"{remisePct:F2}% ({montantRemise:C})";
                }
                else
                {
                    txtRecapRemise.Text = $"{remisePct:F2}%";
                }
            }
            else
            {
                txtRecapRemise.Text = "Pas de remise";
            }
            
            // Alerte si marge < 15%
            // Note: Ajouter lblAvertissement dans le XAML pour afficher ces alertes
            //if (result.MargeNettePct < 15)
            //{
            //    lblAvertissement.Content = $"⚠️ Marge faible ({result.MargeNettePct:F1}%)";
            //    lblAvertissement.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(227, 76, 60));
            //}
            //else if (result.MargeNettePct >= 25)
            //{
            //    lblAvertissement.Content = $"✓ Marge saine ({result.MargeNettePct:F1}%)";
            //    lblAvertissement.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(39, 174, 96));
            //}
            //else
            //{
            //    lblAvertissement.Content = $"≈ Marge acceptable ({result.MargeNettePct:F1}%)";
            //    lblAvertissement.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(241, 196, 15));
            //}
        }

        private async void UpdateDashboard()
        {
            // Ne mettre à jour que si nécessaire (flag)
            if (!_needsDashboardUpdate && _cachedStats != null)
                return;

            if (_resultsList.Count == 0)
            {
                txtDashCalculs.Text = "--";
                txtDashCA.Text = "--";
                txtDashMargeNetteMoy.Text = "--";
                txtDashMargeNetteMinMax.Text = "--";
                txtDashMargeBruteMoy.Text = "--";
                txtDashLastTitle.Text = "--";
                _needsDashboardUpdate = false;
                return;
            }

            // Calculer et mettre en cache les statistiques de manière asynchrone
            await Task.Run(() =>
            {
                _cachedStats = _engine.ComputeStatistics(_resultsList.ToArray());
            });

            txtDashCalculs.Text = _cachedStats.NombreCalculs.ToString();
            txtDashCA.Text = $"{_cachedStats.ChiffreAffairesTotal:F2} €";
            txtDashMargeNetteMoy.Text = $"{_cachedStats.MoyenneMargeNettePct:F2}%";
            txtDashMargeNetteMinMax.Text = $"{_cachedStats.MinMargeNettePct:F2}% / {_cachedStats.MaxMargeNettePct:F2}%";
            txtDashMargeBruteMoy.Text = $"{_cachedStats.MoyenneMargeBrutePct:F2}%";
            txtDashLastTitle.Text = _resultsList.Last().Titre;
            
            _needsDashboardUpdate = false;
        }

        private void ExecuteUndo()
        {
            var previous = _undoRedoManager.Undo();
            if (previous != null)
            {
                AfficherResultats(previous);
                MessageBox.Show("Calcul annulé (Undo)", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void ExecuteRedo()
        {
            var next = _undoRedoManager.Redo();
            if (next != null)
            {
                AfficherResultats(next);
                MessageBox.Show("Calcul rétabli (Redo)", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void btnCalculInverse_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!NumberFormatter.TryParseFormattedNumber(txtVente.Text, out double prixVente) || prixVente <= 0)
                {
                    MessageBox.Show("Entrez un prix de vente valide.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (!NumberFormatter.TryParseFormattedNumber(txtTVA.Text, out double tva))
                {
                    MessageBox.Show("Entrez une TVA valide.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Ouvrir la fenêtre de saisie de la marge visée
                var margeDialog = new MargeViseeDialog();
                margeDialog.Owner = this;
                
                if (margeDialog.ShowDialog() != true || !margeDialog.IsConfirmed)
                {
                    return; // Annulation
                }

                double margeTarget = margeDialog.MargeNetteCible;
                double? margeBruteMin = margeDialog.MargeBruteCible;

                bool fraisEnPourcent = cmbFraisMode.SelectedIndex == 0;
                if (!NumberFormatter.TryParseFormattedNumber(txtFrais.Text, out double fraisGeneraux))
                    fraisGeneraux = 10;

                var result = _engine.CalculateInverse(prixVente, margeTarget, tva, fraisEnPourcent, fraisGeneraux);
                result.Titre = string.IsNullOrWhiteSpace(txtTitre.Text) ? $"{DEFAULT_TITLE} (inversé)" : $"{txtTitre.Text} (inversé)";

                // Vérifier la contrainte de marge brute si définie
                if (margeBruteMin.HasValue && result.MargeBrutePct < margeBruteMin.Value)
                {
                    MessageBox.Show(
                        $"⚠️ Attention: La marge brute calculée ({result.MargeBrutePct:F2}%) est inférieure à la minimale demandée ({margeBruteMin.Value:F2}%).\n\n" +
                        $"Déboursé sec: {result.DebourseSec:F2}€\n" +
                        $"Prix de revient: {result.PrixRevientTotal:F2}€\n" +
                        $"Marge brute: {result.MargeBrutePct:F2}%\n" +
                        $"Marge nette: {result.MargeNettePct:F2}%",
                        "Résultat (sous contrainte)", 
                        MessageBoxButton.OK, 
                        MessageBoxImage.Warning);
                }
                else
                {
                    MessageBox.Show(
                        $"✓ Calcul inversé réussi:\n\n" +
                        $"Déboursé sec max: {result.DebourseSec:F2}€\n" +
                        $"Prix de revient: {result.PrixRevientTotal:F2}€\n" +
                        $"Marge brute: {result.MargeBrutePct:F2}%\n" +
                        $"Marge nette: {result.MargeNettePct:F2}%",
                        "Résultat", 
                        MessageBoxButton.OK, 
                        MessageBoxImage.Information);
                }

                _resultsList.Add(result);
                _undoRedoManager.Push(result);
                AfficherResultats(result);
                UpdateDashboard();
                txtDebourse.Text = result.DebourseSec.ToString("F2");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur calcul inversé:\n{ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnExportCSV_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_resultsList.Count == 0)
                {
                    MessageBox.Show("Aucun calcul à exporter.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                SaveFileDialog dialog = new SaveFileDialog
                {
                    Filter = "CSV (*.csv)|*.csv",
                    DefaultExt = ".csv",
                    FileName = $"marges_{DateTime.Now:yyyyMMdd_HHmmss}.csv"
                };

                if (dialog.ShowDialog() == true)
                {
                    ExportManager.ExportToCSV(_resultsList, dialog.FileName);
                    MessageBox.Show($"Exporté vers:\n{dialog.FileName}", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur export:\n{ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnExportHTML_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_resultsList.Count == 0)
                {
                    MessageBox.Show("Aucun calcul à exporter.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                var stats = _engine.ComputeStatistics(_resultsList.ToArray());

                SaveFileDialog dialog = new SaveFileDialog
                {
                    Filter = "HTML (*.html)|*.html",
                    DefaultExt = ".html",
                    FileName = $"rapport_marges_{DateTime.Now:yyyyMMdd_HHmmss}.html"
                };

                if (dialog.ShowDialog() == true)
                {
                    ExportManager.ExportToHTML(stats, _resultsList, dialog.FileName, "Rapport de marges");
                    MessageBox.Show($"Rapport HTML créé:\n{dialog.FileName}", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur export HTML:\n{ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnStatistiques_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string appDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                string databasePath = System.IO.Path.Combine(appDataFolder, HISTORIQUE_FOLDER, HISTORIQUE_SUBFOLDER, "historique.db");
                
                var dialog = new DatabaseManagerDialog(_databaseService, databasePath);
                dialog.Owner = this;
                dialog.ShowDialog();

                // Recharger l'historique après fermeture de la fenêtre de gestion
                ChargerHistorique();
                UpdateDashboard();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de l'ouverture du gestionnaire de base de données :\n{ex.Message}", 
                    "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnExportStats_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_resultsList.Count == 0)
                {
                    MessageBox.Show("Aucun calcul pour les statistiques.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                var stats = _engine.ComputeStatistics(_resultsList.ToArray());

                SaveFileDialog dialog = new SaveFileDialog
                {
                    Filter = "CSV (*.csv)|*.csv",
                    DefaultExt = ".csv",
                    FileName = $"stats_marges_{DateTime.Now:yyyyMMdd_HHmmss}.csv"
                };

                if (dialog.ShowDialog() == true)
                {
                    ExportManager.ExportStatistics(stats, _resultsList, dialog.FileName);
                    MessageBox.Show($"Statistiques exportées:\n{dialog.FileName}", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur export statistiques:\n{ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnCopyLast_Click(object sender, RoutedEventArgs e)
        {
            if (_resultsList.Count == 0)
            {
                MessageBox.Show("Aucun calcul à copier.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var last = _resultsList.Last();
            var sb = new StringBuilder();
            sb.AppendLine("Titre\tDéboursé sec\tFrais généraux\tMode\tPrix revient\tHT\tTTC\tTVA%\tMarge brute%\tMarge nette%");
            sb.AppendLine($"{last.Titre}\t{last.DebourseSec:F2}\t{last.FraisGeneraux:F2}\t{(last.FraisEnPourcent ? "%" : "€")}\t{last.PrixRevientTotal:F2}\t{last.PrixVenteHT:F2}\t{last.PrixVenteTTC:F2}\t{last.TVAPct:F2}\t{last.MargeBrutePct:F2}\t{last.MargeNettePct:F2}");

            Clipboard.SetText(sb.ToString());
            MessageBox.Show("Dernier résultat copié dans le presse-papiers.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void btnReset_Click(object sender, RoutedEventArgs e)
        {
            txtTitre.Text = "";
            txtDebourse.Text = "";
            txtRevient.Text = "";
            txtVente.Text = "";
            txtTVA.Text = "20";
            txtFrais.Text = "10";
            txtRecapDS.Text = string.Empty;
            txtRecapHT.Text = string.Empty;
            txtRecapTTC.Text = string.Empty;
            txtPrixRevient.Text = string.Empty;
            txtMargeBrute.Text = string.Empty;
            txtMargeNette.Text = string.Empty;
            cmbFraisMode.SelectedIndex = 0;
            cmbPresets.SelectedIndex = 0;
        }

        private void MenuQuitter_Click(object sender, RoutedEventArgs e) => Application.Current.Shutdown();

        private void MenuHistorique_Click(object sender, RoutedEventArgs e)
        {
            bool visible = lstHistorique.Visibility == Visibility.Collapsed;
            lstHistorique.Visibility = visible ? Visibility.Visible : Visibility.Collapsed;
        }

        private void txtRemise_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            CalculerRemise();
        }

        private void txtVente_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            CalculerRemise();
        }

        private void CalculerRemise()
        {
            if (!_isInitialized) return;
            
            try
            {
                // Vérifier que les champs nécessaires sont remplis
                if (string.IsNullOrWhiteSpace(txtVente.Text) || string.IsNullOrWhiteSpace(txtRemise.Text))
                {
                    txtPrixApresRemise.Text = "";
                    return;
                }

                // Parser les valeurs
                if (double.TryParse(txtVente.Text.Replace(" ", ""), out double prixHT) &&
                    double.TryParse(txtRemise.Text.Replace(" ", ""), out double remisePct))
                {
                    // Si remise = 0%, vider le champ
                    if (remisePct == 0)
                    {
                        txtPrixApresRemise.Text = "";
                        return;
                    }
                    
                    // Calculer le prix après remise
                    double montantRemise = prixHT * (remisePct / 100.0);
                    double prixApresRemise = prixHT - montantRemise;
                    
                    // Afficher le résultat
                    txtPrixApresRemise.Text = prixApresRemise.ToString("F2");
                }
            }
            catch
            {
                txtPrixApresRemise.Text = "";
            }
        }

        private void MenuSauvegarder_Click(object sender, RoutedEventArgs e)
        {
            if (lstHistorique.Items.Count == 0)
            {
                MessageBox.Show("L'historique est vide.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "Fichier texte (*.txt)|*.txt",
                Title = "Sauvegarder l'historique",
                InitialDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Historique")
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                using (StreamWriter writer = new StreamWriter(saveFileDialog.FileName))
                {
                    foreach (var item in lstHistorique.Items)
                    {
                        writer.WriteLine(item.ToString());
                    }
                }
                MessageBox.Show("Historique sauvegardé avec succès.", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void btnClearHistory_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Voulez-vous vraiment supprimer tout l'historique ?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                _databaseService.ClearHistory();
                lstHistorique.Items.Clear();
                _historyEntryMap.Clear();
                _resultsList.Clear();
                UpdateDashboard();
                MessageBox.Show("Historique nettoyé avec succès.", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private async void btnDeleteSelected_Click(object sender, RoutedEventArgs e)
        {
            if (lstHistorique.SelectedItem == null)
            {
                MessageBox.Show("Veuillez sélectionner un élément à supprimer.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            if (MessageBox.Show("Voulez-vous vraiment supprimer cet élément ?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                try
                {
                    int selectedIndex = lstHistorique.SelectedIndex;
                    
                    if (_historyEntryMap.ContainsKey(selectedIndex))
                    {
                        var entryToDelete = _historyEntryMap[selectedIndex];
                        
                        // Supprimer de la base de données
                        await _databaseService.DeleteEntryAsync(entryToDelete.Id);
                        
                        // Supprimer de la liste des résultats
                        var match = _resultsList.FirstOrDefault(r =>
                            r.Titre == entryToDelete.Titre &&
                            Math.Abs(r.DebourseSec - entryToDelete.DebourseSec) < 0.01 &&
                            Math.Abs(r.PrixVenteHT - entryToDelete.PrixVenteHT) < 0.01);

                        if (match != null)
                            _resultsList.Remove(match);
                        
                        // Recharger l'historique depuis la base
                        ChargerHistorique();
                    }
                    
                    // Réinitialiser les champs de la calculatrice
                    btnReset_Click(null, null);
                    UpdateDashboard();
                    
                    MessageBox.Show("Élément supprimé avec succès.", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Erreur lors de la suppression : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void lstHistorique_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (lstHistorique.SelectedItem == null) return;

            int selectedIndex = lstHistorique.SelectedIndex;
            
            if (!_historyEntryMap.ContainsKey(selectedIndex))
            {
                MessageBox.Show("Impossible de charger ce calcul.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                var entry = _historyEntryMap[selectedIndex];
                
                txtTitre.Text = entry.Titre;
                txtDebourse.Text = entry.DebourseSec.ToString("F2");
                txtFrais.Text = entry.FraisGeneraux.ToString("F2");
                txtVente.Text = entry.PrixVenteHT.ToString("F2");
                txtTVA.Text = entry.TVA.ToString("F2");
                cmbFraisMode.SelectedIndex = entry.FraisModeIndex;

                MessageBox.Show("Calcul rechargé depuis l'historique. Cliquez sur 'Calculer' pour voir les résultats.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors du chargement : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void lstHistorique_CopyToClipboard(object sender, RoutedEventArgs e)
        {
            try
            {
                if (lstHistorique.SelectedItem == null)
                {
                    MessageBox.Show("Veuillez sélectionner un élément à copier.", "Information", 
                        MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                string selectedText = lstHistorique.SelectedItem.ToString();
                System.Windows.Clipboard.SetText(selectedText);
                MessageBox.Show("✅ Élément copié dans le presse-papiers.", "Succès",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de la copie :\n{ex.Message}", "Erreur",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void lstHistorique_ExportCSV(object sender, RoutedEventArgs e)
        {
            try
            {
                if (lstHistorique.SelectedItem == null)
                {
                    MessageBox.Show("Veuillez sélectionner un élément à exporter.", "Information",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                int selectedIndex = lstHistorique.SelectedIndex;
                if (!_historyEntryMap.ContainsKey(selectedIndex))
                {
                    MessageBox.Show("Élément introuvable.", "Erreur",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var entry = _historyEntryMap[selectedIndex];
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

                SaveFileDialog dialog = new SaveFileDialog
                {
                    Filter = "CSV (*.csv)|*.csv",
                    DefaultExt = ".csv",
                    FileName = $"marge_{entry.Titre}_{DateTime.Now:yyyyMMdd_HHmmss}.csv"
                };

                if (dialog.ShowDialog() == true)
                {
                    ExportManager.ExportToCSV(new List<CalculationEngine.CalculationResult> { result }, dialog.FileName);
                    MessageBox.Show($"✅ Élément exporté vers:\n{dialog.FileName}", "Succès",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de l'export CSV :\n{ex.Message}", "Erreur",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void lstHistorique_ExportHTML(object sender, RoutedEventArgs e)
        {
            try
            {
                if (lstHistorique.SelectedItem == null)
                {
                    MessageBox.Show("Veuillez sélectionner un élément à exporter.", "Information",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                int selectedIndex = lstHistorique.SelectedIndex;
                if (!_historyEntryMap.ContainsKey(selectedIndex))
                {
                    MessageBox.Show("Élément introuvable.", "Erreur",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var entry = _historyEntryMap[selectedIndex];
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

                SaveFileDialog dialog = new SaveFileDialog
                {
                    Filter = "HTML (*.html)|*.html",
                    DefaultExt = ".html",
                    FileName = $"marge_{entry.Titre}_{DateTime.Now:yyyyMMdd_HHmmss}.html"
                };

                if (dialog.ShowDialog() == true)
                {
                    var stats = _engine.ComputeStatistics(new[] { result });
                    ExportManager.ExportToHTML(stats, new List<CalculationEngine.CalculationResult> { result }, 
                        dialog.FileName, $"Rapport - {entry.Titre}");
                    MessageBox.Show($"✅ Élément exporté vers:\n{dialog.FileName}", "Succès",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de l'export HTML :\n{ex.Message}", "Erreur",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool TryLireSaisies(out double debourseSec, out double prixVenteHT, out double tva, out double fraisGenerauxPct, out bool fraisEnPourcent, bool allowEmptyAmounts = false, bool requireCharges = false)
        {
            debourseSec = prixVenteHT = tva = fraisGenerauxPct = 0;
            fraisEnPourcent = cmbFraisMode.SelectedIndex == 0;

            // Parser les valeurs avec gestion des champs vides
            bool deburseOk = string.IsNullOrWhiteSpace(txtDebourse.Text) || NumberFormatter.TryParseFormattedNumber(txtDebourse.Text, out debourseSec);
            bool venteOk = string.IsNullOrWhiteSpace(txtVente.Text) || NumberFormatter.TryParseFormattedNumber(txtVente.Text, out prixVenteHT);

            bool tvaOk;
            bool fraisOk;

            if (requireCharges)
            {
                // En mode sauvegarde de preset : TVA et frais doivent être fournis
                if (string.IsNullOrWhiteSpace(txtTVA.Text) || string.IsNullOrWhiteSpace(txtFrais.Text))
                {
                    MessageBox.Show("Renseignez TVA et Frais généraux avant de sauvegarder un preset.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                    return false;
                }

                tvaOk = NumberFormatter.TryParseFormattedNumber(txtTVA.Text, out tva);
                fraisOk = NumberFormatter.TryParseFormattedNumber(txtFrais.Text, out fraisGenerauxPct);
            }
            else
            {
                tvaOk = string.IsNullOrWhiteSpace(txtTVA.Text) || NumberFormatter.TryParseFormattedNumber(txtTVA.Text, out tva);
                fraisOk = string.IsNullOrWhiteSpace(txtFrais.Text) || NumberFormatter.TryParseFormattedNumber(txtFrais.Text, out fraisGenerauxPct);

                // Si les champs vides, utiliser valeurs par défaut pour le calcul
                if (string.IsNullOrWhiteSpace(txtTVA.Text)) tva = 20;
                if (string.IsNullOrWhiteSpace(txtFrais.Text)) fraisGenerauxPct = 25;
            }

            if (!deburseOk || !venteOk || !tvaOk || !fraisOk)
            {
                MessageBox.Show("Veuillez entrer des valeurs numériques valides.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            // Vérifier qu'au moins un champ essentiel est rempli (sauf si autorisé)
            if (!allowEmptyAmounts && debourseSec == 0 && prixVenteHT == 0)
            {
                MessageBox.Show("Veuillez remplir au moins le Déboursé sec ou le Prix de vente HT.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            return true;
        }

        private void btnApplyPreset_Click(object sender, RoutedEventArgs e)
        {
            switch (cmbPresets.SelectedIndex)
            {
                case 0: // Standard
                    txtTVA.Text = "20";
                    txtFrais.Text = "25";
                    cmbFraisMode.SelectedIndex = 0;
                    break;
                case 1: // Réduit
                    txtTVA.Text = "5.5";
                    txtFrais.Text = "8";
                    cmbFraisMode.SelectedIndex = 0;
                    break;
                case 2: // Service
                    txtTVA.Text = "10";
                    txtFrais.Text = "15";
                    cmbFraisMode.SelectedIndex = 0;
                    break;
                
                    
            }
        }

        private void btnSavePreset_Click(object sender, RoutedEventArgs e)
        {
            // Préremplir TVA/Frais si le nom contient "tva;frais"
            TryApplyInlinePresetValues();

            // Popup de saisie TVA et Frais généraux
            var presetDialog = new CustomPresetDialog(txtTVA.Text, txtFrais.Text, cmbFraisMode.SelectedIndex == 0)
            {
                Owner = this
            };

            if (presetDialog.ShowDialog() != true)
                return;

            txtTVA.Text = presetDialog.TVA.ToString("F2", CultureInfo.InvariantCulture);
            txtFrais.Text = presetDialog.FraisGeneraux.ToString("F2", CultureInfo.InvariantCulture);
            cmbFraisMode.SelectedIndex = presetDialog.FraisEnPourcent ? 0 : 1;

            // Permettre d'enregistrer un preset même si Déboursé/Prix sont vides, mais exiger TVA et FG
            if (!TryLireSaisies(out double debourseSec, out double prixVenteHT, out double tva, out double fraisGenerauxPct, out bool fraisEnPourcent, allowEmptyAmounts: true, requireCharges: true))
                return;

            string name = $"TVA {tva:F2}% - FG {fraisGenerauxPct:F2}{(fraisEnPourcent ? "%" : "€")}";

            var preset = new Preset
            {
                Name = name,
                DebourseSec = debourseSec,
                PrixVenteHT = prixVenteHT,
                TVA = tva,
                FraisGeneraux = fraisGenerauxPct,
                FraisEnPourcent = fraisEnPourcent,
                CreatedAt = DateTime.Now
            };

            _presetManager.SavePreset(preset);
            RefreshCustomPresetsList();
            lstCustomPresets.SelectedItem = preset;

            MessageBox.Show($"Preset \"{name}\" enregistré.", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        /// <summary>
        /// Permet de saisir rapidement TVA et Frais via le nom du preset sous la forme "tva;frais" (ex: 20;40)
        /// </summary>
        private bool TryApplyInlinePresetValues()
        {
            return false; // Not used anymore
        }

        private void btnApplyCustomPreset_Click(object sender, RoutedEventArgs e)
        {
            if (lstCustomPresets.SelectedItem is not Preset preset)
            {
                MessageBox.Show("Sélectionnez un preset personnalisé.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            txtDebourse.Text = preset.DebourseSec.ToString("F2");
            txtVente.Text = preset.PrixVenteHT.ToString("F2");
            txtTVA.Text = preset.TVA.ToString("F2");
            txtFrais.Text = preset.FraisGeneraux.ToString("F2");
            cmbFraisMode.SelectedIndex = preset.FraisEnPourcent ? 0 : 1;
        }

        private void btnDeletePreset_Click(object sender, RoutedEventArgs e)
        {
            if (lstCustomPresets.SelectedItem is not Preset preset)
            {
                MessageBox.Show("Sélectionnez un preset à supprimer.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            _presetManager.DeletePreset(preset);
            RefreshCustomPresetsList();
        }

        private void lstCustomPresets_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            // Selection change handled
        }

        private bool isDarkMode = false;

        private void MenuToggleDarkMode_Click(object sender, RoutedEventArgs e)
        {
            isDarkMode = !isDarkMode;
            ApplyTheme();
        }

        private void ApplyTheme()
        {
            var theme = _themeManager.CurrentTheme;
            var bgBrush = new System.Windows.Media.SolidColorBrush(theme.BackgroundColor);
            var fgBrush = new System.Windows.Media.SolidColorBrush(theme.ForegroundColor);
            
            this.Background = bgBrush;
            this.Foreground = fgBrush;
            
            UpdateMenuColors();
        }

        private void MenuTheme_Click(object sender, RoutedEventArgs e)
        {
            if (sender is System.Windows.Controls.MenuItem menuItem && menuItem.Tag is string themeName)
            {
                _themeManager.SetTheme(themeName);
                _preferencesManager.SetTheme(themeName);
                ApplyTheme();
                UpdateMenuColors();
            }
        }

        private void MenuLanguage_Click(object sender, RoutedEventArgs e)
        {
            if (sender is System.Windows.Controls.MenuItem menuItem && menuItem.Tag is string languageCode)
            {
                _localizationManager.SetLanguage(languageCode);
                _preferencesManager.SetLanguage(languageCode);
                UpdateUILanguage();
            }
        }

        private void UpdateMenuColors()
        {
            // Les styles XAML gèrent déjà les couleurs du menu
            // Pas besoin de mise à jour en code-behind
        }

        private void UpdateUILanguage()
        {
            // Cette fonction peut être étendue pour mettre à jour tous les textes dynamiquement
            // Pour l'instant, un redémarrage de l'application est recommandé
            MessageBox.Show("La langue sera changée au prochain démarrage de l'application.", "Langue", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void MenuPreferences_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Fenêtre de préférences en cours de développement.\n\nPréférences actuelles :\n" +
                $"• Thème : {_themeManager.CurrentTheme.Name}\n" +
                $"• Langue : {_localizationManager.CurrentLanguage}\n" +
                $"• Auto-save : {_preferencesManager.GetAutoSaveEnabled()}\n" +
                $"• Intervalle auto-save : {_preferencesManager.GetAutoSaveInterval()}s",
                "Préférences", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void MenuDatabase_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string appDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                string databasePath = System.IO.Path.Combine(appDataFolder, HISTORIQUE_FOLDER, HISTORIQUE_SUBFOLDER, "historique.db");
                
                var dialog = new DatabaseManagerDialog(_databaseService, databasePath);
                dialog.Owner = this;
                dialog.ShowDialog();

                // Recharger l'historique après fermeture de la fenêtre de gestion
                ChargerHistorique();
                UpdateDashboard();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de l'ouverture de la gestion de la base de données :\n{ex.Message}", 
                    "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private string GetInternetExplorerPath()
        {
            string[] candidates =
            {
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "Internet Explorer", "iexplore.exe"),
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "Internet Explorer", "iexplore.exe")
            };

            return candidates.FirstOrDefault(File.Exists) ?? string.Empty;
        }

        private void OpenMarkdownWithInternetExplorer(string target)
        {
            string iePath = GetInternetExplorerPath();
            if (string.IsNullOrWhiteSpace(iePath))
            {
                MessageBox.Show("Internet Explorer est introuvable sur ce poste.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            string resolvedTarget = target;
            if (!Uri.TryCreate(target, UriKind.Absolute, out Uri uri) || (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
            {
                if (!File.Exists(target))
                {
                    MessageBox.Show($"Le fichier demandé est introuvable : {target}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                resolvedTarget = new Uri(Path.GetFullPath(target)).AbsoluteUri;
            }

            try
            {
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = iePath,
                    Arguments = resolvedTarget,
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Impossible d'ouvrir le contenu Markdown dans Internet Explorer : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void MenuGuide_Click(object sender, RoutedEventArgs e)
        {
            // Ouvrir la fenêtre du guide d'utilisation
            ReadmeDialog dialog = new ReadmeDialog();
            dialog.Owner = this;
            dialog.ShowDialog();
        }

        private void MenuImplementationSummary_Click(object sender, RoutedEventArgs e)
        {
            string summaryPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "docs", "guides", "IMPLEMENTATION_SUMMARY.md");
            OpenMarkdownWithInternetExplorer(summaryPath);
        }

        private void MenuAbout_Click(object sender, RoutedEventArgs e)
        {
            string message = "Calculatrice de Marge\n\n" +
                           "Version : 2.2.0\n" +
                           "Développé avec WPF (.NET 10.0)\n" +
                           "Base de données : SQLite\n\n" +
                           "Application professionnelle de calcul de marge commerciale.\n\n" +
                           "Fonctionnalités principales :\n" +
                           "• Calcul automatique des marges brute et nette\n" +
                           "• Calcul inversé : déterminer le prix de vente à partir de la marge souhaitée\n" +
                           "• Gestion flexible des frais généraux (% ou €)\n" +
                           "• Remises commerciales avec recalcul instantané\n\n" +
                           "Historique & Données :\n" +
                           "• Base de données SQLite robuste et performante\n" +
                           "• Sauvegarde et restauration de l'historique\n" +
                           "• Migration automatique depuis l'ancien format\n" +
                           "• Statistiques avancées et recherche par titre\n" +
                           "• Gestionnaire intégré (Outils > Gestion base de données)\n\n" +
                           "Préconfigurations & Export :\n" +
                           "• Préconfigurations rapides (Standard, Réduit, Service)\n" +
                           "• Création de présets personnalisés\n" +
                           "• Export CSV et génération de rapports HTML\n" +
                           "• Récap rapide : Déboursé sec, Prix HT, Prix TTC\n\n" +
                           "Interface :\n" +
                           "• Interface moderne avec thème sombre/clair\n" +
                           "• Séparateurs de milliers et formatage professionnel\n" +
                           "• Undo/Redo (Ctrl+Z, Ctrl+Y)\n" +
                           "• Raccourcis clavier intuitifs\n" +
                           "• Support complet du clavier (Entrée pour calculer)\n\n" +
                           "© 2025-2026 C. Lecomte - Tous droits réservés\n" +
                           "GitHub : https://github.com/christwadel65-ux/CalculatriceMarge";
            
            MessageBox.Show(message, "À propos", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
