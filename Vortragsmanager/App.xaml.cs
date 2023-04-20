using System.Windows;
using Vortragsmanager.DataModels;
using Vortragsmanager.Module;

namespace Vortragsmanager
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        private void Application_Exit(object sender, ExitEventArgs e)
        {
            if (DataContainer.IsInitialized)
            {
                Log.Info("ApplicationExit", "Save");
                var file = IoSqlite.SaveContainer(Helper.Helper.GlobalSettings.sqlite, Helper.Helper.GlobalSettings.SaveBackups);
                Helper.Helper.GlobalSettings.sqlite = file;
            }

            //Alle Programm-Einstellungen die irgendwann gemacht wurden, bei Programmende speichern
            Helper.Helper.GlobalSettings.Save();
        }
    }
}