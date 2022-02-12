using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace Vortragsmanager.Navigation
{
    /// <summary>
    /// Interaktionslogik für MeinPlan.xaml
    /// </summary>
    public partial class MeinPlan : UserControl
    {
        public MeinPlan()
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
                        ActiveUserControl.Content = new Vortragsmanager.MeinPlan.MeinPlanView();
                        RednerSuchenButton.IsChecked = false;
                        AntwortEintragenButton.IsChecked = false;
                        return;
                    case nameof(AntwortEintragenButton):
                        //ActiveUserControl.Content = new Views.AntwortEintragenControl();
                        //ActiveUserControl.Content = new Vortragsmanager.MeinPlan.AntwortEintragenView();
                        var myControl = new Views.AntwortEintragenControl();
                        ActiveUserControl.Content = myControl;
                        ((Views.AntwortEintragenViewModel)myControl.DataContext).LoadData();
                        RednerSuchenButton.IsChecked = false;
                        KalenderButton.IsChecked = false;
                        return;
                    case nameof(RednerSuchenButton):
                        ActiveUserControl.Content = new Vortragsmanager.MeinPlan.SearchSpeaker();
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
