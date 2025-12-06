namespace CalculatriceMargeMAUI;

public partial class MainPage : ContentPage
{
	public MainPage()
	{
		InitializeComponent();
	}

	private void OnCalculerClicked(object sender, EventArgs e)
	{
		try
		{
			if (!TryLireSaisies(out double debourseSec, out double prixVenteHT, out double tva, out double fraisGenerauxPct, out bool fraisEnPourcent))
				return;

			double fraisEnEuro = fraisEnPourcent ? debourseSec * (fraisGenerauxPct / 100) : fraisGenerauxPct;
			double prixRevientTotal = debourseSec + fraisEnEuro;

			double prixTTC = prixVenteHT * (1 + tva / 100);

			double margeBruteEuro = prixVenteHT - debourseSec;
			double margeNetteEuro = prixVenteHT - prixRevientTotal;

			double margeBrutePct = prixVenteHT == 0 ? 0 : (margeBruteEuro / prixVenteHT) * 100;
			double margeNettePct = prixVenteHT == 0 ? 0 : (margeNetteEuro / prixVenteHT) * 100;

			// Mise à jour Récap rapide
			txtRecapDS.Text = debourseSec.ToString("C");
			txtRecapHT.Text = prixVenteHT.ToString("C");
			txtRecapTTC.Text = prixTTC.ToString("C");

			// Mise à jour Résultats
			txtPrixRevient.Text = prixRevientTotal.ToString("C");
			txtMargeBrute.Text = $"{margeBruteEuro:C} ({margeBrutePct:F2}%)";
			txtMargeNette.Text = $"{margeNetteEuro:C} ({margeNettePct:F2}%)";

			MainThread.BeginInvokeOnMainThread(async () =>
			{
				await DisplayAlert("Succès", $"Calcul effectué\nMarge nette: {margeNetteEuro:C}", "OK");
			});
		}
		catch (Exception ex)
		{
			MainThread.BeginInvokeOnMainThread(async () =>
			{
				await DisplayAlert("Erreur", $"Erreur lors du calcul:\n{ex.Message}", "OK");
			});
		}
	}

	private void OnResetClicked(object sender, EventArgs e)
	{
		txtTitre.Text = "";
		txtDebourse.Text = "";
		txtVente.Text = "";
		txtTVA.Text = "20";
		txtFrais.Text = "10";
		cmbFraisMode.SelectedIndex = 0;

		txtRecapDS.Text = "-";
		txtRecapHT.Text = "-";
		txtRecapTTC.Text = "-";
		txtPrixRevient.Text = "-";
		txtMargeBrute.Text = "-";
		txtMargeNette.Text = "-";
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
			MainThread.BeginInvokeOnMainThread(async () =>
			{
				await DisplayAlert("Erreur", "Veuillez entrer des valeurs numériques valides.", "OK");
			});
			return false;
		}

		if (prixVenteHT <= 0)
		{
			MainThread.BeginInvokeOnMainThread(async () =>
			{
				await DisplayAlert("Erreur", "Le prix de vente HT doit être supérieur à zéro.", "OK");
			});
			return false;
		}

		return true;
	}
}
