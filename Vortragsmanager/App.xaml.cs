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
            //speichern unter neuem Dateiname
            string file = Settings.Default.sqlite;
            if (!file.Contains("anonymisiert"))
            {
                FileInfo fi = new FileInfo(file);
                file = fi.DirectoryName + @"\" + fi.Name.Replace(fi.Extension, "_anonymisiert" + fi.Extension);
                Settings.Default.sqlite = file;
                Settings.Default.Save();
            }
            if (DataContainer.IsInitialized)
            {
                Log.Info("ApplicationExit", "Save");
                file = IoSqlite.SaveContainer(Settings.Default.sqlite, Settings.Default.SaveBackups);
                Settings.Default.sqlite = file;
            }

            //Alle Programm-Einstellungen die irgendwann gemacht wurden, bei Programmende speichern
            Settings.Default.Save();
        }
    }
}