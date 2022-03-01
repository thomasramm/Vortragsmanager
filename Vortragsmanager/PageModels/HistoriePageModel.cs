using System.Windows.Controls;
using DevExpress.Mvvm;
using Vortragsmanager.Enums;
using Vortragsmanager.Interface;
using Vortragsmanager.Pages;

namespace Vortragsmanager.PageModels
{
    public class HistoriePageModel : ViewModelBase, INavigation
    {
        private bool _buttonLogIsChecked;
        private bool _buttonSicherungIsChecked;
        private UserControl _activeControl;
        private string _header;

        public HistoriePageModel()
        {
            ButtonSicherungIsChecked = true;
        }

        public INavigationService Service => ServiceContainer.GetService<INavigationService>();

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
        
        public bool ButtonSicherungIsChecked
        {
            get => _buttonSicherungIsChecked;
            set
            {
                _buttonSicherungIsChecked = value;
                if (value)
                {
                    Header = "Historie > Sicherung";
                    ActiveControl = new HistorieSicherungPage();
                    ButtonLogIsChecked = false;
                }
                else
                {
                    _buttonSicherungIsChecked = NothingIsChecked();
                }
                RaisePropertyChanged();
            }
        }

        public bool ButtonLogIsChecked
        {
            get => _buttonLogIsChecked;
            set
            {
                _buttonLogIsChecked = value;
                if (value)
                {
                    Header = "Historie > Aktivitäten";
                    ActiveControl = new HistorieAktivitätenPage();
                    ButtonSicherungIsChecked = false;
                }
                else
                {
                    _buttonLogIsChecked = NothingIsChecked();
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
        return !_buttonLogIsChecked && !_buttonSicherungIsChecked;
    }
}
}
