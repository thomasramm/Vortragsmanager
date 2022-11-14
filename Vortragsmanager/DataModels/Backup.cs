using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using Vortragsmanager.Enums;
using Vortragsmanager.Module;

namespace Vortragsmanager.DataModels
{
    internal static class Backup
    {
        private static string _backupFile;

        public static string GetBackupFile()
        {
            if (_backupFile == null)
            {
                var folder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData, Environment.SpecialFolderOption.Create) + @"\Vortragsmanager DeLuxe\";
                Directory.CreateDirectory(folder);
                _backupFile = folder + "backup.zip";
            }
            return _backupFile;
        }

        /// <summary>
        /// Fügt eine Datei dem Systemarchiv hinzu.
        /// Der Dateiname wird im Archiv gelöscht und durch einen Timestamp ersetzt.
        /// </summary>
        /// <param name="file">Die Datei die gespeichert werden soll</param>
        /// <returns type="string">Den Namen unter dem die Datei gespeichert wurde</returns>
        public static string Add(string file)
        {
            var archiveName = $"{DateTime.Now:yyyy-MM-dd-HH-mm-ss}.sqlite3";
            Add(file, archiveName);
            return archiveName;
        }

        /// <summary>
        /// Fügt eine Datei dem Systemarchiv hinzu.
        /// Der Dateiname wird im Archiv gelöscht und durch einen Timestamp ersetzt.
        /// </summary>
        /// <param name="fileName">Die Datei die gespeichert werden soll</param>
        /// <param name="archiveName">
        /// Der Name unter dem die Datei im Archiv gespeichert werden soll. 
        /// Achtung! Es werden nur Dateinamen in folgenden Timestamp-Format unterstützt: yyyy-MM-dd-HH-mm-ss.sqlite3
        /// </param>
        public static void Add(string fileName, string archiveName)
        {
            try
            {
                using (var zipArchive = ZipFile.Open(GetBackupFile(), ZipArchiveMode.Update))
                {
                    zipArchive.CreateEntryFromFile(fileName, archiveName);
                }
            }
            catch(Exception ex)
            {
                Log.Error("Backup.Add()", "Kann aktuellen Stand nicht in Backup erstellen. "+ GetBackupFile() + " | " + ex.Message);
            }
        }

        public static List<BackupItem> List()
        {
            if (!File.Exists(GetBackupFile()))
                return new List<BackupItem>();

            try
            {
                using (var zipArchive = ZipFile.OpenRead(GetBackupFile()))
                {
                    return zipArchive.Entries.Select(x => new BackupItem(x.Name)).OrderByDescending(x => x.Date).ToList();
                }
            }
            catch(Exception ex)
            {
                Log.Error("Backup.List()", "Kann Liste der Backup's nicht erstellen. " + GetBackupFile() + " | " + ex.Message);
            }

            return new List<BackupItem>();
        }

        public static bool Restore(string backupName, bool makeBackup = true)
        {
            try
            {
                var fileName = Properties.Settings.Default.sqlite;
                var newBackup = makeBackup ? Add(fileName) : "NONE";
                File.Delete(fileName);
                using (var zip = ZipFile.OpenRead(GetBackupFile()))
                {
                    var datei = zip?.GetEntry(backupName);
                    datei?.ExtractToFile(fileName);
                }
                if (File.Exists(fileName))
                {
                    IoSqlite.ReadContainer(fileName);
                    return true;
                }

                //Wenn das wiederherstellen nicht geklappt hat, dann das "frische" Backup wiederherstellen.
                //Damit das keine Endlosschleife wird, wird das wiederherstellen des "frischen" Backup kein weiteres Backup erzeugen.
                if (newBackup != "NONE")
                    Restore(newBackup, false);
            }
            catch (Exception ex)
            {
                Log.Error("Backup.Restore()", "Kann Backup nicht wieder herstellen. " + GetBackupFile() + " | " + ex.Message);
            }

            return false;
        }

        /// <summary>
        /// Entfernt ein Backup
        /// </summary>
        /// <param name="backup">Der zu entfernende Dateiname</param>
        public static void Remove(string backup)
        {
            try
            {
                using (var zip = ZipFile.Open(GetBackupFile(), ZipArchiveMode.Update))
                {
                    var datei = zip?.GetEntry(backup);
                    datei?.Delete();
                }
            }
            catch (IOException ex)
            {
                Log.Error(nameof(CleanOldBackups), "IO Fehler bei Entfernen eins Backup aus ZIP Datei: " + GetBackupFile() + " | " + ex.Message);
            }
            catch (Exception ex)
            {
                Log.Error(nameof(CleanOldBackups), "Sonstiger Fehler bei Eintfernen eines Backup aus ZIP Datei: " + GetBackupFile() + " | " + ex.Message);
            }
        }

        public static bool CleanOldBackups()
        {
            // Alle Backups nach Datum sortiert, beginnend mit dem Jüngsten
            var items = List();
            var letzteStunde = -1;
            var letzterTag = DateTime.Today;
            var letzterMonat = DateTime.Today.Year * 100 + DateTime.Today.Month;
            foreach (var item in items)
            {
                //Alle behalten
                if (item.Age == BackupAge.Heute)
                    continue;

                //Jede Stunde
                if (item.Age == BackupAge.Diese_Woche)
                {
                    if (item.Date.Date == letzterTag && item.Date.Hour == letzteStunde)
                        Remove(item.FileName);
                    else
                    {
                        letzterTag = item.Date.Date;
                        letzteStunde = item.Date.Hour;
                    }
                    continue;
                }

                //Das letzte von jedem Tag behalten
                if (item.Age == BackupAge.Dieser_Monat)
                {
                    if (item.Date == letzterTag)
                        Remove(item.FileName);
                    else
                        letzterTag = item.Date;
                    continue;
                }

                //Das letzte von jedem Monat
                if (item.Age == BackupAge.Dieses_Jahr)
                {
                    if (item.Date.Month == letzterMonat)
                        Remove(item.FileName);
                    else
                        letzterMonat = item.Date.Year * 100 + item.Date.Month;
                    continue;
                }
                //Das letzte von jedem Monat in den letzten 12 Monaten
                if (item.Age == BackupAge.Älter)
                {
                    if ((DateTime.Today - item.Date).Days < 365)
                    {
                        var monat = item.Date.Year * 100 + item.Date.Month;
                        if (monat == letzterMonat)
                            Remove(item.FileName);
                        else
                            letzterMonat = monat;
                        continue;
                    }
                }
                //Älter als 12 Monate wird nicht mehr gespeichert.
                Remove(item.FileName);
            }
            return true;
        }
    }
}
