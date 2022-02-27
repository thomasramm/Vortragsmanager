using DevExpress.Mvvm;
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
            CreateSpeakerOverviewCommand = new DelegateCommand(CreateSpeakerOverview);
        }

        public DelegateCommand CreateAushangCommand { get; }

        public DelegateCommand CreateContactListCommand { get; }

        public DelegateCommand CreateExchangeRednerListCommand { get; }

        public DelegateCommand CreateOverviewTalkCountCommand { get; }

        public DelegateCommand CreateSpeakerOverviewCommand { get; }

        public bool ListeÖffnen
        {
            get => Settings.Default.ListCreate_OpenFile;
            set
            {
                Settings.Default.ListCreate_OpenFile = value;
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