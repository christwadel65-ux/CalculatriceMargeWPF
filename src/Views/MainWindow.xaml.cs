using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using CalculatriceMargeWPF.Models;

namespace CalculatriceMargeWPF
{
    public partial class MainWindow : Window
    {
        private string historiquePath = "Historique/historique.txt";
        private CalculationEngine _engine;
        private UndoRedoManager _undoRedoManager;
        private PresetManager _presetManager;
        private List<CalculationEngine.CalculationResult> _resultsList;

        public MainWindow()
        {
            InitializeComponent();
            _engine = new CalculationEngine();
            _undoRedoManager = new UndoRedoManager();
            _presetManager = new PresetManager();
            _resultsList = new List<CalculationEngine.CalculationResult>();
            
            EnsureHistoriqueFolder();
            ChargerHistorique();
            ChargerPresets();
            
            // Initialiser les valeurs par défaut
            // txtDebourse et txtVente restent vides par défaut
            txtTVA.Text = "20";
            txtFrais.Text = "10";
            cmbFraisMode.SelectedIndex = 0;
            cmbPresets.SelectedIndex = 0;
            
            // Vider explicitement les champs
            txtDebourse.Clear();
            txtVente.Clear();
            
            // Support clavier
            this.KeyDown += MainWindow_KeyDown;
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
        }

        private void ChargerPresets()
        {
            cmbPresets.Items.Clear();
            cmbPresets.Items.Add("Standard (20% TVA, 10% FG)");
            cmbPresets.Items.Add("Réduit (5.5% TVA, 8% FG)");
            cmbPresets.Items.Add("Service (10% TVA, 15% FG)");
            cmbPresets.SelectedIndex = 0;
        }

        private void EnsureHistoriqueFolder()
        {
            // Utiliser AppData pour éviter les problèmes de permissions dans Program Files
            string appDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string dossier = Path.Combine(appDataFolder, "CalculatriceMarge", "Historique");
            if (!Directory.Exists(dossier))
            {
                Directory.CreateDirectory(dossier);
            }
            historiquePath = Path.Combine(dossier, "historique.txt");
        }

        private void ChargerHistorique()
        {
            if (File.Exists(historiquePath))
            {
                var lignes = File.ReadAllLines(historiquePath);
                foreach (var ligne in lignes)
                {
                    lstHistorique.Items.Add(ligne);
                }
            }
        }

        private void btnCalculer_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!TryLireSaisies(out double debourseSec, out double prixVenteHT, out double tva, out double fraisGenerauxPct, out bool fraisEnPourcent)) 
                    return;

                // Utiliser le CalculationEngine
                var result = _engine.Calculate(debourseSec, prixVenteHT, tva, fraisGenerauxPct, fraisEnPourcent);
                
                // Récupérer le titre
                result.Titre = string.IsNullOrWhiteSpace(txtTitre.Text) ? "Sans titre" : txtTitre.Text;
                
                // Vérifier les doublons
                bool titreExiste = _resultsList.Any(r => r.Titre == result.Titre);
                
                if (!titreExiste)
                {
                    // Ajouter aux résultats
                    _resultsList.Add(result);
                    _undoRedoManager.Push(result);
                    
                    // Afficher les résultats
                    AfficherResultats(result);
                    
                    // Sauvegarder dans l'historique
                    lstHistorique.Items.Add(result.ToString());
                    File.AppendAllText(historiquePath, result.ToString() + Environment.NewLine);
                }
                else
                {
                    MessageBox.Show($"Un calcul avec le titre \"{result.Titre}\" existe déjà.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
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
                if (!double.TryParse(txtVente.Text, out double prixVente) || prixVente <= 0)
                {
                    MessageBox.Show("Entrez un prix de vente valide.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (!double.TryParse(txtTVA.Text, out double tva))
                {
                    MessageBox.Show("Entrez une TVA valide.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Demander la marge nette cible
                var dialog = new System.Windows.Controls.TextBlock { Text = "Marge nette cible (%): " };
                var input = new System.Windows.Controls.TextBox { Width = 100 };
                input.Text = "25";

                if (!double.TryParse(input.Text, out double margeTarget))
                {
                    MessageBox.Show("Entrez un pourcentage valide.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                bool fraisEnPourcent = cmbFraisMode.SelectedIndex == 0;
                if (!double.TryParse(txtFrais.Text, out double fraisGeneraux))
                    fraisGeneraux = 10;

                var result = _engine.CalculateInverse(prixVente, margeTarget, tva, fraisEnPourcent, fraisGeneraux);
                result.Titre = string.IsNullOrWhiteSpace(txtTitre.Text) ? "Sans titre (inversé)" : $"{txtTitre.Text} (inversé)";

                _resultsList.Add(result);
                _undoRedoManager.Push(result);
                AfficherResultats(result);

                txtDebourse.Text = result.DebourseSec.ToString("F2");
                MessageBox.Show($"Calcul inversé:\nDéboursé sec max: {result.DebourseSec:F2}€", "Résultat", MessageBoxButton.OK, MessageBoxImage.Information);
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
                    ExportManager.ExportToHTML(stats, _resultsList, dialog.FileName);
                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo { FileName = dialog.FileName, UseShellExecute = true });
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
            
            // Parse la ligne d'historique - gère l'ancien format (sans MODE) et le nouveau (avec MODE)
            var parts = item.Split('|');
            if (parts.Length < 8) return;

            try
            {
                // Extraire le titre
                string titre = parts[1].Trim();
                txtTitre.Text = titre;
                
                // Extraire DS (déboursé sec)
                string dsPart = parts[2].Trim().Replace("DS:", "").Trim();
                if (double.TryParse(dsPart, out double ds))
                {
                    txtDebourse.Text = ds.ToString("F2");
                }
                
                // Extraire FG (frais généraux)
                string fgPart = parts[3].Trim().Replace("FG:", "").Trim();
                double fg = 0;
                if (double.TryParse(fgPart, out fg))
                {
                    // Vérifier si c'est du nouveau format (avec MODE) ou ancien (sans MODE)
                    string part4Content = parts[4].Trim();
                    
                    if (part4Content.StartsWith("MODE:"))
                    {
                        // Nouveau format : FG est la valeur brute (25 pour 25%)
                        txtFrais.Text = fg.ToString("F2");
                        
                        // Extraire MODE
                        string modePart = part4Content.Replace("MODE:", "").Trim();
                        if (modePart == "%")
                        {
                            cmbFraisMode.SelectedIndex = 0;
                        }
                        else if (modePart == "EUR")
                        {
                            cmbFraisMode.SelectedIndex = 1;
                        }
                        
                        // Extraire HT depuis parts[6]
                        string htPart = parts[6].Trim().Replace("HT:", "").Trim();
                        htPart = System.Text.RegularExpressions.Regex.Replace(htPart, @"[^\d,\.]", "");
                        htPart = htPart.Replace(",", ".");
                        if (double.TryParse(htPart, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out double ht))
                        {
                            txtVente.Text = ht.ToString("F2");
                        }
                        
                        // Extraire TVA depuis parts[8]
                        string tvaPart = parts[8].Trim().Replace("TVA:", "").Replace("%", "").Trim();
                        if (double.TryParse(tvaPart, out double tva))
                        {
                            txtTVA.Text = tva.ToString("F2");
                        }
                    }
                    else if (part4Content.StartsWith("PR:"))
                    {
                        // Ancien format (pas de MODE) : FG est la valeur en euros, on doit retrouver le %
                        // On récupère DS et on calcule le % : fg_pct = (fg / ds) * 100
                        if (ds > 0)
                        {
                            double fgPct = (fg / ds) * 100;
                            txtFrais.Text = fgPct.ToString("F2");
                            cmbFraisMode.SelectedIndex = 0; // Mode % par défaut pour ancien format
                        }
                        else
                        {
                            txtFrais.Text = fg.ToString("F2");
                        }
                        
                        // Extraire HT depuis parts[5]
                        string htPart = parts[5].Trim().Replace("HT:", "").Trim();
                        htPart = System.Text.RegularExpressions.Regex.Replace(htPart, @"[^\d,\.]", "");
                        htPart = htPart.Replace(",", ".");
                        if (double.TryParse(htPart, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out double ht))
                        {
                            txtVente.Text = ht.ToString("F2");
                        }
                        
                        // Extraire TVA depuis parts[7]
                        string tvaPart = parts[7].Trim().Replace("TVA:", "").Replace("%", "").Trim();
                        if (double.TryParse(tvaPart, out double tva))
                        {
                            txtTVA.Text = tva.ToString("F2");
                        }
                    }
                }

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

            bool saisiesValides = double.TryParse(txtDebourse.Text, out debourseSec)
                                 && double.TryParse(txtVente.Text, out prixVenteHT)
                                 && double.TryParse(txtTVA.Text, out tva)
                                 && double.TryParse(txtFrais.Text, out fraisGenerauxPct);

            if (!saisiesValides)
            {
                MessageBox.Show("Veuillez entrer des valeurs numériques valides.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            if (prixVenteHT <= 0)
            {
                MessageBox.Show("Le prix de vente HT doit être supérieur à zéro pour calculer les marges.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
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
            mnuThemeToggle.Header = isDarkMode ? "Mode clair" : "Mode sombre";
            ApplyTheme();
        }

        private void ApplyTheme()
        {
            if (isDarkMode)
            {
                this.Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 15, 20, 25));
                this.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 232, 238, 247));
            }
            else
            {
                this.Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 230, 234, 242));
                this.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 28, 42, 68));
            }
        }

        private void MenuGuide_Click(object sender, RoutedEventArgs e)
        {
            string readmePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "README.md");
            
            if (File.Exists(readmePath))
            {
                try
                {
                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = readmePath,
                        UseShellExecute = true
                    });
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Impossible d'ouvrir le guide : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Le fichier README.md n'a pas été trouvé.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void MenuAbout_Click(object sender, RoutedEventArgs e)
        {
            string message = "Calculatrice de Marge\n\n" +
                           "Version : 1.0.6\n" +
                           "Développé avec WPF (.NET 10)\n\n" +
                           "Application professionnelle de calcul de marge commerciale.\n\n" +
                           "Fonctionnalités :\n" +
                           "• Calcul automatique des marges brute et nette\n" +
                           "• Gestion flexible des frais généraux (% ou €)\n" +
                           "• Historique intelligent avec rechargement et rétrocompatibilité\n" +
                           "• Préconfigurations rapides (Standard, Réduit, Service)\n" +
                           "• Récap rapide : Déboursé sec, Prix HT, Prix TTC\n" +
                           "• Export et sauvegarde automatique avec permissions sécurisées\n" +
                           "• Interface moderne avec thème sombre/clair\n" +
                           "• Séparateurs de milliers et formatage professionnel\n\n" +
                           "© 2025 C. Lecomte - Tous droits réservés";
            
            MessageBox.Show(message, "À propos", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
