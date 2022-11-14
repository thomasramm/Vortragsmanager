using System;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using DevExpress.Xpf.Core;
using Vortragsmanager.Converter;
using Vortragsmanager.Datamodels;
using Vortragsmanager.DataModels;
using Vortragsmanager.Windows;

namespace Vortragsmanager.Module
{
    internal static class Update
    {
        /// <summary>
        /// IoSqlite.UpdateDatabase für Datenbank Updates (Struktur)
        /// Module.Update für C# Updates (Inhalte)
        /// Changelog.md
        /// </summary>
        public static int CurrentVersion => 26;

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

            if (DataContainer.Version < 23)
            {
                //Neues Backup System, alle VdL Dateien des aktuellen Ordner einlesen
                var di = new FileInfo(Properties.Settings.Default.sqlite).Directory;
                if (di != null)
                {
                    var files = di.GetFiles("*_????-??-??-??-??.sqlite3", SearchOption.TopDirectoryOnly);
                    foreach (var file in files)
                    {
                        if (file.Name != Properties.Settings.Default.sqlite)
                        {
                            var name = file.Name.Length >= 24 ? file.Name.Substring(file.Name.Length - 24) : file.Name;
                                name = name.Replace(".sqlite3", "-00.sqlite3");
                            Backup.Add(file.FullName, name);
                            file.Delete();
                        }
                    }
                }
            }

            //Aktualisierte Vorträge, hier kann der Updatebefehl mehrfach genutzt werden. Einfach die neue Programmversion in der nächsten Zeile eintragen.
            if (DataContainer.Version < 26)
            {
                var inhalt = "Es gibt geänderte Vortragsthemen. Du kannst die Themen jetzt aktualisieren. Damit werden individuelle Änderungen die du in der Vergangenheit an den Vortragsthemen vorgenommen hast gelöscht." + Environment.NewLine +
    "Du kannst die Änderung auch später unter 'Vorträge' -> 'Zurücksetzen' durchführen." + Environment.NewLine +
    "Sollen die Vortragsthemen nun aktualisiert werden? (Empfohlen)" + Environment.NewLine +
    "Wenn nicht, musst du die Themenänderungen selber unter 'Vorträge' einpflegen.";
                var result = ThemedMessageBox.Show("Achtung", inhalt, System.Windows.MessageBoxButton.YesNo, System.Windows.MessageBoxImage.Question);
                if (result == System.Windows.MessageBoxResult.Yes)
                {
                    TalkList.Reset();
                }
            }

            //auf aktuellste Version setzen = 25 (siehe oben)
            //siehe auch IoSqlite.UpdateDatabase
            DataContainer.Version = CurrentVersion;
        }

        public static void ShowChanges(bool force = false)
        {
            var oldVersion = StringToVersionConverter.Convert(Properties.Settings.Default.LastChangelog);
            string fileContent;
            var pathToChangelog = Properties.Settings.Default.ChangelogPfad;
            var aktuelleVersion = System.Reflection.Assembly.GetEntryAssembly()?.GetName().Version ?? new Version(1,0);
            var header = $"Aktuelle Version {aktuelleVersion.Major}.{aktuelleVersion.Minor}.{aktuelleVersion.Build}";

            if (oldVersion == aktuelleVersion && !force)
            { 
                return;
            }

            try
            {
                Helper.Helper.GlobalSettings = new MyGloabalSettings();
                
                //Einlesen des Changelog.md
                using (WebClient client = new WebClient())
                {
                    client.Encoding = System.Text.Encoding.UTF8;
                    client.CachePolicy = new System.Net.Cache.RequestCachePolicy(System.Net.Cache.RequestCacheLevel.Reload);
                    fileContent = client.DownloadString(pathToChangelog);
                }

                //Anzeigen nur der Änderungen seit dem letzten Update
                if (oldVersion > new Version() && oldVersion != aktuelleVersion)
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
            var dlg = new LeerDialog();
            var dlgMdl = (LeerViewModel)dlg.DataContext;
            dlgMdl.Titel = "Verbesserungen der neuen Version";
            dlgMdl.HeaderText = header;
            dlgMdl.ShowCopyButton = false;
            dlgMdl.ShowSaveButton = false;
            dlgMdl.ShowCloseButton = true;
            dlgMdl.Text = fileContent;
            dlg.ShowDialog();

            Properties.Settings.Default.LastChangelog = aktuelleVersion.ToString();
            Properties.Settings.Default.Save();
        }

        private static int FindVersion(string fileContent, Version oldVersion)
        {
            string pattern = @"### Version .+###";
            MatchCollection m = Regex.Matches(fileContent, pattern);
            foreach (Match m2 in m)
            {
                var x = m2.Value.Replace("###", "").Replace("Version", "").Trim();
                var posKlammerAuf = x.IndexOf("(", StringComparison.InvariantCulture) - 1;
                if (x.Length >= posKlammerAuf && posKlammerAuf >= 0)
                    x = x.Substring(0,  posKlammerAuf);
                var v = new Version(x);

                Console.WriteLine("Found '{0}' at position {1}.", m2.Value, m2.Index);

                if (v < oldVersion)
                    return m2.Index;
            }
            return -1;
        }
    }
}
