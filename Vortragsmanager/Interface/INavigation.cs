using DevExpress.Mvvm;

namespace Vortragsmanager.Interface
{
    public interface INavigation
    {
        INavigationService Service { get; }

        void NavigateTo(Enums.NavigationPage page, string parameter);

        void NavigateTo(Enums.NavigationPage page, object parameter);
    }
}
