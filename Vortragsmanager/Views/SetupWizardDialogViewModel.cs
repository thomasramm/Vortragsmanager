using DevExpress.Mvvm;
using DevExpress.Xpf.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Forms;
using Vortragsmanager.Models;
using System.Linq;

namespace Vortragsmanager.Views
{
    public class SetupWizardDialogViewModel : ViewModelBase
    {
        public SetupWizardDialogViewModel()
        {
            CloseCommand = new DelegateCommand<ICloseable>(Schließen);
            ExcelFileDialogCommand = new DelegateCommand(ExcelFileDialog);
            ExcelImportierenKoordinatorenCommand = new DelegateCommand(ExcelImportierenKoordinatoren);
            ExcelImportierenPlannungCommand = new DelegateCommand(ExcelImportierenPlannung);
            CanGoNext = true;
        }

        private int _selectedIndex;
        private string _importExcelFile = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\Liste der Vortragskoordinatoren.xlsx";
        private bool _canGoNext;

        public int SelectedIndex
        {
            get
            {
                return _selectedIndex;
            }
            set
            {
                _selectedIndex = value;
                CheckWizardPage();
                RaisePropertyChanged();
            }
        }

        public DelegateCommand<ICloseable> CloseCommand { get; private set; }

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
                case 1:
                    foreach (var vers in Core.DataContainer.Versammlungen)
                    {
                        if (ImportierteKoordinatorenliste.Any(x => x.Nr == vers.Kreis))
                        {
                            var item = ImportierteKoordinatorenliste.First(x => x.Nr == vers.Kreis);
                            item.Anzahl += 1;
                        }
                        else
                            ImportierteKoordinatorenliste.Add(new Kreis(vers.Kreis));
                    }
                    CanGoNext = ImportierteKoordinatorenliste.Count > 0;
                    break;
                case 2:
                    //VersammlungsListe = Core.DataContainer.Versammlungen;
                    CanGoNext = (DeineVersammlung != null);
                    break;
                case 3:
                    CanGoNext = true;
                    break;
                default:
                    CanGoNext = true;
                    break;
            }
        }
        
        public bool CanGoNext
        {
            get 
            {
                return _canGoNext;
            }
            set
            {
                _canGoNext = value;
                RaisePropertyChanged();
            }
        }

        #region Koordinatoren
        public DelegateCommand ExcelImportierenKoordinatorenCommand { get; private set; }

        public DelegateCommand ExcelFileDialogCommand { get; private set; }

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

        public ObservableCollection<Kreis> ImportierteKoordinatorenliste { get; } = new ObservableCollection<Kreis>();

        public void ExcelImportierenKoordinatoren()
        {
            if (!File.Exists(ImportExcelFile))
            {
                ThemedMessageBox.Show(
                    $"Die Datei '{ImportExcelFile}' wurde nicht gefunden.",
                    "Achtung!",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                return;
            }

            //einlesen der Excel-Datei
            if (!Core.IoExcel.Vplanung.ImportKoordinatoren(ImportExcelFile))
                return;

            //prüfen ob Kreis bereits importiert wurde
            if (ImportierteKoordinatorenliste.Any(x => x.Nr == Core.IoExcel.Vplanung.Kreis))
            {
                if (ThemedMessageBox.Show(
                    "Achtung!",
                    $"Der Kreis '{Core.IoExcel.Vplanung.Kreis}' wurde bereits importiert. Bestehende Daten löschen und neu importieren?",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning) != MessageBoxResult.Yes)
                    return;
                
                //löschen des bereits erfolgten imports für den Kreis
                var anzahlVersammlungen = Core.DataContainer.Versammlungen.Count;
                for (int i = anzahlVersammlungen-1; i >0; i--)
                {
                    if (Core.DataContainer.Versammlungen[i].Kreis == Core.IoExcel.Vplanung.Kreis)
                        Core.DataContainer.Versammlungen.RemoveAt(i);
                }
                ImportierteKoordinatorenliste.Remove(new Kreis(Core.IoExcel.Vplanung.Kreis));
                
            }
            foreach (var item in Core.IoExcel.Vplanung.Conregations)
            {
                Core.DataContainer.Versammlungen.Add(item);
            }
            ImportierteKoordinatorenliste.Add(new Kreis(Core.IoExcel.Vplanung.Kreis, Core.IoExcel.Vplanung.Conregations.Count));
            CanGoNext = true;
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
               
        #endregion

        #region DeineVersammlung

        public DelegateCommand ExcelImportierenPlannungCommand { get; private set; }

        public ObservableCollection<Conregation> VersammlungsListe => Core.DataContainer.Versammlungen;

        private Conregation _deineVersammlung;
        public Conregation DeineVersammlung 
        {
            get
            {
                return _deineVersammlung;
            }
            set
            {
                _deineVersammlung = value;
                CanGoNext = (_deineVersammlung != null);
                if (value != null)
                    Core.DataContainer.MeineVersammlung = _deineVersammlung;
            }
        }

        #endregion

        #region Planungen importieren

        public List<string> ImportierteJahreliste { get; } = new List<string>();

        public void ExcelImportierenPlannung()
        {
            if (!File.Exists(ImportExcelFile))
            {
                ThemedMessageBox.Show(
                    $"Die Datei '{ImportExcelFile}' wurde nicht gefunden.",
                    "Achtung!",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                return;
            }

            //einlesen der Excel-Datei
            Core.IoExcel.Vplanung.ImportEigenePlanungen(ImportExcelFile);
            Core.IoExcel.Vplanung.ImportRednerPlanungen(ImportExcelFile);

            var jahr = Core.IoExcel.Vplanung.MeinPlan.Select(x => x.Datum.Year);

            bool hasMatch = ImportierteJahreliste.Any(x => jahr.Any(y => y.ToString(Core.DataContainer.German) == x));

            //prüfen ob bereits importiert wurde
            if (hasMatch)
            {
                if (ThemedMessageBox.Show(
                    $"Es existieren bereits Einträge für das gewählte Jahr. Ereignisse zusammenführen? (Bestehende Einträge werden überschrieben)?",
                    "Achtung!",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning) != MessageBoxResult.Yes)
                    return;

            }

            foreach (var item in Core.IoExcel.Vplanung.ExternerPlan)
            {
                if (hasMatch) //ich muss das nur prüfen wenn hasMatch=true ist, sonst gibt es keine Einträge im Zeitraum
                {
                    var exist = Core.DataContainer.ExternerPlan.FirstOrDefault(x => x.Datum == item.Datum);
                    if (exist != null)
                        Core.DataContainer.ExternerPlan.Remove(exist);
                }
                Core.DataContainer.ExternerPlan.Add(item);
            }

            foreach (var item in Core.IoExcel.Vplanung.MeinPlan)
            {
                if (hasMatch) //ich muss das nur prüfen wenn hasMatch=true ist, sonst gibt es keine Einträge im Zeitraum
                {
                    var exist = Core.DataContainer.MeinPlan.FirstOrDefault(x => x.Datum == item.Datum);
                    if (exist != null)
                        Core.DataContainer.MeinPlan.Remove(exist);
                }
                Core.DataContainer.MeinPlan.Add(item);
            }
            foreach (var item in jahr)
            {
                var neu = item.ToString(Core.DataContainer.German);
                if (!ImportierteJahreliste.Contains(neu))
                    ImportierteJahreliste.Add(neu);
            }
            
            RaisePropertyChanged(nameof(ImportierteJahreliste));
            CanGoNext = true;
            IsFinished = true;
        }

        #endregion

        public bool IsFinished { get; set; }

        public class Kreis
        {
            public Kreis(int nr)
            {
                Nr = nr;
                Anzahl = 1;
            }

            public Kreis(int nr, int anzahl)
            {
                Nr = nr;
                Anzahl = anzahl;
            }

            public int Nr { get; set; }

            public int Anzahl { get; set; }

            public override string ToString()
            {
                return $"Kreis {Nr} ({Anzahl} Versammlungen)";
            }

            public override bool Equals(object obj)
            {
                if (obj == null)
                    return false;
                if (Nr != (obj as Kreis).Nr)
                    return false;

                return true;
            }
        }
    }
}