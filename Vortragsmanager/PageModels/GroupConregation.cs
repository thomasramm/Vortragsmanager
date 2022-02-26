using System.Collections.Generic;
using DevExpress.Mvvm;
using Vortragsmanager.Datamodels;
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

        public DelegateCommand AnfrageSendenCommand { get; private set; }

        public void AskForSpeaker()
        {
            Messenger.Default.Send(this, Messages.DisplayModuleAskForSpeaker);
        }

        public Conregation Versammlung { get; set; }

        public List<GroupSpeaker> Redner { get; } = new List<GroupSpeaker>();

        public string Name => Versammlung.Name;
    }
}