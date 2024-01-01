using System;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using DevExpress.Xpf.Core;
using Vortragsmanager.Converter;
using Vortragsmanager.DataModels;
using Vortragsmanager.Windows;
using System.Linq;
using Vortragsmanager.Helper;

namespace Vortragsmanager.Module
{
    internal static class Update
    {
        /// <summary>
        /// IoSqlite.UpdateDatabase für Datenbank Updates (Struktur)
        /// Module.Update für C# Updates (Inhalte)
        /// Changelog.md
        /// </summary>
        public static int CurrentVersion => 30;

        public static void Process()
        {
            Log.Info(nameof(Process));

            if (DataContainer.Version < 15)
            {
                DataContainer.AufgabenPersonZuordnung.Add(new AufgabenZuordnung(-1) { PersonName = "Nicht Vorgesehen", IsVorsitz = true, IsLeser = true, Häufigkeit = 1 });
            }

            if (DataContainer.Version < 23)
            {
                //Neues Backup System, alle VdL Dateien des aktuellen Ordner einlesen
                var di = new FileInfo(Helper.Helper.GlobalSettings.sqlite).Directory;
                if (di != null)
                {
                    var files = di.GetFiles("*_????-??-??-??-??.sqlite3", SearchOption.TopDirectoryOnly);
                    foreach (var file in files)
                    {
                        if (file.Name != Helper.Helper.GlobalSettings.sqlite)
                        {
                            var name = file.Name.Length >= 24 ? file.Name.Substring(file.Name.Length - 24) : file.Name;
                                name = name.Replace(".sqlite3", "-00.sqlite3");
                            Backup.Add(file.FullName, name);
                            file.Delete();
                        }
                    }
                }
            }

            //Vorträge die nicht mehr gehalten werden sollen
            if (DataContainer.Version < 29)
            {
                FindFutureTalk(112, new DateTime(2023,6,1));
                FindFutureTalk(131, new DateTime(2023,9,1));
                FindFutureTalk(132, new DateTime(2023,9,1));
            }

            //Aktualisierte Vorträge, hier kann der Updatebefehl mehrfach genutzt werden. Einfach die neue Programmversion in der nächsten Zeile eintragen.
            if (DataContainer.Version < 30)
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

        private static void FindFutureTalk(int nr, DateTime sperrdatum)
        {
            var list = DataContainer.MeinPlan.Where(x => x.Vortrag?.Vortrag.Nummer == nr && x.Kw >= DateCalcuation.CalculateWeek(sperrdatum)).ToList();
            if (list.Count == 0) 
            { 
                return; 
            }

            var datum = string.Empty;
            foreach (var item in list)
            {
                datum += DateCalcuation.CalculateWeek(item.Kw).ToShortDateString() + ", ";
            }
            if (datum.Length > 2) 
            {
                datum = datum.TrimEnd(new[]{',', ' '});
            }

            var inhalt = $"Der Vortrag Nr. {nr} sollte nicht mehr gehalten werden, ist aber bei dir am {datum} eingeplant. Du musst ihn evtl. umplanen";
            ThemedMessageBox.Show("Achtung", inhalt, System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
        }

        public static void ShowChanges(bool force = false)
        {
            var oldVersion = StringToVersionConverter.Convert(Helper.Helper.GlobalSettings.LastChangelog);
            string fileContent;
            var pathToChangelog = Helper.Helper.GlobalSettings.ChangelogPfad;
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
                    + Helper.Helper.GlobalSettings.ChangelogPfad;
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

            Helper.Helper.GlobalSettings.LastChangelog = aktuelleVersion.ToString();
            Helper.Helper.GlobalSettings.Save();
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
