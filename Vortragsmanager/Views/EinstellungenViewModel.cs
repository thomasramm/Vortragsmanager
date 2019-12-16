using System.IO;
using System.Windows.Forms;
using DevExpress.Mvvm;
using Vortragsmanager.Core;

namespace Vortragsmanager.Views
{
    public class EinstellungenViewModel : ViewModelBase
    {
        private const string V = "SQLite Datenbank (*.sqlite3)|*.sqlite3|Alle Dateien (*.*)|*.*";
        private string datenbank;

        public EinstellungenViewModel()
        {
            SearchDatabaseCommand = new DelegateCommand<string>(SearchDatabase);
            SearchUpdateCommand = new DelegateCommand(SearchUpdate);
            Datenbank = Properties.Settings.Default.sqlite;
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

        public bool UpdatesEnabled {
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
                
        public void SearchUpdate()
        {
            Updater.CheckForUpdatesForce();
        }
    }
}
