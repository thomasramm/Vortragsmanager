using System.Windows.Controls;
using DevExpress.Mvvm;
using Vortragsmanager.Datamodels;
using Vortragsmanager.Enums;
using Vortragsmanager.Interface;
using Vortragsmanager.Pages;

namespace Vortragsmanager.PageModels
{
    public class VerwaltungLandingPageModel : ViewModelBase, INavigation
    {
        private bool _buttonVersammlungIsChecked;
        private bool _buttonRednerIsChecked;
        private bool _buttonVorträgeIsChecked;
        private bool _buttonVorlagenIsChecked;
        private UserControl _activeControl;
        private string _header;

        //Navigation auf diese Seite mit Parameter
        protected override void OnParameterChanged(object parameter)
        {
            if (parameter is Speaker redner)
            {
                SpeakerParameter = redner;
                ButtonRednerIsChecked = true;
            }
            else
            {
                SpeakerParameter = null;
            }
                //switch (page)
                //{
                //    case "Redner":
                //        ButtonRednerIsChecked = true;
                //        break;
                //    case "Vorträge":
                //        ButtonVorträgeIsChecked = true;
                //        break;
                //    case "Versammlung":
                //        ButtonVersammlungIsChecked = true;
                //        break;
                //    case "Vorlagen":
                //        ButtonVorlagenIsChecked = true;
                //        break;
                //}

            //SelectedGroup = (string)parameter == "Design" ? 2 : 1;
            base.OnParameterChanged(parameter);
        }

        public INavigationService Service => ServiceContainer.GetService<INavigationService>();

        public void NavigateTo(NavigationPage page)
        {
            Service?.Navigate(page.ToString(), null, this);
        }

        public void NavigateTo(NavigationPage page, string parameter)
        {
            Service?.Navigate(page.ToString(), parameter, this);
        }

        public void NavigateTo(NavigationPage page, object parameter)
        {
            Service?.Navigate(page.ToString(), parameter, this);
        }

        public UserControl ActiveControl
        {
            get => _activeControl;
            set
            {
                _activeControl = value;
                RaisePropertyChanged();
            }
        }

        public Speaker SpeakerParameter { get; set; }

        public bool ButtonVorlagenIsChecked
        {
            get => _buttonVorlagenIsChecked;
            set
            {
                _buttonVorlagenIsChecked = value;
                if (value)
                {
                    Header = "Verwaltung > Vorlagen";
                    ActiveControl = new VerwaltungVorlagenPage();
                    ButtonRednerIsChecked = false;
                    ButtonVersammlungIsChecked = false;
                    ButtonVorträgeIsChecked = false;
                }
                else
                {
                    _buttonVorlagenIsChecked = NothingIsChecked();
                }
                RaisePropertyChanged();
            }
        }
        
        public bool ButtonVorträgeIsChecked
        {
            get => _buttonVorträgeIsChecked;
            set
            {
                _buttonVorträgeIsChecked = value;
                if (value)
                {
                    Header = "Verwaltung > Vortragsthemen";
                    ActiveControl = new VerwaltungVorträgePage();
                    ButtonRednerIsChecked = false;
                    ButtonVersammlungIsChecked = false;
                    ButtonVorlagenIsChecked = false;
                }
                else
                {
                    _buttonVorträgeIsChecked = NothingIsChecked();
                }
                RaisePropertyChanged();
            }
        }

        public bool ButtonRednerIsChecked
        {
            get => _buttonRednerIsChecked;
            set
            {
                _buttonRednerIsChecked = value;
                if (value)
                {
                    Header = "Verwaltung > Redner";
                    ActiveControl = new VerwaltungRednerPage(SpeakerParameter);
                    ButtonVorträgeIsChecked = false;
                    ButtonVersammlungIsChecked = false;
                    ButtonVorlagenIsChecked = false;
                }
                else
                {
                    _buttonRednerIsChecked = NothingIsChecked();
                }
                RaisePropertyChanged();
            }
        }

        public bool ButtonVersammlungIsChecked
        {
            get => _buttonVersammlungIsChecked;
            set
            {
                _buttonVersammlungIsChecked = value;
                if (value)
                {
                    Header = "Verwaltung > Versammlungen";
                    ActiveControl = new VerwaltungVersammlungPage(this);
                    ButtonVorträgeIsChecked = false;
                    ButtonRednerIsChecked = false;
                    ButtonVorlagenIsChecked = false;
                }
                else
                {
                    _buttonVersammlungIsChecked = NothingIsChecked();
                }
                RaisePropertyChanged();
            }
        }

        public string Header
        {
            get => _header;
            set
            {
                _header = value;
                RaisePropertyChanged();
            }
        }

        private bool NothingIsChecked()
        {
            return !_buttonVorträgeIsChecked && !_buttonVersammlungIsChecked && !_buttonRednerIsChecked && !_buttonVorlagenIsChecked;
        }
    }
}
