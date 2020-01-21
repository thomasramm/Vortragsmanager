using System;
using System.IO;
using System.Windows;
using Vortragsmanager.Core;
using Vortragsmanager.Properties;

namespace Vortragsmanager
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Exit(object sender, ExitEventArgs e)
        {
            if (DataContainer.IsInitialized)
            {
                Log.Info("ApplicationExit", "Save");
                var file = IoSqlite.SaveContainer(Settings.Default.sqlite, true);
                Settings.Default.sqlite = file;
            }

            //Alle Programm-Einstellungen die irgendwann gemacht wurden, bei Programmende speichern
            Settings.Default.Save();
        }
    }
}