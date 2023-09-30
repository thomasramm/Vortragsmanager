using DevExpress.Mvvm;
using System;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace Vortragsmanager.DataModels
{
    public class MyGloabalSettings : ViewModelBase
    {
        public MyGloabalSettings()
        {
            Titel = "vdl";
            Programmversion = GetVersion();
        }

        private static string GetVersion()
        {
            var version = System.Reflection.Assembly.GetEntryAssembly()?.GetName().Version;
            if (version == null)
            {
                return "Version ist unbekannt";
            }

            var v = $"Version {version.Major}.{version.Minor}.{version.Build}";
            return v;
        }

        public void RefreshTitle()
        {
            var fi = new FileInfo(Helper.Helper.GlobalSettings.sqlite);
            Titel = "Vortragsmanager DeLuxe | " + fi.Name;
            if (DataContainer.IsDemo)
                Titel += "   ~~~~~ DEMODATEI ~~~~~ ÄNDERUNGEN WERDEN NICHT GESPEICHERT! ~~~~~";

            RaisePropertyChanged(nameof(Titel));
        }

        [XmlIgnore]
        public string Programmversion { get; }

        [XmlIgnore]
        public string Titel { get; set; }

        public string ChangelogPfad { get; set; } = "https://raw.githubusercontent.com/thomasramm/Vortragsmanager/master/Changelog.md";

        public bool DashboardShowDetails { get; set; } = true;

        public string ExcelTemplateAushang { get; set; } = string.Empty;

        public bool FotoExport { get; set; } = true;

        public bool HideChangelog { get; set; } = false;

        public string LastChangelog { get; set; } = "0";

        public int ListAushangAnzahlWochen { get; set; } = 10;

        public bool ListCreate_OpenFile { get; set; } = true;

        public string LogFolder { get; set; } = "-";

        public int LogLevel { get; set; } = 1;

        public string NameKreisaufseher { get; set; } = "Name des Kreisaufseher";

        public int RednerSuchenAbstandAnzahlMonate { get; set; } = 12;

        public int RednerSuchenAnzahlMonate { get; set; } = 12;

        public bool SaveBackups { get; set; } = true;

        public string SearchSpeaker_Kreis { get; set; } = string.Empty;

        public int SearchSpeaker_MaxEntfernung { get; set; } = 100;

        public bool SearchSpeaker_OffeneAnfrage { get; set; } = false;

        public bool SearchSpeaker_RednerCheckCancelation { get; set; } = true;

        public bool SearchSpeaker_RednerCheckFuture { get; set; } = true;

        public bool SearchSpeaker_RednerCheckHistory { get; set; } = true;

        public bool SearchSpeaker_VortragCheckFuture { get; set; } = true;

        public bool SearchSpeaker_VortragCheckHistory { get; set; } = true;

        public bool SearchSpeaker_VortragCheckOpenRequest { get; set; } = true;

        public bool ShowActivityButtons { get; set; } = true;

        public int SonntagAnzeigeMonate { get; set; } = 1;

        public string sqlite { get; set; } = "vortragsmanager.sqlite3";

        public bool ThemeIsDark { get; set; } = true;

        public void Read()
        {
            var configFile = Helper.Helper.AppFolderPath + "Settings.xml";
            if (!File.Exists(configFile))
            {
                ////ToDo: 20.04.2023: Dieser Programmcode wird in ein paar Versionen entfernt.
                ////aktuell nutze ich die Settings mit seinen default-Werten, später die default-Werte der Klasse
                DashboardShowDetails = Properties.Settings.Default.DashboardShowDetails;
                ExcelTemplateAushang = Properties.Settings.Default.ExcelTemplateAushang;
                FotoExport = Properties.Settings.Default.FotoExport;
                HideChangelog = Properties.Settings.Default.HideChangelog;
                LastChangelog = Properties.Settings.Default.LastChangelog;
                ListAushangAnzahlWochen = Properties.Settings.Default.ListAushangAnzahlWochen;
                ListCreate_OpenFile = Properties.Settings.Default.ListCreate_OpenFile;
                LogFolder= Properties.Settings.Default.LogFolder;
                LogLevel= Properties.Settings.Default.LogLevel;
                NameKreisaufseher = Properties.Settings.Default.NameKreisaufseher;
                RednerSuchenAbstandAnzahlMonate = Properties.Settings.Default.RednerSuchenAbstandAnzahlMonate;
                RednerSuchenAnzahlMonate = Properties.Settings.Default.RednerSuchenAnzahlMonate;
                SaveBackups = Properties.Settings.Default.SaveBackups;
                SearchSpeaker_Kreis = Properties.Settings.Default.SearchSpeaker_Kreis;
                SearchSpeaker_MaxEntfernung = Properties.Settings.Default.SearchSpeaker_MaxEntfernung;
                SearchSpeaker_OffeneAnfrage = Properties.Settings.Default.SearchSpeaker_OffeneAnfrage;
                SearchSpeaker_RednerCheckCancelation = Properties.Settings.Default.SearchSpeaker_RednerCheckCancelation;
                SearchSpeaker_RednerCheckFuture = Properties.Settings.Default.SearchSpeaker_RednerCheckFuture;
                SearchSpeaker_RednerCheckHistory = Properties.Settings.Default.SearchSpeaker_RednerCheckHistory;
                SearchSpeaker_VortragCheckFuture = Properties.Settings.Default.SearchSpeaker_VortragCheckFuture;
                SearchSpeaker_VortragCheckHistory = Properties.Settings.Default.SearchSpeaker_VortragCheckHistory;
                SearchSpeaker_VortragCheckOpenRequest = Properties.Settings.Default.SearchSpeaker_VortragCheckOpenRequest;
                ShowActivityButtons = Properties.Settings.Default.ShowActivityButtons;
                SonntagAnzeigeMonate = Properties.Settings.Default.SonntagAnzeigeMonate;
                sqlite = Properties.Settings.Default.sqlite;
                ThemeIsDark = Properties.Settings.Default.ThemeIsDark;

                return;
            }

            XmlSerializer xs = new XmlSerializer(typeof(MyGloabalSettings));
            using (var sr = new StreamReader(configFile))
            {
                Helper.Helper.GlobalSettings = (MyGloabalSettings)xs.Deserialize(sr);
            }
        }

        public void Save()
        {
            try
            {
                XmlSerializer xs = new XmlSerializer(typeof(MyGloabalSettings));
                TextWriter tw = new StreamWriter(Helper.Helper.AppFolderPath + "Settings.xml");
                xs.Serialize(tw, Helper.Helper.GlobalSettings);
            }
            catch(Exception ex)
            {
                Module.Log.Error("SettingsSave", ex.Message);
            }
        }
    }
}