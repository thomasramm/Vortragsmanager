using DevExpress.Xpf.Core;
using System;
using System.ComponentModel;
using System.Net;
using System.Reflection;
using System.Text;
using Vortragsmanager.Views;

namespace Vortragsmanager.Core
{
    internal static class Updater
    {
        private static readonly BackgroundWorker _updateWorker = new BackgroundWorker();
        internal static bool _silent = true;

        public static DateTime LocalDate { get; set; }

        public static DateTime ServerDate { get; set; }

        public static Ini ServerVersions { get; set; }

        public static void CheckForUpdates()
        {
            Log.Info(nameof(CheckForUpdates));
            if (!Properties.Settings.Default.SearchForUpdates)
                return;

            var nextSearch = Properties.Settings.Default.NextUpdateSearch;
            if (nextSearch > DateTime.Today)
                return;

            CheckForUpdatesForce(true);
        }

        public static void CheckForUpdatesForce(bool silent)
        {
            Log.Info(nameof(CheckForUpdates), $"silent={silent}");
            _silent = silent;
            var nextSearch = Properties.Settings.Default.NextUpdateSearch;
            nextSearch = DateTime.Today.AddDays(1);
            Properties.Settings.Default.NextUpdateSearch = nextSearch;
            Properties.Settings.Default.Save();

            var localVersion = Assembly.GetEntryAssembly().GetName().Version;
            LocalDate = new DateTime(2000, 1, 1).AddDays(localVersion.Build);

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
            Log.Info(nameof(UpdaterFinished));
            _updateWorker.DoWork -= new DoWorkEventHandler(UpdaterDoWork);
            _updateWorker.RunWorkerCompleted -= new RunWorkerCompletedEventHandler(UpdaterFinished);
            if (ServerDate == new DateTime(2000, 1, 1))
                return;
            if (LocalDate >= ServerDate)
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
            data.LocalVersion = LocalDate;
            data.ServerVersion = ServerDate;
            data.ServerIni = ServerVersions;
            w.ShowDialog();
        }

        private static Ini ReadNewestVersion()
        {
            Log.Info(nameof(ReadNewestVersion));
            var iniString = string.Empty;

            try
            {
                using (WebClient client = new WebClient())
                {
                    client.Encoding = Encoding.UTF8;
                    client.CachePolicy = new System.Net.Cache.RequestCachePolicy(System.Net.Cache.RequestCacheLevel.Reload);
                    iniString = client.DownloadString(Properties.Settings.Default.ChangelogPfad);
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
            ServerDate = new DateTime(2000, 1, 1);
            foreach (var version in versionen)
            {
                var v = DateTime.Parse(version, Helper.German);
                if (v > ServerDate)
                    ServerDate = v;
            }
            return ServerVersions;
        }
    }
}