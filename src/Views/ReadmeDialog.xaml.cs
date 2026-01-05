using System;
using System.IO;
using System.Reflection;
using System.Windows;

namespace CalculatriceMargeWPF.Views
{
    /// <summary>
    /// Interaction logic for ReadmeDialog.xaml
    /// </summary>
    public partial class ReadmeDialog : Window
    {
        public ReadmeDialog()
        {
            InitializeComponent();
            LoadReadmeContent();
        }

        private void LoadReadmeContent()
        {
            try
            {
                // Charger le README.md depuis les ressources embarquées
                var assembly = Assembly.GetExecutingAssembly();
                var resourceName = "CalculatriceMargeWPF.README.md";

                using (Stream stream = assembly.GetManifestResourceStream(resourceName))
                {
                    if (stream == null)
                    {
                        readmeContent.Text = "Erreur : Impossible de charger le guide d'utilisation.";
                        return;
                    }

                    using (StreamReader reader = new StreamReader(stream))
                    {
                        string content = reader.ReadToEnd();
                        // Nettoyer le contenu Markdown pour l'affichage en TextBlock
                        readmeContent.Text = CleanMarkdown(content);
                    }
                }
            }
            catch (Exception ex)
            {
                readmeContent.Text = $"Erreur lors du chargement du guide : {ex.Message}";
            }
        }

        private string CleanMarkdown(string content)
        {
            // Supprimer les symboles Markdown courants pour un meilleur affichage
            content = System.Text.RegularExpressions.Regex.Replace(content, @"#{1,6}\s+", "");  // En-têtes
            content = System.Text.RegularExpressions.Regex.Replace(content, @"\*\*(.+?)\*\*", "$1");  // Gras
            content = System.Text.RegularExpressions.Regex.Replace(content, @"__(.+?)__", "$1");  // Gras alternatif
            content = System.Text.RegularExpressions.Regex.Replace(content, @"\*(.+?)\*", "$1");  // Italique
            content = System.Text.RegularExpressions.Regex.Replace(content, @"_(.+?)_", "$1");  // Italique alternatif
            content = System.Text.RegularExpressions.Regex.Replace(content, @"`(.+?)`", "$1");  // Code inline
            content = System.Text.RegularExpressions.Regex.Replace(content, @"```[\s\S]*?```", "");  // Blocs de code
            content = System.Text.RegularExpressions.Regex.Replace(content, @"- ✅", "✓");  // Puces
            content = System.Text.RegularExpressions.Regex.Replace(content, @"^- ", "", System.Text.RegularExpressions.RegexOptions.Multiline);  // Puces
            content = System.Text.RegularExpressions.Regex.Replace(content, @"^\d+\.\s+", "", System.Text.RegularExpressions.RegexOptions.Multiline);  // Listes numérotées
            content = System.Text.RegularExpressions.Regex.Replace(content, @"\[(.+?)\]\(.+?\)", "$1");  // Liens
            content = System.Text.RegularExpressions.Regex.Replace(content, @"_{3,}", "");  // Séparateurs
            
            return content.Trim();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
