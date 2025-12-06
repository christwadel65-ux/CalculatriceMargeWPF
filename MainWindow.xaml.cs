using Microsoft.Win32;
using System;
using System.IO;
using System.Text;
using System.Windows;

namespace CalculatriceMargeWPF
{
    public partial class MainWindow : Window
    {
        private string historiquePath = "Historique/historique.txt";

        public MainWindow()
        {
            InitializeComponent();
            EnsureHistoriqueFolder();
            ChargerHistorique();
            
            // Initialiser les valeurs par défaut
            txtDebourse.Text = "0";
            txtTVA.Text = "20";
            txtFrais.Text = "10";
            cmbFraisMode.SelectedIndex = 0;
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
                if (!TryLireSaisies(out double debourseSec, out double prixVenteHT, out double tva, out double fraisGenerauxPct, out bool fraisEnPourcent)) return;

                double fraisEnEuro = fraisEnPourcent ? debourseSec * (fraisGenerauxPct / 100) : fraisGenerauxPct;
                double prixRevientTotal = debourseSec + fraisEnEuro;
                txtRevient.Text = prixRevientTotal.ToString("F2");

                double prixTTC = prixVenteHT * (1 + tva / 100);

                double margeBruteEuro = prixVenteHT - debourseSec;
                double margeNetteEuro = prixVenteHT - prixRevientTotal;

                double margeBrutePct = prixVenteHT == 0 ? 0 : (margeBruteEuro / prixVenteHT) * 100;
                double margeNettePct = prixVenteHT == 0 ? 0 : (margeNetteEuro / prixVenteHT) * 100;

                txtRecapPR.Text = prixRevientTotal.ToString("C");
                txtRecapHT.Text = prixVenteHT.ToString("C");
                txtRecapTTC.Text = prixTTC.ToString("C");
                
                // Mise à jour des résultats détaillés
                txtPrixRevient.Text = prixRevientTotal.ToString("C");
                txtMargeBrute.Text = $"{margeBruteEuro:C} ({margeBrutePct:F2}%)";
                txtMargeNette.Text = $"{margeNetteEuro:C} ({margeNettePct:F2}%)";

                string titre = string.IsNullOrWhiteSpace(txtTitre.Text) ? "Sans titre" : txtTitre.Text;
                
                // Vérifier si un calcul avec ce titre existe déjà
                bool titreExiste = false;
                foreach (var item in lstHistorique.Items)
                {
                    string itemStr = item.ToString();
                    if (itemStr.Contains($"| {titre} |"))
                    {
                        titreExiste = true;
                        break;
                    }
                }

                if (!titreExiste)
                {
                    string modeFreais = fraisEnPourcent ? "%" : "EUR";
                    string ligneHistorique = $"{DateTime.Now:dd/MM/yyyy HH:mm:ss} | {titre} | DS:{debourseSec:N2} | FG:{fraisGenerauxPct:N2} | MODE:{modeFreais} | PR:{prixRevientTotal:C} | HT:{prixVenteHT:C} | TTC:{prixTTC:C} | TVA:{tva:F2}% | MB:{margeBruteEuro:C} ({margeBrutePct:F2}%) | MN:{margeNetteEuro:C} ({margeNettePct:F2}%)";
                    lstHistorique.Items.Add(ligneHistorique);
                    File.AppendAllText(historiquePath, ligneHistorique + Environment.NewLine);
                }
                else
                {
                    MessageBox.Show($"Un calcul avec le titre \"{titre}\" existe déjà dans l'historique.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors du calcul :\n{ex.Message}\n\nDétails :\n{ex.StackTrace}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
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
            txtRecapPR.Text = string.Empty;
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
                           "Version : 1.0.5\n" +
                           "Développé avec WPF (.NET 10)\n\n" +
                           "Application professionnelle de calcul de marge commerciale.\n\n" +
                           "Fonctionnalités :\n" +
                           "• Calcul automatique des marges brute et nette\n" +
                           "• Gestion flexible des frais généraux (% ou €)\n" +
                           "• Historique intelligent avec rechargement\n" +
                           "• Préconfigurations rapides (Standard, Réduit, Service)\n" +
                           "• Export et sauvegarde automatique\n" +
                           "• Interface moderne avec thème sombre/clair\n" +
                           "• Séparateurs de milliers et formatage professionnel\n\n" +
                           "© 2025 C. Lecomte - Tous droits réservés";
            
            MessageBox.Show(message, "À propos", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
