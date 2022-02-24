using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Vortragsmanager.UserControls;

namespace Vortragsmanager.Pages
{
    /// <summary>
    /// Interaktionslogik für MeinPlan.xaml
    /// </summary>
    public partial class MeinPlanLandingPage
    {
        public MeinPlanLandingPage()
        {
            InitializeComponent();
            KalenderButton.IsChecked = true;
        }

        private void ToggleButton_Checked(object sender, RoutedEventArgs e)
        {
            var a = sender;
            if (sender is ToggleButton button)
            {
                switch(button.Name)
                {
                    case nameof(KalenderButton):
                        ActiveUserControl.Content = new MeinPlanKalenderPage();
                        RednerSuchenButton.IsChecked = false;
                        AntwortEintragenButton.IsChecked = false;
                        return;
                    case nameof(AntwortEintragenButton):
                        //ActiveUserControl.Content = new Views.AntwortEintragenControl();
                        //ActiveUserControl.Content = new Vortragsmanager.MeinPlan.AntwortEintragenView();
                        var myControl = new AntwortEintragenControl();
                        ActiveUserControl.Content = myControl;
                        ((AntwortEintragenViewModel)myControl.DataContext).LoadData();
                        RednerSuchenButton.IsChecked = false;
                        KalenderButton.IsChecked = false;
                        return;
                    case nameof(RednerSuchenButton):
                        ActiveUserControl.Content = new MeinPlanRednerSuchenPage();
                        AntwortEintragenButton.IsChecked = false;
                        KalenderButton.IsChecked = false;
                        return;
                }
            }
        }

        private void ToggleButton_Unchecked(object sender, RoutedEventArgs e)
        {
            var deSelected = (ToggleButton)sender;
            if (KalenderButton.IsChecked == false
                && AntwortEintragenButton.IsChecked == false
                && RednerSuchenButton.IsChecked == false)
                deSelected.IsChecked = true;
        }
    }
}
