using System;
using System.IO;
using DevExpress.Mvvm;
using DevExpress.Xpf.Core;
using Vortragsmanager.Module;
using Vortragsmanager.Properties;

namespace Vortragsmanager.PageModels
{
    public class ListeAusgebenPageModel : ViewModelBase
    {
        public ListeAusgebenPageModel()
        {
            Log.Info(nameof(ListeAusgebenPageModel), "");
            CreateAushangCommand = new DelegateCommand(CreateAushang);
            CreateContactListCommand = new DelegateCommand(CreateContactList);
            CreateExchangeRednerListCommand = new DelegateCommand(CreateExchangeRednerList);
            CreateOverviewTalkCountCommand = new DelegateCommand(CreateOverviewTalkCount);
            CreateDataExportCommand = new DelegateCommand(CreateDataExport);
        }

        public DelegateCommand CreateAushangCommand { get; }

        public DelegateCommand CreateContactListCommand { get; }

        public DelegateCommand CreateExchangeRednerListCommand { get; }

        public DelegateCommand CreateOverviewTalkCountCommand { get; }

        public DelegateCommand CreateDataExportCommand { get; }

        public bool ListeÖffnen
        {
            get => Settings.Default.ListCreate_OpenFile;
            set
            {
                Settings.Default.ListCreate_OpenFile = value;
                Settings.Default.Save();
            }
        }

        public bool FotoExport
        {
            get => Settings.Default.FotoExport;
            set
            {
                Settings.Default.FotoExport = value;
                Settings.Default.Save();
            }
        }

        public int ListAushangAnzahlWochen
        {
            get => Settings.Default.ListAushangAnzahlWochen;
            set
            {
                if (value > 24)
                    value = 24;
                if (value < 1)
                    value = 1;
                Settings.Default.ListAushangAnzahlWochen = value;
                RaisePropertyChanged();
            }
        }

        private DateTime _listAushangStartDate = DateTime.Today;
        public DateTime ListAushangStartDate
        {
            get => _listAushangStartDate;
            set 
            {
                _listAushangStartDate = value;
                RaisePropertyChanged();
            }
            
        }

        public void CreateAushang()
        {
            var template = Settings.Default.ExcelTemplateAushang;
            if (string.IsNullOrEmpty(template))
                IoExcel.Export.Aushang(ListeÖffnen, ListAushangStartDate);
            else
            {
                if (File.Exists(template))
                    IoExcel.Export.AushangTemplate(ListeÖffnen, template, ListAushangStartDate);
                else
                    ThemedMessageBox.Show("Vorlage nicht gefunden", $"Die Vorlagendatei\n{template}\n wurde nicht gefunden. Bitte prüfe den Pfad in den Einstellungen.", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);

            }
        }

        public void CreateContactList()
        {
            IoExcel.Export.ContactList(ListeÖffnen);
        }

        public void CreateExchangeRednerList()
        {
            IoExcel.Export.ExchangeRednerList(ListeÖffnen);
        }

        public void CreateOverviewTalkCount()
        {
            IoExcel.Export.OverviewTalkCount(ListeÖffnen);
        }

        public void CreateDataExport()
        {
            var excelFile = IoExcel.Export.SpeakerConregationCoordinatorOverview(ListeÖffnen);
            if (excelFile == null) 
                return;

            var fi = new FileInfo(excelFile);
            Photo.SaveToFile(fi.DirectoryName);
        }
    }
}