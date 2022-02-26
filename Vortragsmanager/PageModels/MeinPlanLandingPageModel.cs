using System.Windows.Controls;
using DevExpress.Mvvm;
using Vortragsmanager.Enums;
using Vortragsmanager.Interface;
using Vortragsmanager.Pages;
using Vortragsmanager.UserControls;

namespace Vortragsmanager.PageModels
{
    public class MeinPlanLandingPageModel : ViewModelBase, INavigation
    {
        private bool _buttonAntwortEintragenIsChecked;
        private bool _buttonRednerSuchenIsChecked;
        private bool _buttonKalenderIsChecked;
        private UserControl _activeControl;

        //Navigation auf diese Seite mit Parameter
        protected override void OnParameterChanged(object parameter)
        {
            if (parameter != null)
            {
                var myParam = parameter.ToString().Split('#');
                var page = myParam.Length >= 1 ? myParam[0] : parameter.ToString();
                PageParameter = myParam.Length >= 2 ? myParam[1] : null;

                switch (page)
                {
                    case "RednerSuchen":
                        ButtonRednerSuchenIsChecked = true;
                        break;
                    case "Kalender":
                        ButtonKalenderIsChecked = true;
                        break;
                    case "AntwortEintragen":
                        ButtonAntwortEintragenIsChecked = true;
                        break;
                }

            }

            base.OnParameterChanged(parameter);
        }

        public INavigationService Service => ServiceContainer.GetService<INavigationService>();

        public void NavigateTo(NavigationPage page)
        {
            Service?.Navigate(page.ToString(), null, this);
        }

        public void NavigateTo(NavigationPage page, string parameter)
        {
            Service?.Navigate(page.ToString() , parameter, this);
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

        public string PageParameter { get; set; }

        public bool ButtonKalenderIsChecked
        {
            get => _buttonKalenderIsChecked;
            set
            {
                _buttonKalenderIsChecked = value;
                if (value)
                {
                    ActiveControl = new MeinPlanKalenderPage(this);
                    ButtonRednerSuchenIsChecked = false;
                    ButtonAntwortEintragenIsChecked = false;
                }
                else
                {
                    _buttonKalenderIsChecked = NothingIsChecked();
                }
                RaisePropertyChanged();
            }
        }

        public bool ButtonRednerSuchenIsChecked
        {
            get => _buttonRednerSuchenIsChecked;
            set
            {
                _buttonRednerSuchenIsChecked = value;
                if (value)
                {
                    ActiveControl = new MeinPlanRednerSuchenPage(PageParameter);
                    ButtonKalenderIsChecked = false;
                    ButtonAntwortEintragenIsChecked = false;
                }
                else
                {
                    _buttonRednerSuchenIsChecked = NothingIsChecked();
                }
                RaisePropertyChanged();
            }
        }

        public bool ButtonAntwortEintragenIsChecked
        {
            get => _buttonAntwortEintragenIsChecked;
            set
            {
                _buttonAntwortEintragenIsChecked = value;
                if (value)
                {
                    var myControl = new AntwortEintragenControl();
                    ActiveControl = myControl;
                    ((AntwortEintragenViewModel)myControl.DataContext).LoadData();

                    ButtonKalenderIsChecked = false;
                    ButtonRednerSuchenIsChecked = false;
                }
                else
                {
                    _buttonAntwortEintragenIsChecked = NothingIsChecked();
                }
                RaisePropertyChanged();
            }
        }

        private bool NothingIsChecked()
        {
            return !_buttonKalenderIsChecked && !_buttonAntwortEintragenIsChecked && !_buttonRednerSuchenIsChecked;
        }
    }
}
