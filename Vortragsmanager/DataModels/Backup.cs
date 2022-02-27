using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.IO.Compression;
using System.Linq;
using Vortragsmanager.Module;

namespace Vortragsmanager.DataModels
{
    internal static class Backup
    {
        private static string _backupFile;

        private static string GetBackupFile()
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
            using (var zipArchive = ZipFile.Open(GetBackupFile(), ZipArchiveMode.Update))
            {
                zipArchive.CreateEntryFromFile(fileName, archiveName);
            }
        }

        public static List<BackupItem> List()
        {
            if (!File.Exists(GetBackupFile()))
                return new List<BackupItem>();

            using (var zipArchive = ZipFile.OpenRead(GetBackupFile()))
            {
                return zipArchive.Entries.Select(x => new BackupItem(x.Name)).OrderByDescending(x => x.Date).ToList();
            }
        }

        public static bool Restore(string backupName, bool makeBackup = true)
        {
            var fileName = Properties.Settings.Default.sqlite;
            var newBackup = makeBackup? Add(fileName) : "NONE";
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

            return false;
        }

        /// <summary>
        /// Entfernt ein Backup
        /// </summary>
        /// <param name="backup">Der zu entfernende Dateiname</param>
        public static void Remove(string backup)
        {
            using(var zip = ZipFile.Open(GetBackupFile(), ZipArchiveMode.Update))
            {
                var datei = zip?.GetEntry(backup);
                datei?.Delete();
            }
        }

        public static void CleanOldBackups()
        {
            //Alle Backups nach Datum sortiert, beginnend mit dem Jüngsten
            var items = List();
            var letzteStunde = -1;
            var letzterTag = DateTime.Today;
            var letzterMonat = DateTime.Today.Year*100 + DateTime.Today.Month;
            foreach(var item in items)
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
                        letzterMonat = item.Date.Year*100+item.Date.Month;
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

        }
    }

    public class BackupItem
    {
        private DateTime _date;
        private string _fileName;

        public BackupItem(string filename)
        {
            FileName = filename;
        }

        public string Name => FileName.Replace(".sqlite3", "");

        public string FileName
        {
            get => _fileName; 
            set
            {
                _fileName = value;
                _date = DateTime.ParseExact(Name, "yyyy-MM-dd-HH-mm-ss", null);
                Age = GetAge();
            }
        }

        public string DisplayName
        {
            get
            {
                switch (Age)
                {
                    case BackupAge.Heute:
                        return _date.ToLongTimeString() + " Uhr";
                    case BackupAge.Diese_Woche:
                        return _date.ToString("dddd, dd.MM HH:mm:ss") + " Uhr";
                    case BackupAge.Dieser_Monat:
                        return _date.ToString("dd.MM HH:mm:ss") + " Uhr";
                    case BackupAge.Dieses_Jahr:
                        return _date.ToString("MMMM, dd.MM HH:mm:ss") + " Uhr";
                    case BackupAge.Älter:
                        return _date.ToString("yyyy, dd.MM.yyyy HH:mm:ss") + " Uhr";
                    default:
                        return _date.ToString("dd.MM.yyyy HH:mm:ss") + " Uhr";
                }
            }
        }

        public BackupAge Age { get; set; }

        public DateTime Date => _date;

        public string AgeIcon
        {
            get
            {
                switch (Age)
                {
                    case BackupAge.Heute:
                        return @"\Images\CalendarDay_32x32.png";
                    case BackupAge.Diese_Woche:
                        return @"\Images\CalendarWeek_32x32.png";
                    case BackupAge.Dieser_Monat:
                        return @"\Images\Calendar_32x32.png";
                    case BackupAge.Dieses_Jahr:
                        return @"\Images\CalendarYear_32x32.png";
                    case BackupAge.Älter:
                        return @"\Images\CalendarYear2_32x32.png";
                    default:
                        return @"\Images\Calendar_32x32.png";
                }
                
            }
        }

        public string Zeitabstand
        {
            get
            {
                var diff = DateTime.Now - _date;

                if (diff.TotalMinutes <= 1)
                    return $"vor {diff.Seconds} Sekunden.";
                if (diff.TotalMinutes < 60)
                    return $"vor {diff.Minutes} Minuten und {diff.Seconds} Sekunden";
                if (diff.TotalHours < 24)
                    return $"vor ~ {diff.Hours} Stunden und {diff.Minutes} Minuten.";
                if (diff.TotalDays < 31)
                    return $"vor {diff.Days} Tagen";

                return $"vor {Math.Round(diff.TotalDays / 31,1)} Monaten";
            }
        }

        private BackupAge GetAge()
        {
            //heute
            if (_date.Date == DateTime.Today)
                return BackupAge.Heute;

            //Diese Woche
            var cal = System.Globalization.DateTimeFormatInfo.CurrentInfo.Calendar;
            var d1 = _date.Date.AddDays(-1 * (int)cal.GetDayOfWeek(_date));
            var d2 = DateTime.Today.Date.AddDays(-1 * (int)cal.GetDayOfWeek(DateTime.Today));
            if (d1 == d2)
            {
                return BackupAge.Diese_Woche;
            }

            //dieses Jahr
            if (_date.Year == DateTime.Today.Year)
            {
                //dieser Monat
                if (_date.Month == DateTime.Today.Month)
                {
                    return BackupAge.Dieser_Monat;
                }

                return BackupAge.Dieses_Jahr;
            }

            //älter
            return BackupAge.Älter;
        }
    }

    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public enum BackupAge
    {
        Heute,
        Diese_Woche,
        Dieser_Monat,
        Dieses_Jahr,
        Älter
    }
}
