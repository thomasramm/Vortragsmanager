using DevExpress.Xpf.Core;
using System;
using System.ComponentModel;
using System.Net;
using System.Reflection;
using Vortragsmanager.Views;

namespace Vortragsmanager.Core
{
    internal static class Updater
    {
        private static readonly BackgroundWorker _updateWorker = new BackgroundWorker();
        internal static bool _silent = true;

        public static Version LocalVersion { get; set; }

        public static Version ServerVersion { get; set; }

        public static Ini ServerVersions { get; set; }

        public static void CheckForUpdates()
        {
            if (!Properties.Settings.Default.SearchForUpdates)
                return;

            var nextSearch = Properties.Settings.Default.NextUpdateSearch;
            if (nextSearch > DateTime.Today)
                return;

            CheckForUpdatesForce(true);
        }

        public static void CheckForUpdatesForce(bool silent)
        {
            _silent = silent;
            var nextSearch = Properties.Settings.Default.NextUpdateSearch;
            nextSearch = DateTime.Today.AddDays(1);
            Properties.Settings.Default.NextUpdateSearch = nextSearch;
            Properties.Settings.Default.Save();

            LocalVersion = Assembly.GetEntryAssembly().GetName().Version;

            _updateWorker.DoWork += new DoWorkEventHandler(UpdaterDoWork);
            _updateWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(UpdaterFinished);
            _updateWorker.RunWorkerAsync();
        }

        private static void UpdaterDoWork(object sender, DoWorkEventArgs e)
        {
            var ini = ReadNewestVersion();
            e.Result = ini;
        }

        private static void UpdaterFinished(object sender, RunWorkerCompletedEventArgs e)
        {
            _updateWorker.DoWork -= new DoWorkEventHandler(UpdaterDoWork);
            _updateWorker.RunWorkerCompleted -= new RunWorkerCompletedEventHandler(UpdaterFinished);
            if (ServerVersion == null)
                return;
            if (LocalVersion >= ServerVersion)
            {
                if (!_silent)
                    ThemedMessageBox.Show("Information",
                        "Neueste Version ist bereits installiert",
                        System.Windows.MessageBoxButton.OK,
                        System.Windows.MessageBoxImage.Information);
                return;
            }

            ServerVersions = (Ini)e.Result;

            var w = new UpdateDialog();
            var data = (UpdateDialogViewModel)w.DataContext;
            data.LocalVersion = LocalVersion;
            data.ServerVersion = ServerVersion;
            data.ServerIni = ServerVersions;
            w.ShowDialog();
        }

        private static Ini ReadNewestVersion()
        {
            var iniString = string.Empty;

            try
            {
                using (WebClient client = new WebClient())
                {
                    iniString = client.DownloadString("http://thomas-ramm.de/Vortragsmanager/version.ini");
                }
                if (string.IsNullOrEmpty(iniString))
                {
                    if (!_silent)
                        ThemedMessageBox.Show(Properties.Resources.Achtung,
                            "Fehler beim suchen nach der neuesten Version. Kein Zugriff auf Webseite",
                            System.Windows.MessageBoxButton.OK,
                            System.Windows.MessageBoxImage.Error);
                    return null;
                }
            }
            catch (Exception ex)
            {
                if (!_silent)
                    ThemedMessageBox.Show(Properties.Resources.Achtung,
                        "Fehler beim suchen nach der neuesten Version\n" + ex.Message,
                        System.Windows.MessageBoxButton.OK,
                        System.Windows.MessageBoxImage.Error);
                return null;
            }

            var ServerVersions = new Ini();
            ServerVersions.Load(iniString);

            var versionen = ServerVersions.GetSections();
            ServerVersion = new Version("0.0.0.0");
            foreach (var version in versionen)
            {
                var v = new Version(version);
                if (v > ServerVersion)
                    ServerVersion = v;
            }
            return ServerVersions;
        }
    }
}