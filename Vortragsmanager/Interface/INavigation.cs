using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevExpress.Mvvm;
using DevExpress.Xpf.WindowsUI;

namespace Vortragsmanager.Interface
{
    public interface INavigation
    {
        INavigationService Service { get; }

        void NavigateTo(Enums.NavigationPage page);

        void NavigateTo(Enums.NavigationPage page, string parameter);

        void NavigateTo(Enums.NavigationPage page, object parameter);
    }
}
