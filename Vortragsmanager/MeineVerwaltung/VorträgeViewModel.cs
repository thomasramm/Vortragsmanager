using DevExpress.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vortragsmanager.Properties;

namespace Vortragsmanager.MeineVerwaltung
{
    public class VorträgeViewModel : ViewModelBase
    {
        public ObservableCollection<Datamodels.Talk> Talks { get; private set; }

        public VorträgeViewModel()
        {
            Talks = new ObservableCollection<Datamodels.Talk>(Datamodels.TalkList.Get());
            ResetCommand = new DelegateCommand(Reset);
            CreateOverviewTalkCountCommand = new DelegateCommand(CreateOverviewTalkCount);
            AddThemaCommand = new DelegateCommand(AddThema);
            NewTalkNumber = Talks.Select(x => x.Nummer).Max()+1;
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
