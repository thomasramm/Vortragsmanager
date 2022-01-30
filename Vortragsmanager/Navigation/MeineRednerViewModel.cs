using DevExpress.Mvvm;
using System.Windows.Controls;
using Vortragsmanager.MeineRedner;

namespace Vortragsmanager.Navigation
{
    public class MeineRednerViewModel : ViewModelBase
    {
        private bool _neueAnfrageButtonIsChecked;
        public bool NeueAnfrageButtonIsChecked
        {
            get => _neueAnfrageButtonIsChecked;
            set
            {
                _neueAnfrageButtonIsChecked = value;
                if (value)
                {
                    //load Neue Anfrage
                    ActiveUserControl = new ExternalQuestionEdit();
                }
                else
                {
                    ActiveUserControl = new MeineRednerPlan();
                }

                RaisePropertiesChanged();
                RaisePropertiesChanged(nameof(MenuHeaderTitel));
                RaisePropertiesChanged(nameof(ActiveUserControl));
            }
        }

        public UserControl ActiveUserControl { get; set; } = new MeineRednerPlan();

        public string MenuHeaderTitel => _neueAnfrageButtonIsChecked ? "Meine Redner > Neue Anfrage" : "Meine Redner";
    }
}
