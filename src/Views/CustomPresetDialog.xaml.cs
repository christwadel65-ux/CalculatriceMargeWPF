using System.Windows;
using CalculatriceMargeWPF.Helpers;

namespace CalculatriceMargeWPF.Views
{
    public partial class CustomPresetDialog : Window
    {
        public double TVA { get; private set; }
        public double FraisGeneraux { get; private set; }
        public bool FraisEnPourcent { get; private set; }

        public CustomPresetDialog(string tvaInitial, string fraisInitial, bool fraisEnPourcentInitial)
        {
            InitializeComponent();
            txtTva.Text = tvaInitial;
            txtFrais.Text = fraisInitial;
            cmbMode.SelectedIndex = fraisEnPourcentInitial ? 0 : 1;
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            if (!NumberFormatter.TryParseFormattedNumber(txtTva.Text, out double tva))
            {
                MessageBox.Show("Saisissez une TVA valide.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!NumberFormatter.TryParseFormattedNumber(txtFrais.Text, out double frais))
            {
                MessageBox.Show("Saisissez des frais généraux valides.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            TVA = tva;
            FraisGeneraux = frais;
            FraisEnPourcent = cmbMode.SelectedIndex == 0;
            DialogResult = true;
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
