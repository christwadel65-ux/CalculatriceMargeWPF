using System;
using System.Diagnostics;
using System.Windows;

namespace CalculatriceMargeWPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
            // Fermer le processus de l'application
            Process.GetCurrentProcess().Kill();
        }
    }
}
