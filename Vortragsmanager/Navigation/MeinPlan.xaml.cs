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
            ToggleButton_Unchecked(null, null);
        }

        private void ToggleButton_Checked(object sender, RoutedEventArgs e)
        {
            var a = sender;
            if (sender is ToggleButton button)
            {
                switch(button.Name)
                {
                    case nameof(AntwortEintragenButton):
                        //ActiveUserControl.Content = new Views.AntwortEintragenControl();
                        //ActiveUserControl.Content = new Vortragsmanager.MeinPlan.AntwortEintragenView();
                        var myControl = new Views.AntwortEintragenControl();
                        ActiveUserControl.Content = myControl;
                        ((Views.AntwortEintragenViewModel)myControl.DataContext).LoadData();
                        MenuHeaderTitel.Content = "Mein Plan > Antwort eintragen";
                        RednerSuchenButton.IsChecked = false;
                        return;
                    case nameof(RednerSuchenButton):
                        ActiveUserControl.Content = new Vortragsmanager.MeinPlan.SearchSpeaker();
                        MenuHeaderTitel.Content = "Mein Plan > Redner suchen";
                        AntwortEintragenButton.IsChecked = false;
                        return;
                }
            }
        }

        private void ToggleButton_Unchecked(object sender, RoutedEventArgs e)
        {
            if (AntwortEintragenButton.IsChecked == false && RednerSuchenButton.IsChecked == false)
            {
                ActiveUserControl.Content = new Vortragsmanager.MeinPlan.MeinPlanView();
                MenuHeaderTitel.Content = "Mein Plan";
            }
        }
    }
}
