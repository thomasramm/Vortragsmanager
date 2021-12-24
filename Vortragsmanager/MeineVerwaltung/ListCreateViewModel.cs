using DevExpress.Mvvm;
using OfficeOpenXml;
using OfficeOpenXml.Table;
using System;
using System.IO;
using System.Linq;
using Vortragsmanager.Core;
using Vortragsmanager.Datamodels;
using Vortragsmanager.Properties;

namespace Vortragsmanager.MeineVerwaltung
{
    public class ListCreateViewModel : ViewModelBase
    {
        public ListCreateViewModel()
        {
            Log.Info(nameof(ListCreateViewModel), "");
            CreateAushangCommand = new DelegateCommand(CreateAushang);
            CreateContactListCommand = new DelegateCommand(CreateContactList);
            CreateExchangeRednerListCommand = new DelegateCommand(CreateExchangeRednerList);
            CreateOverviewTalkCountCommand = new DelegateCommand(CreateOverviewTalkCount);
            CreateSpeakerOverviewCommand = new DelegateCommand(CreateSpeakerOverview);
        }

        public DelegateCommand CreateAushangCommand { get; private set; }

        public DelegateCommand CreateContactListCommand { get; private set; }

        public DelegateCommand CreateExchangeRednerListCommand { get; private set; }

        public DelegateCommand CreateOverviewTalkCountCommand { get; private set; }

        public DelegateCommand CreateSpeakerOverviewCommand { get; private set; }

        public bool ListeÖffnen
        {
            get => Settings.Default.ListCreate_OpenFile;
            set
            {
                Settings.Default.ListCreate_OpenFile = value;
                Settings.Default.Save();
            }
        }

        public void CreateAushang()
        {
            IoExcel.Export.Aushang(ListeÖffnen);
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

        public void CreateSpeakerOverview()
        {
            IoExcel.Export.SpeakerConregationCoordinatorOverview(ListeÖffnen);
        }

        
    }
}