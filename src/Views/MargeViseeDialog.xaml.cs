using System.Windows;

namespace CalculatriceMargeWPF.Views
{
    public partial class MargeViseeDialog : Window
    {
        public double MargeNetteCible { get; private set; }
        public double? MargeBruteCible { get; private set; }
        public bool IsConfirmed { get; private set; }

        public MargeViseeDialog()
        {
            InitializeComponent();
        }

        private void BtnOK_Click(object sender, RoutedEventArgs e)
        {
            if (!double.TryParse(txtMargeNette.Text, out double margeNette))
            {
                MessageBox.Show("Entrez un pourcentage valide pour la marge nette.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (margeNette <= 0 || margeNette > 100)
            {
                MessageBox.Show("La marge nette doit être entre 0 et 100%.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            MargeNetteCible = margeNette;

            // Marge brute optionnelle
            if (!string.IsNullOrWhiteSpace(txtMargeBrute.Text))
            {
                if (double.TryParse(txtMargeBrute.Text, out double margeBrute))
                {
                    if (margeBrute < 0 || margeBrute > 100)
                    {
                        MessageBox.Show("La marge brute doit être entre 0 et 100%.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    MargeBruteCible = margeBrute;
                }
                else
                {
                    MessageBox.Show("Format invalide pour la marge brute.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }
            else
            {
                MargeBruteCible = null;
            }

            IsConfirmed = true;
            this.DialogResult = true;
            this.Close();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            IsConfirmed = false;
            this.DialogResult = false;
            this.Close();
        }
    }
}
