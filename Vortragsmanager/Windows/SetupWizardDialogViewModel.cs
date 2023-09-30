using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Forms;
using DevExpress.Mvvm;
using DevExpress.Xpf.Core;
using Vortragsmanager.DataModels;
using Vortragsmanager.Enums;
using Vortragsmanager.Interface;
using Vortragsmanager.Module;

namespace Vortragsmanager.Windows
{
    public class SetupWizardDialogViewModel : ViewModelBase
    {
        public SetupWizardDialogViewModel()
        {
            CloseCommand = new DelegateCommand<ICloseable>(Schließen);
            ExcelFileDialogCommand = new DelegateCommand(ExcelFileDialog);
            ExcelImportierenKoordinatorenCommand = new DelegateCommand(ExcelImportierenKoordinatoren);
            ExcelImportierenPlannungCommand = new DelegateCommand(ExcelImportierenPlannung);
            ExcelImportierenPlannungExternCommand = new DelegateCommand(ExcelImportierenPlannungExtern);
            ExcelImportierenRednerCommand = new DelegateCommand(ExcelImportierenRedner);
            OpenExcelExampleCommand = new DelegateCommand<string>(OpenExcelExample);
            VortragsmanagerdateiLadenCommand = new DelegateCommand(VortragsmanagerdateiLaden);
            DatabaseFileDialogCommand = new DelegateCommand(DatabaseFileDialog);
            CanGoNext = true;
            DatenbankÖffnenHeight = new GridLength(0, GridUnitType.Pixel);
        }

        private int _selectedIndex;
        private string _importExcelFile = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\Versammlungen.xlsx";
        private bool _canGoNext;
        private bool _datenbankÖffnenChecked;
        private bool _vplanungChecked = true;
        private string _importFile = string.Empty;
        private Conregation _deineVersammlung;
        private bool _meineVersammlungIstImportiertChecked = true;
        private bool? _dialogResult;

        public DelegateCommand ExcelImportierenPlannungCommand { get; }
        public DelegateCommand ExcelImportierenPlannungExternCommand { get; }
        public DelegateCommand<ICloseable> CloseCommand { get; }
        public DelegateCommand VortragsmanagerdateiLadenCommand { get; }
        public DelegateCommand DatabaseFileDialogCommand { get; }
        public DelegateCommand ExcelImportierenKoordinatorenCommand { get; }
        public DelegateCommand ExcelImportierenRednerCommand { get; }
        public DelegateCommand<string> OpenExcelExampleCommand { get; }
        public DelegateCommand ExcelFileDialogCommand { get; }

        public ObservableCollection<Conregation> VersammlungsListe => DataContainer.Versammlungen;

        public ObservableCollection<string> ImportierteJahreliste { get; } = new ObservableCollection<string>();

        public ObservableCollection<Kreis> ImportierteKoordinatorenliste { get; } = new ObservableCollection<Kreis>();

        public ObservableCollection<string> ImportierteRednerliste { get; } = new ObservableCollection<string>();
               
        public bool IsFinished { get; set; }

        public bool DemoChecked { get; set; }

        public bool NeuBeginnenChecked { get; set; }

        public bool? DialogResult { get => _dialogResult; set => SetValue(ref _dialogResult, value); }

        public bool MeineVersammlungIstImportiertChecked
        {
            get => _meineVersammlungIstImportiertChecked;
            set
            {
                _meineVersammlungIstImportiertChecked = value;
                CanGoNext = (_deineVersammlung != null || !_meineVersammlungIstImportiertChecked);
                RaisePropertyChanged();
            }
        }

        public Conregation DeineVersammlung
        {
            get => _deineVersammlung;
            set
            {
                _deineVersammlung = value;
                CanGoNext = (_deineVersammlung != null || !_meineVersammlungIstImportiertChecked);
                if (value != null)
                    DataContainer.MeineVersammlung = _deineVersammlung;
            }
        }

        public int SelectedIndex
        {
            get => _selectedIndex;
            set
            {
                _selectedIndex = value;
                CheckWizardPage();
                RaisePropertyChanged();
            }
        }

        public bool CanGoNext
        {
            get => _canGoNext;
            set
            {
                _canGoNext = value;
                RaisePropertyChanged();
            }
        }

        public bool DatenbankÖffnenChecked
        {
            get => _datenbankÖffnenChecked;
            set
            {
                _datenbankÖffnenChecked = value;
                DatenbankÖffnenHeight = value ? new GridLength(1, GridUnitType.Star) : new GridLength(0, GridUnitType.Pixel);
                RaisePropertyChanged(nameof(DatenbankÖffnenHeight));
            }
        }

        public bool VplanungChecked
        {
            get => _vplanungChecked;
            set
            {
                _vplanungChecked = value;
                VplanungCheckedHeight = value ? new GridLength(1, GridUnitType.Star) : new GridLength(0, GridUnitType.Pixel);
                RaisePropertyChanged(nameof(VplanungCheckedHeight));
            }
        }

        public GridLength DatenbankÖffnenHeight
        {
            get;
            set;
        }

        public GridLength VplanungCheckedHeight
        {
            get;
            set;
        }

        public string ImportFile
        {
            get => _importFile;
            set
            {
                _importFile = value;
                RaisePropertyChanged();
            }
        }

        public string ImportExcelFile
        {
            get => _importExcelFile;
            set
            {
                _importExcelFile = value;
                RaisePropertyChanged();
            }
        }


        public static void Schließen(ICloseable window)
        {
            if (window != null)
                window.Close();
        }

        public void CheckWizardPage()
        {
            CanGoNext = true;

            switch (SelectedIndex)
            {
                //0 = Grundsätzliche Auswahl wie man starten soll, hier gibt es keine Optionen
                case 0:
                    break;
                // 1 = Erste Seite nach der Auswahl.
                // Option Demo+Leer haben keine weiteren Optionen. Die Tasks werden gestartet und der Wizard geschlossen
                // Option Datenbank öffnen hat noch diese 1 Wizard-Seite, deshalb ist Next daktiviert
                // Option Excel-Import ist der eigentliche "Echte" Wizard-Import.
                case 1:
                    if (DemoChecked)
                    {
                        var quelle = $"{Helper.Helper.TemplateFolder}demo.sqlite3";
                        var ziel = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\demo.sqlite3";
                        File.Copy(quelle, ziel, true);
                        IoSqlite.ReadContainer(ziel);
                        Initialize.DemoAktualisieren();
                        Helper.Helper.GlobalSettings.sqlite = ziel;

                        IsFinished = true;
                        DialogResult = true;
                    }
                    else if (NeuBeginnenChecked)
                    {
                        var con = new Conregation() { Name = "Meine Versammlung", Kreis = 1, Id = 1 };
                        DataContainer.Versammlungen.Add(con);
                        DataContainer.MeineVersammlung = con;
                        DataContainer.IsInitialized = true;
                        DataContainer.IsDemo = false;
                        Helper.Helper.GlobalSettings.sqlite = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\vortragsmanager.sqlite3";
                        IoSqlite.SaveContainer(Helper.Helper.GlobalSettings.sqlite, false);

                        IsFinished = true;
                        DialogResult = true;
                    }
                    else if (DatenbankÖffnenChecked)
                    {
                        //Hier gibt es noch keine Optionen die dynamisch gesetzt werden müssen.
                        CanGoNext = false;
                    }
                    else if (VplanungChecked)
                    {
                        //Hier gibt es noch keine Optionen die dynamisch gesetzt werden müssen.
                        CanGoNext = true;
                    }
                    break;
                //Seite "VERSAMMLUNGEN UND KOORDINATOREN"
                case 2:
                    ImportExcelFile = "Versammlungen.xlsx";
                    DataContainer.Versammlungen.Clear();
                    ImportierteKoordinatorenliste.Clear();
                    CanGoNext = true;
                    break;
                //Seite "WÄHLE DEINE VERSAMMLUNG
                case 3:
                    //VersammlungsListe = Core.DataContainer.Versammlungen;
                    CanGoNext = false;
                    break;
                //Seite REDNER
                case 4:
                    ImportExcelFile = "Redner.xlsx";
                    DataContainer.Redner.Clear();
                    ImportierteRednerliste.Clear();
                    CheckMeineVersammlung();
                    CanGoNext = true;
                    break;
                //Seite DEINE PLANUNG
                case 5:
                    ImportExcelFile = "MeinPlan.xlsx";
                    DataContainer.MeinPlan.Clear();
                    ImportierteJahreliste.Clear();
                    break;
                //Seite EXTERNE PLANUNG
                case 6:
                    ImportExcelFile = "Extern.xlsx";
                    DataContainer.ExternerPlan.Clear();
                    ImportierteJahreliste.Clear();
                    break;
                //Seite FINISH
                default:
                    CanGoNext = true;
                    break;
            }
        }

        public void OpenExcelExample(string dateiname)
        {
            var quelle = $"{Helper.Helper.TemplateFolder}{dateiname}";
            System.Diagnostics.Process.Start(quelle);
        }

        public void VortragsmanagerdateiLaden()
        {
            IoSqlite.ReadContainer(ImportFile);
            Helper.Helper.GlobalSettings.sqlite = ImportFile;
            Helper.Helper.GlobalSettings.Save();
            IsFinished = true;
            DialogResult = true;
        }

        public void DatabaseFileDialog()
        {
            if (string.IsNullOrEmpty(ImportFile))
                ImportFile = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "Planungsdatei.sqlite3";

            var fi = new FileInfo(ImportFile);

            var openDialog = new OpenFileDialog
            {
                Filter = Properties.Resources.DateifilterSqlite,
                FilterIndex = 1,
                RestoreDirectory = false,
                InitialDirectory = fi.DirectoryName,
                FileName = fi.Name,
                CheckFileExists = true
            };

            if (openDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {

                ImportFile = openDialog.FileName;
            }

            openDialog.Dispose();
        }
              
        public void ExcelImportierenKoordinatoren()
        {
            if (!File.Exists(ImportExcelFile))
            {
                ThemedMessageBox.Show(
                    Properties.Resources.Achtung,
                    $"Die Datei '{ImportExcelFile}' wurde nicht gefunden.",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                return;
            }

            //einlesen der Excel-Datei
            if (!IoExcel.Import.Versammlung(ImportExcelFile, false))
                return;

            ImportierteKoordinatorenliste.Clear();
            foreach (var k in DataContainer.Versammlungen.GroupBy(info => info.Kreis)
                        .Select(group => new Kreis(group.Key, group.Count()))
                        .OrderBy(x => x.Anzahl))
                ImportierteKoordinatorenliste.Add(k);
        }

        public void ExcelImportierenRedner()
        {
            if (!File.Exists(ImportExcelFile))
            {
                ThemedMessageBox.Show(
                    Properties.Resources.Achtung,
                    $"Die Datei '{ImportExcelFile}' wurde nicht gefunden.",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                return;
            }

            //einlesen der Excel-Datei
            if (!IoExcel.Import.Redner(ImportExcelFile, false))
                return;

            ImportierteRednerliste.Clear();
            foreach (var k in DataContainer.Redner.GroupBy(info => info.Versammlung)
                        .Select(group => new { Versammlung = group.Key.Name, Anzahl = group.Count() })
                        .OrderBy(x => x.Versammlung))
            {
                ImportierteRednerliste.Add($"{k.Versammlung} ({k.Anzahl} Redner)");
            }
        }

        private string _lastFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

        public void ExcelFileDialog()
        {
            string dir;
            string fn;
            if (!ImportExcelFile.Contains(@"\"))
            {
                dir = _lastFolder;
                fn = string.IsNullOrEmpty(ImportExcelFile) ? "Excelfile.xlsx" : ImportExcelFile;
            }
            else
            {
                var fi = new FileInfo(ImportExcelFile);
                dir = fi.Directory != null && fi.Directory.Exists ? fi.DirectoryName : Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                fn = fi.Name;
            }
            var openDialog = new OpenFileDialog
            {
                Filter = Properties.Resources.DateifilterExcel,
                FilterIndex = 1,
                RestoreDirectory = false,
                InitialDirectory = dir,
                FileName = fn,
                CheckFileExists = true
            };

            if (openDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                ImportExcelFile = openDialog.FileName;
                _lastFolder = new FileInfo(ImportExcelFile).DirectoryName;
            }

            openDialog.Dispose();
        }

        private void CheckMeineVersammlung()
        {
            //Wurde auf der vorigen Seite eine Versammlung ausgewählt?
            if (_deineVersammlung != null)
                return;

            var c = DataContainer.ConregationFindOrAdd("Meine Versammlung");
            c.Zeit.Add(DateTime.Today.Year, Wochentag.Sonntag, "Unbekannt");
            DeineVersammlung = c;
        }

        public void ExcelImportierenPlannung()
        {
            if (!File.Exists(ImportExcelFile))
            {
                ThemedMessageBox.Show(
                    $"Die Datei '{ImportExcelFile}' wurde nicht gefunden.",
                    Properties.Resources.Achtung,
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                return;
            }

            //einlesen der Excel-Datei
            if (!IoExcel.Import.EigenePlanungen(ImportExcelFile))
                return;

            ImportierteJahreliste.Clear();
            foreach (var j in DataContainer.MeinPlan.GroupBy(x => x.Kw / 100))
            {
                ImportierteJahreliste.Add(j.Key.ToString(Helper.Helper.German));
            }

            RaisePropertyChanged(nameof(ImportierteJahreliste));
            CanGoNext = true;
            IsFinished = true;
        }

        public void ExcelImportierenPlannungExtern()
        {
            if (!File.Exists(ImportExcelFile))
            {
                ThemedMessageBox.Show(
                    $"Die Datei '{ImportExcelFile}' wurde nicht gefunden.",
                    Properties.Resources.Achtung,
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                return;
            }

            //einlesen der Excel-Datei
            if (!IoExcel.Import.ExternePlanungen(ImportExcelFile))
                return;

            ImportierteJahreliste.Clear();
            foreach (var j in DataContainer.ExternerPlan.GroupBy(x => x.Kw / 100))
            {
                ImportierteJahreliste.Add(j.Key.ToString(Helper.Helper.German));
            }

            RaisePropertyChanged(nameof(ImportierteJahreliste));
            CanGoNext = true;
            IsFinished = true;
        }
    }

    public class Kreis
    {
        public Kreis(int nr, int anzahl)
        {
            Nr = nr;
            Anzahl = anzahl;
        }

        public int Nr { get; }

        public int Anzahl { get; set; }

        public override string ToString()
        {
            return $"Kreis {Nr} ({Anzahl} Versammlungen)";
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            if (Nr != ((Kreis) obj).Nr)
                return false;

            return true;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                // Choose large primes to avoid hashing collisions
                const int hashingBase = (int)2166136261;
                const int hashingMultiplier = 16777619;

                var hash = hashingBase;
                hash = (hash * hashingMultiplier) ^ (Nr.GetHashCode());
                return hash;
            }
        }
    }
}