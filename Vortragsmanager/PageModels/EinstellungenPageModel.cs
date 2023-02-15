using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Forms;
using DevExpress.Mvvm;
using DevExpress.Xpf.Core;
using Vortragsmanager.Datamodels;
using Vortragsmanager.Enums;
using Vortragsmanager.Helper;
using Vortragsmanager.Module;
using Vortragsmanager.Windows;
using Application = System.Windows.Application;

namespace Vortragsmanager.PageModels
{
    public class EinstellungenPageModel : ViewModelBase
    {
        private string _datenbank;

        public EinstellungenPageModel()
        {
            ExcelFileDialogCommand = new DelegateCommand(ExcelFileDialog);
            SearchDatabaseCommand = new DelegateCommand<string>(SearchDatabase);
            UpdateSpeakerFromExcelCommand = new DelegateCommand(UpdateSpeakerFromExcel);
            EmergencyMailCommand = new DelegateCommand<int?>(EmergencyMail);
            CalculateRouteCommand = new DelegateCommand<bool>(CalculateRoute);
            ShowChangelogCommand = new DelegateCommand(ShowChangelog);
            Datenbank = Properties.Settings.Default.sqlite;
            SelectedTheme = ThemeIsDark ? "Dunkel" : "Hell";
        }

        protected override void OnParameterChanged(object parameter)
        {
            SelectedGroup = (string)parameter == "Design" ? 2 : 1;
            base.OnParameterChanged(parameter);
        }

        private int _selectedGroup;
        private int SelectedGroup
        {
            get => _selectedGroup;
            set
            {
                _selectedGroup = value;
                RaisePropertiesChanged(nameof(GruppeAussehenVisible)
                    , nameof(GruppeDateiVisible)
                    , nameof(GruppeVerhaltenVisible)
                    , nameof(GruppeAktionenVisible));
                RaisePropertiesChanged(nameof(GruppeAussehenChecked)
                    , nameof(GruppeDateiChecked)
                    , nameof(GruppeVerhaltenChecked)
                    , nameof(GruppeAktionenChecked));
            }
        }
        public Visibility GruppeDateiVisible => SelectedGroup == 1 || SelectedGroup == 0 ? Visibility.Visible : Visibility.Collapsed;

        public Visibility GruppeAussehenVisible => SelectedGroup == 2 || SelectedGroup == 0 ? Visibility.Visible : Visibility.Collapsed;

        public Visibility GruppeVerhaltenVisible => SelectedGroup == 3 || SelectedGroup == 0 ? Visibility.Visible : Visibility.Collapsed;

        public Visibility GruppeAktionenVisible => SelectedGroup == 4 || SelectedGroup == 0 ? Visibility.Visible : Visibility.Collapsed;

        public bool GruppeDateiChecked
        {
            get => (SelectedGroup == 1);
            set
            {
                if (value)
                    SelectedGroup = 1;
            }
        }

        public bool GruppeAussehenChecked
        {
            get => (SelectedGroup == 2);
            set
            {
                if (value)
                    SelectedGroup = 2;
            }
        }

        public bool GruppeVerhaltenChecked
        {
            get => (SelectedGroup == 3);
            set
            {
                if (value)
                    SelectedGroup = 3;
            }
        }

        public bool GruppeAktionenChecked
        {
            get => (SelectedGroup == 4);
            set
            {
                if (value)
                    SelectedGroup = 4;
            }
        }


        private string _importExcelFile = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\Liste der Vortragskoordinatoren.xlsx";
        private static string _selectedTheme;

        public DelegateCommand ExcelFileDialogCommand { get; }

        public DelegateCommand UpdateSpeakerFromExcelCommand { get; }

        public DelegateCommand<int?> EmergencyMailCommand { get; }

        public DelegateCommand<bool> CalculateRouteCommand { get; }

        public DelegateCommand ShowChangelogCommand { get; }

        public string ImportExcelFile
        {
            get => _importExcelFile;
            set
            {
                _importExcelFile = value;
                RaisePropertyChanged();
            }
        }

        public int ListAushangAnzahlWochen
        {
            get => Properties.Settings.Default.ListAushangAnzahlWochen;
            set
            {
                if (value > 24)
                    value = 24;
                if (value < 1)
                    value = 1;
                Properties.Settings.Default.ListAushangAnzahlWochen = value;
                RaisePropertyChanged();
            }
        }

        public int RednerSuchenAnzahlMonate
        {
            get => Properties.Settings.Default.RednerSuchenAnzahlMonate;
            set
            {
                if (value > 36)
                    value = 36;
                if (value < 1)
                    value = 1;
                Properties.Settings.Default.RednerSuchenAnzahlMonate = value;
                RaisePropertyChanged();
            }
        }

        public int RednerSuchenAbstandAnzahlMonate
        {
            get => Properties.Settings.Default.RednerSuchenAbstandAnzahlMonate;
            set
            {
                if (value > 99)
                    value = 99;
                if (value < 1)
                    value = 1;
                Properties.Settings.Default.RednerSuchenAbstandAnzahlMonate = value;
                RaisePropertyChanged();
            }
        }

    public void ShowChangelog()
        {
            Update.ShowChanges(true);
        }

        public void ExcelFileDialog()
        {
            var fi = new FileInfo(ImportExcelFile);
            var dir = fi.Directory != null && fi.Directory.Exists ? fi.DirectoryName : Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            var openDialog = new OpenFileDialog
            {
                Filter = Properties.Resources.DateifilterExcel,
                FilterIndex = 1,
                RestoreDirectory = false,
                InitialDirectory = dir,
                FileName = fi.Name,
                CheckFileExists = true
            };

            if (openDialog.ShowDialog() == DialogResult.OK)
            {
                ImportExcelFile = openDialog.FileName;
            }

            openDialog.Dispose();
        }

        public void UpdateSpeakerFromExcel()
        {
            IoExcel.UpdateSpeakers(ImportExcelFile);
        }

        public string Datenbank
        {
            get => _datenbank;
            set
            {
                _datenbank = value;
                RaisePropertyChanged();
            }
        }

        public DelegateCommand<string> SearchDatabaseCommand { get; }

        public void SearchDatabase(string typ)
        {
            var fi = new FileInfo(Datenbank);

            if (typ == "Open")
            {
                var openDialog = new OpenFileDialog
                {
                    Filter = Properties.Resources.DateifilterSqlite,
                    FilterIndex = 1,
                    RestoreDirectory = false,
                    InitialDirectory = fi.DirectoryName,
                    FileName = fi.Name,
                    CheckFileExists = true
                };

                if (openDialog.ShowDialog() == DialogResult.OK)
                {
                    Datenbank = openDialog.FileName;
                    IoSqlite.ReadContainer(openDialog.FileName);
                    Properties.Settings.Default.sqlite = openDialog.FileName;
                    Properties.Settings.Default.Save();
                }

                openDialog.Dispose();
            }
            else if (typ == "Save")
            {
                var saveDialog = new SaveFileDialog
                {
                    Filter = Properties.Resources.DateifilterSqlite,
                    FilterIndex = 1,
                    RestoreDirectory = false,
                    InitialDirectory = fi.DirectoryName,
                    FileName = fi.Name
                };

                if (saveDialog.ShowDialog() == DialogResult.OK)
                {
                    Datenbank = saveDialog.FileName;
                    IoSqlite.SaveContainer(saveDialog.FileName, SaveBackup);
                    Properties.Settings.Default.sqlite = saveDialog.FileName;
                    Properties.Settings.Default.Save();
                }

                saveDialog.Dispose();
            }

            Helper.Helper.GlobalSettings.RefreshTitle();
        }

        public bool SaveBackup
        {
            get => Properties.Settings.Default.SaveBackups;
            set
            {
                Properties.Settings.Default.SaveBackups = value;
                Properties.Settings.Default.Save();
                RaisePropertyChanged();
            }
        }

        public bool ShowChangelogState
        {
            get => !Properties.Settings.Default.HideChangelog;
            set
            {
                Properties.Settings.Default.HideChangelog = !value;
                Properties.Settings.Default.Save();
                RaisePropertyChanged();
            }
        }

        public bool ShowActivityButtons
        {
            get => Properties.Settings.Default.ShowActivityButtons;
            set
            {
                Properties.Settings.Default.ShowActivityButtons = value;
                Properties.Settings.Default.Save();
                RaisePropertyChanged();
            }
        }

        public int SelectedLogLevel
        {
            get => Properties.Settings.Default.LogLevel;
            set
            {
                Log.Start((LogLevel)value);
                RaisePropertyChanged();
            }
        }

        public string LogFolder
        {
            get => Properties.Settings.Default.LogFolder;
            set
            {
                var di = new DirectoryInfo(value);
                if (!di.Exists)
                {
                    return;
                }
                Properties.Settings.Default.LogFolder = value;
                Properties.Settings.Default.Save();
                RaisePropertyChanged();
            }
        }

        public Wochentag ConregationDayOfWeek
        {
            get => DateCalcuation.Wochentag;
            set
            {
                DateCalcuation.Wochentag = value;
                var zeititem = DataContainer.MeineVersammlung.Zeit.Get(DateTime.Today.Year);
                zeititem.Tag = DateCalcuation.Wochentag;
                RaisePropertyChanged();
            }
        }

        public IEnumerable<Wochentag> DaysOfWeek => Enum.GetValues(typeof(Wochentag)).Cast<Wochentag>();

        public static void EmergencyMail(int? maxEntfernung)
        {
            var empfänger = maxEntfernung is null
                ? DataContainer.Versammlungen.Where(x => x.Kreis == DataContainer.MeineVersammlung.Kreis)
                : DataContainer.Versammlungen.Where(x => x.Entfernung <= maxEntfernung);

            var mailadressen = "------------------------------\nListe der Mailadressen\n------------------------------\n";
            var mailadressenFuß = "----- Koordinatoren ohne Mailadresse -----\n";
            var jwpubadressen = "------------------------------\nListe der JwPub-Adressen\n------------------------------\n";
            var jwpubadressenFuß = "----- Koordinatoren ohne JwPub-Adresse -----\n";
            foreach (var mail in empfänger)
            {
                if (!string.IsNullOrEmpty(mail.KoordinatorMail))
                    mailadressen += $"{mail.Koordinator} <{mail.KoordinatorMail}>;" + Environment.NewLine;
                else
                    mailadressenFuß += mail.Koordinator + Environment.NewLine;
                if (!string.IsNullOrEmpty(mail.KoordinatorJw))
                    jwpubadressen += $"{mail.Koordinator} <{mail.KoordinatorJw}>;" + Environment.NewLine;
                else
                    jwpubadressenFuß += mail.Koordinator + Environment.NewLine;
            }

            var dialog = new LeerDialog();
            var data = (LeerViewModel)dialog.DataContext;
            data.Titel = "Mail an alle Koordinatoren";
            data.ShowCopyButton = true;
            data.ShowCloseButton = true;
            data.ShowSaveButton = false;
            data.Text = jwpubadressen + Environment.NewLine
                      + jwpubadressenFuß + Environment.NewLine
                      + mailadressen + Environment.NewLine
                      + mailadressenFuß;

            dialog.ShowDialog();
            ActivityAddItem.SendMail(data.Text, maxEntfernung);
        }

        public static void CalculateRoute(bool alle)
        {
            var start = DataContainer.MeineVersammlung;
            var end = DataContainer.Versammlungen.Where(x => x != start);
            if (!alle)
                end = end.Where(x => x.Entfernung == 0);
            var erfolgreich = 0;
            var fehler = 0;
            foreach (var ziel in end)
            {
                var km = GeoApi.GetDistance(start, ziel);
                if (km != null)
                {
                    ziel.Entfernung = (int)km;
                    erfolgreich++;
                }
                else
                    fehler++;
            }
            ThemedMessageBox.Show("Entfernungsberechnung", $"Es wurden {erfolgreich} Entfernungen berechnet und eingetragen. {fehler} Berechnungen haben nicht geklappt, es wurde die Entfernung 0km eingetragen.");
        }

        public static bool ThemeIsDark
        {
            get => Properties.Settings.Default.ThemeIsDark;
            set
            {
                Properties.Settings.Default.ThemeIsDark = value;
                Properties.Settings.Default.Save();
                var theme = value ? "MetropolisDark" : "Office2019White";
                var dict = value ? "Dark.xaml" : "Light.xaml";
                ApplyResources(dict);
                ApplicationThemeHelper.ApplicationThemeName = theme;
            }
        }

        public string SelectedTheme
        {
            get => _selectedTheme;
            set
            {
                if (_selectedTheme == value)
                    return;

                _selectedTheme = value;
                if (ThemeIsDark != (_selectedTheme == "Dunkel"))
                {
                    ThemeIsDark = _selectedTheme == "Dunkel";
                }

                RaisePropertyChanged();
            }
        }

        public List<string> Themes { get; } = new List<string>(2) { "Dunkel", "Hell" };

        private static void ApplyResources(string src)
        {
            var a = Application.Current as App;
            var dict = new ResourceDictionary() { Source = new Uri("Themes/" +src, UriKind.Relative) };
            foreach (var mergeDict in dict.MergedDictionaries)
            {
                a?.Resources.MergedDictionaries.Add(mergeDict);
            }

            foreach (var key in dict.Keys)
            {
                if (a != null) a.Resources[key] = dict[key];
            }
        }
    }
}