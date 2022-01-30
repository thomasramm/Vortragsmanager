using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Vortragsmanager.Navigation
{
    /// <summary>
    /// Interaktionslogik für Verwaltung.xaml
    /// </summary>
    public partial class Verwaltung : UserControl
    {
        public Verwaltung()
        {
            InitializeComponent();
            Redner.IsChecked = true;
        }
        private void ToggleButton_Checked(object sender, RoutedEventArgs e)
        {
            var a = sender;
            if (sender is ToggleButton button)
            {
                DeselectAllButtonsExpectThis(button);

                switch (button.Name)
                {
                    case nameof(Versammlung):
                        ActiveUserControl.Content = new MeineVerwaltung.VersammlungSettingsView();
                        MenuHeaderTitel.Content = "Verwaltung > Versammlungen";
                        return;
                    case nameof(Redner):
                        ActiveUserControl.Content = new MeineVerwaltung.RednerView();
                        MenuHeaderTitel.Content = "Verwaltung > Redner";
                        return;
                    case nameof(Vorträge):
                        ActiveUserControl.Content = new MeineVerwaltung.VorträgeView();
                        MenuHeaderTitel.Content = "Verwaltung > Vortragsthemen";
                        return;
                    case nameof(Vorlagen):
                        ActiveUserControl.Content = new MeineVerwaltung.VorlagenView();
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
