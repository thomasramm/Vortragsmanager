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
                var file = IoSqlite.SaveContainer(Settings.Default.sqlite);

                //Aktuell wird jedes mal eine Sicherheitskopie erstellt
                var fi = new FileInfo(file);
                var backup = fi.DirectoryName + "\\" + fi.Name.Substring(0, fi.Name.Length - fi.Extension.Length) + $"_{DateTime.Now:yyyy-MM-dd-hh-mm}" + fi.Extension;
                File.Copy(file, backup, true);
            }
        }
    }
}