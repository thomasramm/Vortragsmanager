using System.Collections.ObjectModel;
using System.Linq;
using DevExpress.Mvvm;
using Vortragsmanager.Properties;

namespace Vortragsmanager.PageModels
{
    public class VerwaltungVorträgePageModel : ViewModelBase
    {
        public ObservableCollection<Datamodels.Talk> Talks { get; private set; }

        public VerwaltungVorträgePageModel()
        {
            Talks = new ObservableCollection<Datamodels.Talk>(Datamodels.TalkList.Get());
            ResetCommand = new DelegateCommand(Reset);
            CreateOverviewTalkCountCommand = new DelegateCommand(CreateOverviewTalkCount);
            AddThemaCommand = new DelegateCommand(AddThema);
            NewTalkNumber = Talks.Select(x => x.Nummer).Max() + 1;
        }

        public DelegateCommand ResetCommand { get; private set; }

        public DelegateCommand CreateOverviewTalkCountCommand { get; private set; }

        public DelegateCommand AddThemaCommand { get; private set; }

        public bool ListeÖffnen
        {
            get => Settings.Default.ListCreate_OpenFile;
            set
            {
                Settings.Default.ListCreate_OpenFile = value;
                Settings.Default.Save();
            }
        }

        public void Reset()
        {
            Datamodels.TalkList.Reset();
        }

        public void CreateOverviewTalkCount()
        {
            Core.IoExcel.Export.OverviewTalkCount(ListeÖffnen);
        }

        public void AddThema()
        {
            if (Talks.Any(x => x.Nummer == NewTalkNumber))
            {
                return;
            }
            var newTalk = new Datamodels.Talk(NewTalkNumber, "Neues Vortragsthema");
            Talks.Add(newTalk);
            Datamodels.TalkList.Add(newTalk);
        }

        private int _newTalkNumber;
        public int NewTalkNumber
        {
            get
            {
                return _newTalkNumber;
            }
            set
            {
                _newTalkNumber = value;
                RaisePropertyChanged();
            }
        }
    }
}
