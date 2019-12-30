using System.Windows.Controls;

namespace Vortragsmanager.Views
{
    /// <summary>
    /// Interaktionslogik für MeinPlanView.xaml
    /// </summary>
    public partial class MeinPlanView : UserControl
    {
        public MeinPlanView()
        {
            InitializeComponent();
        }

        private void MenuItem_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            //xmlns: dxwuin = "http://schemas.devexpress.com/winfx/2008/xaml/windowsui/navigation"
            //     xmlns: dxnav = "http://schemas.devexpress.com/winfx/2008/xaml/navigation"
            //DevExpress.Xpf.Navigation.NavigateTo. dxwuin: Navigation.NavigateTo = "MeinPlanView"
            //var p = ((System.Windows.Controls.Primitives.Popup)((ContextMenu)((MenuItem)sender).Parent).Parent).Parent;
            //Navigation.NavigationView.Frame.Navigate("SearchSpeaker");
        }
    }
}