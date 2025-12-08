using System;
using System.Globalization;
using System.Windows.Controls;

namespace CalculatriceMargeWPF.Helpers
{
    /// <summary>
    /// Helper pour formater les nombres avec séparateurs de milliers
    /// </summary>
    public static class NumberFormatter
    {
        /// <summary>
        /// Configure un TextBox pour accepter et afficher les nombres avec séparateurs de milliers
        /// </summary>
        public static void SetupThousandsSeparatorTextBox(TextBox textBox)
        {
            textBox.PreviewTextInput += (s, e) =>
            {
                // Permettre les chiffres, virgule, point, tiret
                e.Handled = !IsValidNumberChar(e.Text);
            };

            textBox.TextChanged += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(textBox.Text))
                    return;

                // Essayer de parser et reformater
                string cleanedText = CleanNumberInput(textBox.Text);
                if (double.TryParse(cleanedText, CultureInfo.InvariantCulture, out double value))
                {
                    // Formater avec séparateurs de milliers
                    string formatted = FormatNumber(value);
                    if (formatted != textBox.Text)
                    {
                        int cursorPos = textBox.SelectionStart;
                        textBox.Text = formatted;
                        // Repositionner le curseur
                        textBox.SelectionStart = Math.Min(cursorPos, textBox.Text.Length);
                    }
                }
            };
        }

        /// <summary>
        /// Nettoie une entrée utilisateur en supprimant les séparateurs
        /// </summary>
        public static string CleanNumberInput(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return "";

            // Supprimer les espaces et garder le point/virgule
            input = input.Replace(" ", "").Trim();
            
            // Si la dernière partie commence par une virgule/point, c'est la décimale
            // Exemple: 1 234,56 ou 1,234.56
            if (input.Contains(",") && input.Contains("."))
            {
                // Déterminer lequel est le séparateur de milliers
                int lastComma = input.LastIndexOf(",");
                int lastDot = input.LastIndexOf(".");
                
                if (lastDot > lastComma)
                {
                    // Format: 1,234.56 (point = décimale)
                    input = input.Replace(",", "");
                }
                else
                {
                    // Format: 1.234,56 (virgule = décimale)
                    input = input.Replace(".", "").Replace(",", ".");
                }
            }
            else if (input.Contains(","))
            {
                // Vérifier si c'est un séparateur de milliers ou décimal
                // Si 3 chiffres après la virgule, c'est un millier
                int lastCommaPos = input.LastIndexOf(",");
                int digitsAfter = input.Length - lastCommaPos - 1;
                
                if (digitsAfter > 3)
                {
                    // Séparateur de milliers
                    input = input.Replace(",", "");
                }
                else
                {
                    // Décimal
                    input = input.Replace(",", ".");
                }
            }

            return input;
        }

        /// <summary>
        /// Formate un nombre avec séparateurs de milliers
        /// </summary>
        public static string FormatNumber(double value)
        {
            // Formater avec 2 décimales et séparateurs d'espaces pour les milliers
            return value.ToString("#,##0.##", new CultureInfo("fr-FR"));
        }

        /// <summary>
        /// Vérifie si un caractère est valide pour une entrée numérique
        /// </summary>
        private static bool IsValidNumberChar(string text)
        {
            if (string.IsNullOrEmpty(text))
                return false;

            // Accepter les chiffres, la virgule, le point, le tiret
            foreach (char c in text)
            {
                if (!char.IsDigit(c) && c != ',' && c != '.' && c != '-')
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Parse un nombre à partir d'une entrée utilisateur formatée
        /// </summary>
        public static bool TryParseFormattedNumber(string input, out double value)
        {
            value = 0;
            if (string.IsNullOrWhiteSpace(input))
                return false;

            string cleaned = CleanNumberInput(input);
            return double.TryParse(cleaned, CultureInfo.InvariantCulture, out value);
        }
    }
}
