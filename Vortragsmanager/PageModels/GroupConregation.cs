using System.Collections.Generic;
using System.Linq;
using DevExpress.Mvvm;
using Vortragsmanager.DataModels;
using Vortragsmanager.Enums;

namespace Vortragsmanager.PageModels
{
    /// <summary>
    /// Liste der Versammlungen
    /// </summary>
    public class GroupConregation : ViewModelBase
    {
        public GroupConregation()
        {
            AnfrageSendenCommand = new DelegateCommand(AskForSpeaker);
        }

        public DelegateCommand AnfrageSendenCommand { get; }

        public void AskForSpeaker()
        {
            Messenger.Default.Send(this, Messages.DisplayModuleAskForSpeaker);
        }

        public Conregation Versammlung { get; set; }

        public List<GroupSpeaker> Redner { get; } = new List<GroupSpeaker>();

        public string Name => Versammlung.Name;

        public bool HatGewählteRedner => Redner.Where(x => x.Gewählt).Any();

        public void RefreshGewählteRedner()
        {
            RaisePropertyChanged();
        }
    }
}