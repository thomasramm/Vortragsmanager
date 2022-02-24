using System.Windows.Controls;
using DevExpress.Mvvm;
using Vortragsmanager.Pages;

namespace Vortragsmanager.PageModels
{
    public class MeineRednerLandingPageModel : ViewModelBase
    {
        public MeineRednerLandingPageModel()
        {
            RednereinladungenButtonIsChecked = true;
        }

        private bool _neueAnfrageButtonIsChecked;
        public bool NeueAnfrageButtonIsChecked
        {
            get => _neueAnfrageButtonIsChecked;
            set
            {
                _neueAnfrageButtonIsChecked = value;
                if (value)
                {
                    _rednereinladungenButtonIsChecked = false;
                    //load Neue Anfrage
                    ActiveUserControl = new MeineRednerNeueAnfragePage();
                    RaisePropertiesChanged(nameof(MenuHeaderTitel));
                    RaisePropertiesChanged(nameof(ActiveUserControl));
                }
                else
                {
                    if (!_rednereinladungenButtonIsChecked)
                        RednereinladungenButtonIsChecked = true;
                }
                RaisePropertiesChanged();
            }
        }

        private bool _rednereinladungenButtonIsChecked;
        public bool RednereinladungenButtonIsChecked
        {
            get => _rednereinladungenButtonIsChecked;
            set
            {
                _rednereinladungenButtonIsChecked = value;
                if (value)
                {
                    _neueAnfrageButtonIsChecked = false;
                    ActiveUserControl = new MeineRednerKalenderPage();
                    RaisePropertiesChanged(nameof(MenuHeaderTitel));
                    RaisePropertiesChanged(nameof(ActiveUserControl));
                }
                else
                {
                    if (!_neueAnfrageButtonIsChecked)
                        RednereinladungenButtonIsChecked = true;
                }
                RaisePropertiesChanged();
            }
        }

        public UserControl ActiveUserControl { get; set; } = new MeineRednerKalenderPage();

        public string MenuHeaderTitel => _neueAnfrageButtonIsChecked ? "Meine Redner > Neue Anfrage" : "Meine Redner";
    }
}
