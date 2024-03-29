﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Forms;
using DevExpress.Mvvm;
using DevExpress.Xpf.Controls.Native;
using DevExpress.Xpf.Core;
using Vortragsmanager.DataModels;
using Vortragsmanager.Enums;
using Vortragsmanager.Helper;
using Vortragsmanager.Interface;
using Vortragsmanager.Module;
using Vortragsmanager.Windows;
using Application = System.Windows.Application;

namespace Vortragsmanager.PageModels
{
    public class EinstellungenPageModel : ViewModelBase, INavigation
    {
        private string _datenbank;

        public EinstellungenPageModel()
        {
            ExcelFileDialogCommand = new DelegateCommand<string>(ExcelFileDialog);
            SearchDatabaseCommand = new DelegateCommand<string>(SearchDatabase);
            UpdateSpeakerFromExcelCommand = new DelegateCommand(UpdateSpeakerFromExcel);
            EmergencyMailCommand = new DelegateCommand<int?>(EmergencyMail);
            CalculateRouteCommand = new DelegateCommand<bool>(CalculateRoute);
            ShowChangelogCommand = new DelegateCommand(ShowChangelog);
            OpenExcelTemplateAushangCommand = new DelegateCommand(OpenExcelTemplateAushang);
            WizardCommand = new DelegateCommand(Wizard);
            Datenbank = Helper.Helper.GlobalSettings.sqlite;
            SelectedTheme = ThemeIsDark ? "Dunkel" : "Hell";
        }

        public INavigationService Service => ServiceContainer.GetService<INavigationService>();

        public void NavigateTo(NavigationPage page, string parameter)
        {
            Service?.Navigate(page.ToString(), parameter, this);
        }

        public void NavigateTo(NavigationPage page, object parameter)
        {
            Service?.Navigate(page.ToString(), parameter, this);
        }

        private void Wizard()
        {
            Initialize.LoadWizard();
            Helper.Helper.GlobalSettings.RefreshTitle();
            NavigateTo(NavigationPage.DashboardPage, null);
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

        public DelegateCommand<string> ExcelFileDialogCommand { get; }

        public DelegateCommand WizardCommand { get; }

        public DelegateCommand UpdateSpeakerFromExcelCommand { get; }

        public DelegateCommand<int?> EmergencyMailCommand { get; }

        public DelegateCommand<bool> CalculateRouteCommand { get; }

        public DelegateCommand ShowChangelogCommand { get; }

        public DelegateCommand OpenExcelTemplateAushangCommand { get; }

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
            get => Helper.Helper.GlobalSettings.ListAushangAnzahlWochen;
            set
            {
                if (value > 24)
                    value = 24;
                if (value < 1)
                    value = 1;
                Helper.Helper.GlobalSettings.ListAushangAnzahlWochen = value;
                RaisePropertyChanged();
            }
        }

        public int RednerSuchenAnzahlMonate
        {
            get => Helper.Helper.GlobalSettings.RednerSuchenAnzahlMonate;
            set
            {
                if (value > 36)
                    value = 36;
                if (value < 1)
                    value = 1;
                Helper.Helper.GlobalSettings.RednerSuchenAnzahlMonate = value;
                RaisePropertyChanged();
            }
        }

        public int RednerSuchenAbstandAnzahlMonate
        {
            get => Helper.Helper.GlobalSettings.RednerSuchenAbstandAnzahlMonate;
            set
            {
                if (value > 99)
                    value = 99;
                if (value < 1)
                    value = 1;
                Helper.Helper.GlobalSettings.RednerSuchenAbstandAnzahlMonate = value;
                RaisePropertyChanged();
            }
        }

        public void ShowChangelog()
        {
            Update.ShowChanges(true);
        }

        public void OpenExcelTemplateAushang()
        {
            var template = $"{Helper.Helper.TemplateFolder}AushangExcelTemplate.xlsx";
            var saveFileDialog1 = new SaveFileDialog
            {
                Filter = Properties.Resources.DateifilterExcel,
                FilterIndex = 1,
                RestoreDirectory = false,
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                CheckFileExists = false,
                FileName = "AushangExcelTemplate.xlsx",
            };
            string resultFile = null;

            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                var fi = new FileInfo(saveFileDialog1.FileName);
                resultFile = fi.FullName;
                var i = 0;
                try
                {
                    if (File.Exists(resultFile)) 
                        File.Delete(resultFile);
                }
                catch (IOException)
                {
                    while (File.Exists(resultFile))
                    {
                        i++;
                        resultFile = $"{fi.DirectoryName}\\{fi.Name.Substring(0, fi.Name.Length - fi.Extension.Length)} ({i}){fi.Extension}";
                    }
                }
                finally
                {
                    File.Copy(template, resultFile);
                    System.Diagnostics.Process.Start(resultFile);
                }
            }
        }

        public void ExcelFileDialog(string dateiTyp)
        {
            string datei = string.Empty;
            switch(dateiTyp)
            {
                case "ImportRednerliste":
                    datei = ImportExcelFile;
                    break;
                case "Aushang":
                    datei = ExcelTemplateAushang; 
                    break;
            }
            FileInfo fi;
            if (!string.IsNullOrEmpty(ExcelTemplateAushang))
                fi = new FileInfo(datei);
            else 
                fi = new FileInfo(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "AushangTemplate.xlsx"));
            var dir = fi.Directory.FullName;

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
                
                switch (dateiTyp)
                {
                    case "ImportRednerliste":
                        ImportExcelFile = openDialog.FileName;
                        break;
                    case "Aushang":
                        ExcelTemplateAushang = openDialog.FileName;
                        break;
                }
            }

            openDialog.Dispose();
            Helper.Helper.GlobalSettings.Save();
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
                    Helper.Helper.GlobalSettings.sqlite = openDialog.FileName;
                    Helper.Helper.GlobalSettings.Save();
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
                    Helper.Helper.GlobalSettings.sqlite = saveDialog.FileName;
                    Helper.Helper.GlobalSettings.Save();
                }

                saveDialog.Dispose();
            }

            Helper.Helper.GlobalSettings.RefreshTitle();
        }

        public bool SaveBackup
        {
            get => Helper.Helper.GlobalSettings.SaveBackups;
            set
            {
                Helper.Helper.GlobalSettings.SaveBackups = value;
                Helper.Helper.GlobalSettings.Save();
                RaisePropertyChanged();
            }
        }

        public bool ShowChangelogState
        {
            get => !Helper.Helper.GlobalSettings.HideChangelog;
            set
            {
                Helper.Helper.GlobalSettings.HideChangelog = !value;
                Helper.Helper.GlobalSettings.Save();
                RaisePropertyChanged();
            }
        }

        public bool ShowActivityButtons
        {
            get => Helper.Helper.GlobalSettings.ShowActivityButtons;
            set
            {
                Helper.Helper.GlobalSettings.ShowActivityButtons = value;
                Helper.Helper.GlobalSettings.Save();
                RaisePropertyChanged();
            }
        }

        public int SelectedLogLevel
        {
            get => Helper.Helper.GlobalSettings.LogLevel;
            set
            {
                Log.Start((LogLevel)value);
                RaisePropertyChanged();
            }
        }

        public string LogFolder
        {
            get => Helper.Helper.GlobalSettings.LogFolder;
            set
            {
                var di = new DirectoryInfo(value);
                if (!di.Exists)
                {
                    return;
                }
                Helper.Helper.GlobalSettings.LogFolder = value;
                Helper.Helper.GlobalSettings.Save();
                RaisePropertyChanged();
            }
        }

        public string ExcelTemplateAushang
        {
            get => Helper.Helper.GlobalSettings.ExcelTemplateAushang;
            set
            {
                var di = new FileInfo(value);
                if (!di.Exists)
                {
                    value = string.Empty;
                }
                Helper.Helper.GlobalSettings.ExcelTemplateAushang = value;
                Helper.Helper.GlobalSettings.Save();
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
            get => Helper.Helper.GlobalSettings.ThemeIsDark;
            set
            {
                Helper.Helper.GlobalSettings.ThemeIsDark = value;
                Helper.Helper.GlobalSettings.Save();
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