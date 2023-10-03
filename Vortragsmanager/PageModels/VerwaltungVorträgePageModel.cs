using System.Collections.ObjectModel;
using System.Linq;
using DevExpress.Mvvm;
using Vortragsmanager.Module;

namespace Vortragsmanager.PageModels
{
    public class VerwaltungVorträgePageModel : ViewModelBase
    {
        public ObservableCollection<DataModels.Talk> Talks { get; }

        public VerwaltungVorträgePageModel()
        {
            Talks = new ObservableCollection<DataModels.Talk>(DataModels.TalkList.Get());
            ResetCommand = new DelegateCommand(Reset);
            CreateOverviewTalkCountCommand = new DelegateCommand(CreateOverviewTalkCount);
            AddThemaCommand = new DelegateCommand(AddThema);
            NewTalkNumber = Talks.Select(x => x.Nummer).Max() + 1;
        }

        public DelegateCommand ResetCommand { get; }

        public DelegateCommand CreateOverviewTalkCountCommand { get; }

        public DelegateCommand AddThemaCommand { get; }

        public bool ListeÖffnen
        {
            get => Helper.Helper.GlobalSettings.ListCreate_OpenFile;
            set
            {
                Helper.Helper.GlobalSettings.ListCreate_OpenFile = value;
                Helper.Helper.GlobalSettings.Save();
            }
        }

        public void Reset()
        {
            DataModels.TalkList.Reset();
        }

        public void CreateOverviewTalkCount()
        {
            IoExcel.Export.OverviewTalkCount(ListeÖffnen);
        }

        public void AddThema()
        {
            if (Talks.Any(x => x.Nummer == NewTalkNumber))
            {
                return;
            }
            var newTalk = new DataModels.Talk(NewTalkNumber, "Neues Vortragsthema");
            Talks.Add(newTalk);
            DataModels.TalkList.Add(newTalk);
        }

        private int _newTalkNumber;
        public int NewTalkNumber
        {
            get => _newTalkNumber;
            set
            {
                _newTalkNumber = value;
                RaisePropertyChanged();
            }
        }
    }
}
