using DevExpress.Mvvm;
using DevExpress.Xpf.Core;
using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Vortragsmanager.Core;

namespace Vortragsmanager.Views
{
    public class EinstellungenViewModel : ViewModelBase
    {
        private string datenbank;

        public EinstellungenViewModel()
        {
            ExcelFileDialogCommand = new DelegateCommand(ExcelFileDialog);
            SearchDatabaseCommand = new DelegateCommand<string>(SearchDatabase);
            SearchUpdateCommand = new DelegateCommand(SearchUpdate);
            UpdateSpeakerFromExcelCommand = new DelegateCommand(UpdateSpeakerFromExcel);
            EmergencyMailCommand = new DelegateCommand(EmergencyMail);
            CalculateRouteCommand = new DelegateCommand<bool>(CalculateRoute);
            Datenbank = Properties.Settings.Default.sqlite;
        }

        private string _importExcelFile = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\Liste der Vortragskoordinatoren.xlsx";

        public DelegateCommand ExcelFileDialogCommand { get; private set; }

        public DelegateCommand UpdateSpeakerFromExcelCommand { get; private set; }

        public DelegateCommand EmergencyMailCommand { get; private set; }

        public DelegateCommand<bool> CalculateRouteCommand { get; private set; }

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
                    IoSqlite.SaveContainer(saveDialog.FileName);
                    Properties.Settings.Default.sqlite = saveDialog.FileName;
                    Properties.Settings.Default.Save();
                }

                saveDialog.Dispose();
            }
        }

        public bool UpdatesEnabled
        {
            get
            {
                return Properties.Settings.Default.SearchForUpdates;
            }
            set
            {
                Properties.Settings.Default.SearchForUpdates = value;
                Properties.Settings.Default.Save();
                RaisePropertyChanged();
            }
        }

        public DelegateCommand SearchUpdateCommand { get; private set; }

        public static void SearchUpdate()
        {
            Updater.CheckForUpdatesForce();
        }

        public static void EmergencyMail()
        {
            var mailadressen = "------------------------------\nListe der Mailadressen\n------------------------------\n";
            var jwpubadressen = "------------------------------\nListe der JwPub-Adressen\n------------------------------\n";
            foreach (var mail in Core.DataContainer.Versammlungen.Where(x => x.Kreis == DataContainer.MeineVersammlung.Kreis))
            {
                if (!string.IsNullOrEmpty(mail.KoordinatorMail))
                    mailadressen += mail.KoordinatorMail + ";" + Environment.NewLine;
                if (!string.IsNullOrEmpty(mail.KoordinatorJw))
                    jwpubadressen += mail.KoordinatorJw + ";" + Environment.NewLine;
            }

            var dialog = new leerDialog();
            var data = (LeerViewModel)dialog.DataContext;
            data.Titel = "Mail an alle Koordinatoren";
            data.ShowCopyButton = true;
            data.ShowCloseButton = true;
            data.ShowSaveButton = false;
            data.Text = jwpubadressen + Environment.NewLine + Environment.NewLine + mailadressen;

            dialog.ShowDialog();
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