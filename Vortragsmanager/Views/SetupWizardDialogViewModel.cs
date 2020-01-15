﻿using DevExpress.Mvvm;
using DevExpress.Xpf.Core;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Forms;
using Vortragsmanager.Core;
using Vortragsmanager.Models;

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
            VortragsmanagerdateiLadenCommand = new DelegateCommand<ICloseable>(VortragsmanagerdateiLaden);
            DatabaseFileDialogCommand = new DelegateCommand(DatabaseFileDialog);
            NeuBeginnenCommand = new DelegateCommand<ICloseable>(NeuBeginnen);
            CanGoNext = true;
            DatenbankÖffnenHeight = new GridLength(0, GridUnitType.Pixel);
            NeuBeginnenHeight = new GridLength(0, GridUnitType.Pixel);
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

        public DelegateCommand<ICloseable> NeuBeginnenCommand { get; private set; }

        public static void Schließen(ICloseable window)
        {
            if (window != null)
                window.Close();
        }

        private void NeuBeginnen(ICloseable window)
        {
            var con = new Conregation() { Name = "Meine Versammlung", Kreis = 309, Id = 1 };
            DataContainer.Versammlungen.Add(con);
            DataContainer.MeineVersammlung = con;
            DataContainer.IsInitialized = true;
            IsFinished = true;
            Schließen(window);
        }

        public void CheckWizardPage()
        {
            CanGoNext = true;

            switch (SelectedIndex)
            {
                case 1:
                    if (VplanungChecked)
                    {
                        CanGoNext = true;
                    }
                    else if (DatenbankÖffnenChecked)
                    {
                        CanGoNext = false;
                    }
                    else if (NeuBeginnenChecked)
                    {
                        CanGoNext = false;
                    }
                    break;

                case 2:
                    foreach (var vers in DataContainer.Versammlungen)
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

                case 3:
                    //VersammlungsListe = Core.DataContainer.Versammlungen;
                    CanGoNext = (DeineVersammlung != null);
                    break;

                case 4:
                    CanGoNext = ImportierteJahreliste.Count > 0;
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

        private bool _datenbankÖffnenChecked = false;

        public bool DatenbankÖffnenChecked
        {
            get { return _datenbankÖffnenChecked; }
            set
            {
                _datenbankÖffnenChecked = value;
                DatenbankÖffnenHeight = value ? new GridLength(1, GridUnitType.Star) : new GridLength(0, GridUnitType.Pixel);
                RaisePropertyChanged(nameof(DatenbankÖffnenHeight));
            }
        }

        private bool _vplanungChecked = true;

        public bool VplanungChecked
        {
            get { return _vplanungChecked; }
            set
            {
                _vplanungChecked = value;
                VplanungCheckedHeight = value ? new GridLength(1, GridUnitType.Star) : new GridLength(0, GridUnitType.Pixel);
                RaisePropertyChanged(nameof(VplanungCheckedHeight));
            }
        }

        private bool _neuBeginnenChecked = false;

        public bool NeuBeginnenChecked
        {
            get
            {
                return _neuBeginnenChecked;
            }
            set
            {
                _neuBeginnenChecked = value;
                NeuBeginnenHeight = value ? new GridLength(1, GridUnitType.Star) : new GridLength(0, GridUnitType.Pixel);
                RaisePropertyChanged(nameof(NeuBeginnenHeight));
            }
        }

        //new GridLength(0, GridUnitType.Auto)
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

        public GridLength NeuBeginnenHeight
        {
            get;
            set;
        }

        #region Datenbankdatei öffnen

        public DelegateCommand<ICloseable> VortragsmanagerdateiLadenCommand { get; private set; }

        public DelegateCommand DatabaseFileDialogCommand { get; private set; }

        public void VortragsmanagerdateiLaden(ICloseable window)
        {
            IoSqlite.ReadContainer(ImportFile);
            Properties.Settings.Default.sqlite = ImportFile;
            IsFinished = true;
            Schließen(window);
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

            if (openDialog.ShowDialog() == DialogResult.OK)
            {
                ImportFile = openDialog.FileName;
            }

            openDialog.Dispose();
        }

        private string _importFile = string.Empty;

        public string ImportFile
        {
            get
            {
                return _importFile;
            }
            set
            {
                _importFile = value;
                RaisePropertyChanged();
            }
        }

        #endregion Datenbankdatei öffnen

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
                    Properties.Resources.Achtung,
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                return;
            }

            //einlesen der Excel-Datei
            if (!IoExcel.Vplanung.ImportKoordinatoren(ImportExcelFile))
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
                for (int i = anzahlVersammlungen - 1; i > 0; i--)
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

        #endregion Koordinatoren

        #region DeineVersammlung

        public DelegateCommand ExcelImportierenPlannungCommand { get; private set; }

        public static ObservableCollection<Conregation> VersammlungsListe => DataContainer.Versammlungen;

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

        #endregion DeineVersammlung

        #region Planungen importieren

        public ObservableCollection<string> ImportierteJahreliste { get; } = new ObservableCollection<string>();

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
            if (!Core.IoExcel.Vplanung.ImportEigenePlanungen(ImportExcelFile) ||
                !Core.IoExcel.Vplanung.ImportRednerPlanungen(ImportExcelFile))
                return;

            var jahr = Core.IoExcel.Vplanung.MeinPlan.Select(x => x.Datum.Year);

            bool hasMatch = ImportierteJahreliste.Any(x => jahr.Any(y => y.ToString(Core.DataContainer.German) == x));

            //prüfen ob bereits importiert wurde
            if (hasMatch)
            {
                if (ThemedMessageBox.Show(
                    $"Es existieren bereits Einträge für das gewählte Jahr. Ereignisse zusammenführen? (Bestehende Einträge werden überschrieben)?",
                    Properties.Resources.Achtung,
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

        #endregion Planungen importieren

        public bool IsFinished { get; set; }
    }

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

        public override int GetHashCode()
        {
            unchecked
            {
                // Choose large primes to avoid hashing collisions
                const int HashingBase = (int)2166136261;
                const int HashingMultiplier = 16777619;

                int hash = HashingBase;
                hash = (hash * HashingMultiplier) ^ (!object.ReferenceEquals(null, Nr) ? Nr.GetHashCode() : 0);
                return hash;
            }
        }
    }
}