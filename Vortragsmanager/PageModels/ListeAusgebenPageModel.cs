using System;
using System.IO;
using DevExpress.Mvvm;
using DevExpress.Xpf.Core;
using Vortragsmanager.Module;
using Vortragsmanager.DataModels;

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
            get => Helper.Helper.GlobalSettings.ListCreate_OpenFile;
            set
            {
                Helper.Helper.GlobalSettings.ListCreate_OpenFile = value;
                Helper.Helper.GlobalSettings.Save();
            }
        }

        public bool FotoExport
        {
            get => Helper.Helper.GlobalSettings.FotoExport;
            set
            {
                Helper.Helper.GlobalSettings.FotoExport = value;
                Helper.Helper.GlobalSettings.Save();
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
            var template = Helper.Helper.GlobalSettings.ExcelTemplateAushang;
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