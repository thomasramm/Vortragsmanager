﻿using DevExpress.Xpf.Core;
using System;
using System.Net;
using System.Text.RegularExpressions;
using Vortragsmanager.Datamodels;
using Vortragsmanager.Views;

namespace Vortragsmanager.Core
{
    internal static class Update
    {
        /// <summary>
        /// IoSqlite.UpdateDatabase für Datenbank Updates (Struktur)
        /// Initialize.Update für C# Updates (Inhalte)
        /// Changelog.md
        /// </summary>
        public static int CurrentVersion => 20;

        public static void Process()
        {
            Log.Info(nameof(Process));

            if (DataContainer.Version < 3)
            {
                TalkList.Add(56, "Wessen Führung kannst du vertrauen?");
                TalkList.Add(-1, "Unbekannt", false);
            }

            if (DataContainer.Version < 11)
            {
                // => IoSqlite.UpdateDatabase(); -- Bestehender Vortrag 24 auf -24 geändert
                TalkList.Add(24, "„Eine besonders kostbare Perle“ – habe ich sie gefunden?");
            }

            if (DataContainer.Version < 15)
            {
                DataContainer.AufgabenPersonZuordnung.Add(new AufgabenZuordnung(-1) { PersonName = "Nicht Vorgesehen", IsVorsitz = true, IsLeser = true, Häufigkeit = 1 });
            }

            if (DataContainer.Version < 16)
            {
                TalkList.Reset();
            }

            if (DataContainer.Version < 20)
            {
                var inhalt = $"Es gibt geänderte Vortragsthemen. Du kannst die Themen jetzt aktualisieren. Damit werden individuelle Änderungen die du in der Vergangenheit an den Vortragsthemen vorgenommen hast gelöscht." + Environment.NewLine +
    $"Du kannst die Änderung auch später unter 'Vorträge' -> 'Zurücksetzen' durchführen." + Environment.NewLine +
    "Sollen die Vortragsthemen nun aktualisiert werden? (Empfohlen)" + Environment.NewLine +
    "Wenn nicht, musst du die Themenänderungen selber unter 'Vorträge' einpflegen.";
                var result = ThemedMessageBox.Show("Achtung", inhalt, System.Windows.MessageBoxButton.YesNo, System.Windows.MessageBoxImage.Question);
                if (result == System.Windows.MessageBoxResult.Yes)
                {
                    TalkList.Reset();
                }
            }

            //auf aktuellste Version setzen = 21
            //siehe auch IoSqlite.UpdateDatabase
            DataContainer.Version = Update.CurrentVersion;

            if (!Properties.Settings.Default.HideChangelog)
                ShowChanges(Helper.ConvertToVersion(Properties.Settings.Default.LastChangelog));
        }

        public static void ShowChanges(Version oldVersion)
        {
            string fileContent = String.Empty;
            string pathToChangelog = Properties.Settings.Default.ChangelogPfad;

            try
            {
                var aktuelleVersion = System.Reflection.Assembly.GetEntryAssembly().GetName().Version;

                Helper.GlobalSettings = new MyGloabalSettings();
                string message = $"Aktuelle Version {aktuelleVersion.Major}.{aktuelleVersion.Minor}.{aktuelleVersion.Build}{Environment.NewLine}{Environment.NewLine}";

                //Einlesen des Changelog.md
                using (WebClient client = new WebClient())
                {
                    client.Encoding = System.Text.Encoding.UTF8;
                    client.CachePolicy = new System.Net.Cache.RequestCachePolicy(System.Net.Cache.RequestCacheLevel.Reload);
                    fileContent = client.DownloadString(pathToChangelog);
                }

                //Anzeigen nur der Änderungen seit dem letzten Update
                if (oldVersion > new Version())
                {
                    var index = FindVersion(fileContent, oldVersion);
                    if (index > -1)
                        fileContent = fileContent.Substring(0, index);
                }
            }
            catch
            {
                fileContent = "Fehler beim Abrufen der Änderungsliste (Changelog) aus dem Internet." + Environment.NewLine
                    + "Bitte die Änderungshinweise manuell aufrufen." + Environment.NewLine
                    + Environment.NewLine
                    + Properties.Settings.Default.ChangelogPfad;
            }

            //Dialog anzeigen
            var dlg = new leerDialog();
            var dlg_mdl = (LeerViewModel)dlg.DataContext;
            dlg_mdl.Titel = "Verbesserungen der neuen Version";
            dlg_mdl.ShowCopyButton = false;
            dlg_mdl.ShowSaveButton = false;
            dlg_mdl.ShowCloseButton = true;
            dlg_mdl.Text = fileContent;
            dlg.ShowDialog();
        }

        private static int FindVersion(string fileContent, Version oldVersion)
        {
            string pattern = @"### Version .+###";
            MatchCollection m = Regex.Matches(fileContent, pattern);
            foreach (Match m2 in m)
            {
                var x = m2.Value.Replace("###", "").Replace("Version", "").Trim();
                x = x.Substring(0, x.IndexOf("(") - 1);
                var v = new Version(x);

                Console.WriteLine("Found '{0}' at position {1}.", m2.Value, m2.Index);

                if (v < oldVersion)
                    return m2.Index;
            }
            return -1;
        }
    }
}
