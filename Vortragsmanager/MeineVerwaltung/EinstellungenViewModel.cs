using DevExpress.Mvvm;
using DevExpress.Xpf.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using Vortragsmanager.Core;
using Vortragsmanager.Datamodels;

namespace Vortragsmanager.MeineVerwaltung
{
    public class EinstellungenViewModel : ViewModelBase
    {
        private string datenbank;

        public EinstellungenViewModel()
        {
            ExcelFileDialogCommand = new DelegateCommand(ExcelFileDialog);
            SearchDatabaseCommand = new DelegateCommand<string>(SearchDatabase);
            UpdateSpeakerFromExcelCommand = new DelegateCommand(UpdateSpeakerFromExcel);
            EmergencyMailCommand = new DelegateCommand<int?>(EmergencyMail);
            CalculateRouteCommand = new DelegateCommand<bool>(CalculateRoute);
            ShowChangelogCommand = new DelegateCommand(ShowChangelog);
            Datenbank = Properties.Settings.Default.sqlite;
        }

        private string _importExcelFile = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\Liste der Vortragskoordinatoren.xlsx";

        public DelegateCommand ExcelFileDialogCommand { get; private set; }

        public DelegateCommand UpdateSpeakerFromExcelCommand { get; private set; }

        public DelegateCommand<int?> EmergencyMailCommand { get; private set; }

        public DelegateCommand<bool> CalculateRouteCommand { get; private set; }

        public DelegateCommand ShowChangelogCommand { get; private set; }

        public string ImportExcelFile
        {
            get
            {
                return _importExcelFile;
            }
            set
            {
                _importExcelFile = value;
                RaisePropertyChanged();
            }
        }

        public void ShowChangelog()
        {
            Initialize.ShowChanges(0);
        }

        public void ExcelFileDialog()
        {
            var fi = new FileInfo(ImportExcelFile);
            var dir = fi.Directory.Exists ? fi.DirectoryName : Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

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
            get
            {
                return datenbank;
            }
            set
            {
                datenbank = value;
                RaisePropertyChanged();
            }
        }

        public DelegateCommand<string> SearchDatabaseCommand { get; private set; }

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

            Helper.GlobalSettings.RefreshTitle();
        }

        public bool SaveBackup
        {
            get
            {
                return Properties.Settings.Default.SaveBackups;
            }
            set
            {
                Properties.Settings.Default.SaveBackups = value;
                Properties.Settings.Default.Save();
                RaisePropertyChanged();
            }
        }


        public bool ShowChangelogState
        {
            get
            {
                return !Properties.Settings.Default.HideChangelog;
            }
            set
            {
                Properties.Settings.Default.HideChangelog = !value;
                Properties.Settings.Default.Save();
                RaisePropertyChanged();
            }
        }


        public bool DashboardShowDetails
        {
            get
            {
                return Properties.Settings.Default.DashboardShowDetails;
            }
            set
            {
                Properties.Settings.Default.DashboardShowDetails = value;
                Properties.Settings.Default.Save();
                RaisePropertyChanged();
            }
        }
        
        public bool ShowActivityButtons
        {
            get
            {
                return Properties.Settings.Default.ShowActivityButtons;
            }
            set
            {
                Properties.Settings.Default.ShowActivityButtons = value;
                Properties.Settings.Default.Save();
                RaisePropertyChanged();
            }
        }
        public int SelectedLogLevel
        {
            get
            {
                return Properties.Settings.Default.LogLevel;
            }
            set
            {
                Log.Start((LogLevel)value);
                RaisePropertyChanged();
            }
        }

        public string LogFolder
        {
            get
            {
                return Properties.Settings.Default.LogFolder;
            }
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

        public DayOfWeeks ConregationDayOfWeek
        {
            get
            {
                return Helper.Wochentag;
            }
            set
            {
                Helper.Wochentag = value;
                var zeititem = DataContainer.MeineVersammlung.Zeit.Get(DateTime.Today.Year);
                zeititem.Tag = Helper.Wochentag;
                RaisePropertyChanged();
            }
        }

        public IEnumerable<DayOfWeeks> DaysOfWeek => Enum.GetValues(typeof(DayOfWeeks)).Cast<DayOfWeeks>();

        public static void EmergencyMail(int? maxEntfernung)
        {
            IEnumerable<Conregation> empfänger;
            if (maxEntfernung is null)
                empfänger = DataContainer.Versammlungen.Where(x => x.Kreis == DataContainer.MeineVersammlung.Kreis);
            else
                empfänger = DataContainer.Versammlungen.Where(x => x.Entfernung <= maxEntfernung);

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

            var dialog = new Views.leerDialog();
            var data = (Views.LeerViewModel)dialog.DataContext;
            data.Titel = "Mail an alle Koordinatoren";
            data.ShowCopyButton = true;
            data.ShowCloseButton = true;
            data.ShowSaveButton = false;
            data.Text = jwpubadressen + Environment.NewLine
                      + jwpubadressenFuß + Environment.NewLine
                      + mailadressen + Environment.NewLine
                      + mailadressenFuß;

            dialog.ShowDialog();
            ActivityLog.AddActivity.SendMail(data.Text, maxEntfernung);
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
    }
}