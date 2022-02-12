using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Navigation;
using DevExpress.Mvvm;

namespace Vortragsmanager.MeineVerwaltung
{
    /// <summary>
    /// Interaction logic for EinstellungenView.xaml
    /// </summary>
    public partial class EinstellungenView : UserControl
    {
        public EinstellungenView()
        {
            InitializeComponent();
        }

        private void ExcelFile_ValidateExists(object sender, DevExpress.Xpf.Editors.ValidationEventArgs e)
        {
            e.IsValid = File.Exists(e.Value.ToString());
        }

        private void ToggleButton_Checked(object sender, RoutedEventArgs e)
        {
            if (sender is ToggleButton button)
            {
                if (button.Name != nameof(ButtonDatei))
                {
                    GruppeDatei.Visibility = Visibility.Collapsed;
                    ButtonDatei.IsChecked = false;
                }

                if (button.Name != nameof(ButtonAussehen))
                {
                    GruppeAussehen.Visibility = Visibility.Collapsed;
                    ButtonAussehen.IsChecked = false;
                }

                if (button.Name != nameof(ButtonVerhalten))
                {
                    ButtonVerhalten.IsChecked = false;
                    GruppeVerhalten.Visibility = Visibility.Collapsed;
                }

                if (button.Name != nameof(ButtonAktion))
                {
                    ButtonAktion.IsChecked = false;
                    GruppeAktionen.Visibility = Visibility.Collapsed;
                }

                switch (button.Name)
                {
                    case nameof(ButtonDatei):
                        GruppeDatei.Visibility = Visibility.Visible;
                        return;
                    case nameof(ButtonAussehen):
                        GruppeAussehen.Visibility = Visibility.Visible;
                        return;
                    case nameof(ButtonVerhalten):
                        GruppeVerhalten.Visibility = Visibility.Visible;
                        return;
                    case nameof(ButtonAktion):
                        GruppeAktionen.Visibility = Visibility.Visible;
                        return;
                }
            }
        }

        private void ToggleButton_Unchecked(object sender, RoutedEventArgs e)
        {
            //Ein Button ist immer aktiv
            var deSelected = (ToggleButton)sender;
            if (ButtonDatei.IsChecked != false 
                || ButtonAussehen.IsChecked != false 
                || ButtonVerhalten.IsChecked != false 
                || ButtonAktion.IsChecked != false) 
                return;

            GruppeAussehen.Visibility = Visibility.Visible;
            GruppeDatei.Visibility = Visibility.Visible;
            GruppeVerhalten.Visibility = Visibility.Visible;
            GruppeAktionen.Visibility = Visibility.Visible;
        }

        private void Selector_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //NavigationService.Refresh();
        }
    }
}