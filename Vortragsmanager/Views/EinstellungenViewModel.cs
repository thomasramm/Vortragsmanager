using DevExpress.Mvvm;
using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Vortragsmanager.Core;

namespace Vortragsmanager.Views
{
    public class EinstellungenViewModel : ViewModelBase
    {
        private const string V = "SQLite Datenbank (*.sqlite3)|*.sqlite3|Alle Dateien (*.*)|*.*";
        private string datenbank;

        public EinstellungenViewModel()
        {
            ExcelFileDialogCommand = new DelegateCommand(ExcelFileDialog);
            SearchDatabaseCommand = new DelegateCommand<string>(SearchDatabase);
            SearchUpdateCommand = new DelegateCommand(SearchUpdate);
            UpdateSpeakerFromExcelCommand = new DelegateCommand(UpdateSpeakerFromExcel);
            EmergencyMailCommand = new DelegateCommand(EmergencyMail);
            Datenbank = Properties.Settings.Default.sqlite;
        }

        private string _importExcelFile = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\Liste der Vortragskoordinatoren.xlsx";

        public DelegateCommand ExcelFileDialogCommand { get; private set; }

        public DelegateCommand UpdateSpeakerFromExcelCommand { get; private set; }

        public DelegateCommand EmergencyMailCommand { get; private set; }

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
                Filter = "Excel Datei (*.xlsx)|*.xlsx|Alle Dateien (*.*)|*.*",
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
                    Filter = V,
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
                    Filter = V,
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

        public void EmergencyMail()
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
    }
}