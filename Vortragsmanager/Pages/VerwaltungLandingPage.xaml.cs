using System.Windows;
using System.Windows.Controls.Primitives;

namespace Vortragsmanager.Pages
{
    /// <summary>
    /// Interaktionslogik für Verwaltung.xaml
    /// </summary>
    public partial class VerwaltungLandingPage
    {
        public VerwaltungLandingPage()
        {
            InitializeComponent();
            Redner.IsChecked = true;
        }
        private void ToggleButton_Checked(object sender, RoutedEventArgs e)
        {
            if (sender is ToggleButton button)
            {
                DeselectAllButtonsExpectThis(button);

                switch (button.Name)
                {
                    case nameof(Versammlung):
                        ActiveUserControl.Content = new VerwaltungVersammlungPage();
                        MenuHeaderTitel.Content = "Verwaltung > Versammlungen";
                        return;
                    case nameof(Redner):
                        ActiveUserControl.Content = new VerwaltungRednerPage();
                        MenuHeaderTitel.Content = "Verwaltung > Redner";
                        return;
                    case nameof(Vorträge):
                        ActiveUserControl.Content = new VerwaltungVorträgePage();
                        MenuHeaderTitel.Content = "Verwaltung > Vortragsthemen";
                        return;
                    case nameof(Vorlagen):
                        ActiveUserControl.Content = new VerwaltungVorlagenPage();
                        MenuHeaderTitel.Content = "Verwaltung > Vorlagen";
                        return;
                }
            }
        }

        private void DeselectAllButtonsExpectThis(ToggleButton button)
        {
            if (Versammlung != button)
                Versammlung.IsChecked = false;

            if (Redner != button)
                Redner.IsChecked = false;

            if (Vorträge != button)
                Vorträge.IsChecked = false;

            if (Vorlagen != button)
                Vorlagen.IsChecked = false;
        }

        private void ToggleButton_Unchecked(object sender, RoutedEventArgs e)
        {
            if (sender is ToggleButton button)
            {
                if (Versammlung.IsChecked == true)
                    return;
                if (Redner.IsChecked == true)
                    return;
                if (Vorträge.IsChecked == true)
                    return;
                if (Vorlagen.IsChecked == true)
                    return;

                button.IsChecked = true;

            }
        }
    }
}
