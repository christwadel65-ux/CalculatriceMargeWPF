using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using CalculatriceMargeWPF.Models;
using CalculatriceMargeWPF.Views;
using CalculatriceMargeWPF.Helpers;

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
        private CalculationEngine _engine;
        private UndoRedoManager _undoRedoManager;
        private PresetManager _presetManager;
        private List<CalculationEngine.CalculationResult> _resultsList;
        private LocalizationManager _localizationManager;
        private ThemeManager _themeManager;
        private PreferencesManager _preferencesManager;
        private System.Windows.Threading.DispatcherTimer _autoSaveTimer;
        private bool _isInitialized = false;

        public MainWindow()
        {
            InitializeComponent();
            _engine = new CalculationEngine();
            _undoRedoManager = new UndoRedoManager();
            _presetManager = new PresetManager();
            _resultsList = new List<CalculationEngine.CalculationResult>();
            
            // Initialiser les gestionnaires
            _localizationManager = LocalizationManager.Instance;
            _themeManager = ThemeManager.Instance;
            _preferencesManager = PreferencesManager.Instance;
            
            // Charger les préférences
            LoadPreferences();
            
            EnsureHistoriqueFolder();
            ChargerHistorique();
            ChargerPresets();
            
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
        }

        private void ChargerHistorique()
        {
            try
            {
                if (File.Exists(historiquePath))
                {
                    var lignes = File.ReadAllLines(historiquePath);
                    lstHistorique.Items.Clear();
                    foreach (var ligne in lignes)
                    {
                        if (!string.IsNullOrWhiteSpace(ligne))
                        {
                            lstHistorique.Items.Add(ligne);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors du chargement de l'historique:\n{ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void btnCalculer_Click(object sender, RoutedEventArgs e)
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
                
                // Sauvegarder dans l'historique
                try
                {
                    string ligneHistorique = result.ToString();
                    
                    if (indexHistorique >= 0)
                    {
                        // Mettre à jour la ligne existante
                        lstHistorique.Items[indexHistorique] = ligneHistorique;
                    }
                    else
                    {
                        // Ajouter nouvelle ligne
                        lstHistorique.Items.Add(ligneHistorique);
                    }
                    
                    // Réécrire tout l'historique
                    var toutesLignes = lstHistorique.Items.Cast<string>().ToList();
                    File.WriteAllLines(historiquePath, toutesLignes, System.Text.Encoding.UTF8);
                }
                catch (Exception exHist)
                {
                    MessageBox.Show($"Erreur lors de la sauvegarde dans l'historique:\n{exHist.Message}", "Avertissement", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
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

        private void btnStatistiques_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_resultsList.Count == 0)
                {
                    MessageBox.Show("Aucun calcul pour les statistiques.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                var stats = _engine.ComputeStatistics(_resultsList.ToArray());

                string message = $"STATISTIQUES ({stats.NombreCalculs} calculs)\n\n" +
                    $"Chiffre d'affaires TTC: {stats.ChiffreAffairesTotal:F2}€\n" +
                    $"Marge nette totale: {stats.MargeNetteEuroTotal:F2}€\n\n" +
                    $"MARGE BRUTE\n" +
                    $"Moyenne: {stats.MoyenneMargeBrutePct:F2}%\n" +
                    $"Min - Max: {stats.MinMargeBrutePct:F2}% - {stats.MaxMargeBrutePct:F2}%\n" +
                    $"Écart-type: {stats.EcartTypeMargeBrutePct:F2}%\n\n" +
                    $"MARGE NETTE\n" +
                    $"Moyenne: {stats.MoyenneMargeNettePct:F2}%\n" +
                    $"Min - Max: {stats.MinMargeNettePct:F2}% - {stats.MaxMargeNettePct:F2}%\n" +
                    $"Écart-type: {stats.EcartTypeMargeNettePct:F2}%";

                MessageBox.Show(message, "Statistiques", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur calcul statistiques:\n{ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
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
            btnClearHistory.Visibility = lstHistorique.Visibility;
            btnDeleteSelected.Visibility = lstHistorique.Visibility;
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
                lstHistorique.Items.Clear();
                if (File.Exists(historiquePath)) File.WriteAllText(historiquePath, string.Empty);
                MessageBox.Show("Historique nettoyé avec succès.", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void btnDeleteSelected_Click(object sender, RoutedEventArgs e)
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
                    lstHistorique.Items.Remove(lstHistorique.SelectedItem);
                    
                    // Réécrire le fichier historique avec les éléments restants
                    if (lstHistorique.Items.Count > 0)
                    {
                        StringBuilder sb = new StringBuilder();
                        foreach (var item in lstHistorique.Items)
                        {
                            sb.AppendLine(item.ToString());
                        }
                        File.WriteAllText(historiquePath, sb.ToString());
                    }
                    else
                    {
                        // Si vide, créer un fichier vide
                        File.WriteAllText(historiquePath, string.Empty);
                    }
                    
                    // Réinitialiser les champs de la calculatrice
                    btnReset_Click(null, null);
                    
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

            string item = lstHistorique.SelectedItem.ToString();
            
            // Utiliser le parser d'historique pour extraire les valeurs
            if (!HistoryParser.TryParseHistoryLine(item, out var entry))
            {
                MessageBox.Show("Impossible de charger ce calcul.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                txtTitre.Text = entry.Titre;
                txtDebourse.Text = entry.DebourseSec.ToString("F2");
                txtFrais.Text = entry.FraisGeneraux.ToString("F2");
                txtVente.Text = entry.PrixVente.ToString("F2");
                txtTVA.Text = entry.TVA.ToString("F2");
                cmbFraisMode.SelectedIndex = entry.FraisModeIndex;

                MessageBox.Show("Calcul rechargé depuis l'historique. Cliquez sur 'Calculer' pour voir les résultats.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors du chargement : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool TryLireSaisies(out double debourseSec, out double prixVenteHT, out double tva, out double fraisGenerauxPct, out bool fraisEnPourcent)
        {
            debourseSec = prixVenteHT = tva = fraisGenerauxPct = 0;
            fraisEnPourcent = cmbFraisMode.SelectedIndex == 0;

            // Parser les valeurs avec gestion des champs vides
            bool deburseOk = string.IsNullOrWhiteSpace(txtDebourse.Text) || NumberFormatter.TryParseFormattedNumber(txtDebourse.Text, out debourseSec);
            bool venteOk = string.IsNullOrWhiteSpace(txtVente.Text) || NumberFormatter.TryParseFormattedNumber(txtVente.Text, out prixVenteHT);
            bool tvaOk = string.IsNullOrWhiteSpace(txtTVA.Text) || NumberFormatter.TryParseFormattedNumber(txtTVA.Text, out tva);
            bool fraisOk = string.IsNullOrWhiteSpace(txtFrais.Text) || NumberFormatter.TryParseFormattedNumber(txtFrais.Text, out fraisGenerauxPct);

            // Si les champs vides, utiliser valeurs par défaut
            if (string.IsNullOrWhiteSpace(txtTVA.Text)) tva = 20;
            if (string.IsNullOrWhiteSpace(txtFrais.Text)) fraisGenerauxPct = 25;

            if (!deburseOk || !venteOk || !tvaOk || !fraisOk)
            {
                MessageBox.Show("Veuillez entrer des valeurs numériques valides.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            // Vérifier qu'au moins un champ essentiel est rempli
            if (debourseSec == 0 && prixVenteHT == 0)
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
            // Ouvrir le guide local (README.md) dans Internet Explorer
            string readmePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "docs", "guides", "README.md");
            OpenMarkdownWithInternetExplorer(readmePath);
        }

        private void MenuImplementationSummary_Click(object sender, RoutedEventArgs e)
        {
            string summaryPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "docs", "guides", "IMPLEMENTATION_SUMMARY.md");
            OpenMarkdownWithInternetExplorer(summaryPath);
        }

        private void MenuAbout_Click(object sender, RoutedEventArgs e)
        {
            string message = "Calculatrice de Marge\n\n" +
                           "Version : 2.1\n" +
                           "Développé avec WPF (.NET 10)\n\n" +
                           "Application professionnelle de calcul de marge commerciale.\n\n" +
                           "Fonctionnalités :\n" +
                           "• Calcul automatique des marges brute et nette\n" +
                           "• Calcul inversé : déterminer le prix de vente à partir de la marge souhaitée\n" +
                           "• Gestion flexible des frais généraux (% ou €)\n" +
                           "• Historique intelligent avec rechargement et rétrocompatibilité\n" +
                           "• Préconfigurations rapides (Standard, Réduit, Service)\n" +
                           "• Récap rapide : Déboursé sec, Prix HT, Prix TTC\n" +
                           "• Statistiques détaillées et analyses\n" +
                           "• Export CSV et génération de rapports HTML\n" +
                           "• Undo/Redo (Ctrl+Z, Ctrl+Y)\n" +
                           "• Interface moderne avec thème sombre/clair\n" +
                           "• Séparateurs de milliers et formatage professionnel\n\n" +
                           "© 2025 C. Lecomte - Tous droits réservés";
            
            MessageBox.Show(message, "À propos", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
