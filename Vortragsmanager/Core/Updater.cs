﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Reflection;
using System.Net;
using System.IO;
using Vortragsmanager.Views;

namespace Vortragsmanager.Core
{
    static class Updater
    {
        private static readonly BackgroundWorker _updateWorker = new BackgroundWorker();

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

            CheckForUpdatesForce();
        }
        public static void CheckForUpdatesForce()
        {
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
            if (LocalVersion >= ServerVersion)
                return;

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

            using (WebClient client = new WebClient())
            {
                iniString = client.DownloadString("http://thomas-ramm.de/Vortragsmanager/version.ini");
            }
            if (string.IsNullOrEmpty(iniString))
                return null;

            var ServerVersions = new Ini();
            ServerVersions.Load(iniString);

            var versionen = ServerVersions.GetSections();
            ServerVersion = new Version("0.0.0.0");
            foreach(var version in versionen)
            {
                var v = new Version(version);
                if (v > ServerVersion)
                    ServerVersion = v;
            }
            return ServerVersions;
        }
    }
}
