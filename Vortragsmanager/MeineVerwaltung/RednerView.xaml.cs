using System.Windows.Controls;
using DevExpress.Xpf.WindowsUI.Navigation;

namespace Vortragsmanager.MeineVerwaltung
{
    /// <summary>
    /// Interaction logic for VorlagenView.xaml
    /// </summary>
    public partial class RednerView : UserControl, INavigationAware
    {
        private readonly RednerViewModel DataModel;

        public RednerView()
        {
            InitializeComponent();
            DataModel = (RednerViewModel)DataContext;
        }

        public void NavigatedTo(NavigationEventArgs e)
        {
            if (e is null)
                return;

            if (e.Parameter is null)
                return;

            var redner = (Datamodels.Speaker)e.Parameter;

            if (redner == null)
                return;

            RednerSelect.SelectedName = redner.Name;
            RednerSelect.SelectedItem = redner;
            //DataModel.Redner = redner;
        }

        /// <summary>
        /// Ich verlasse gleich die View.
        /// </summary>
        /// <param name="e">Parameter.</param>
        public void NavigatingFrom(NavigatingEventArgs e)
        {
        }

        /// <summary>
        /// Ich habe die view bereits verlassen.
        /// </summary>
        /// <param name="e">Parameter.</param>
        public void NavigatedFrom(NavigationEventArgs e)
        {
            if (e.Source.ToString() == "VersammlungSettingsView")
            {
                var vers = DataModel.Redner?.Versammlung;
                if (vers == null)
                    vers = DataModel.SelectedConregation;
                if (vers == null)
                    return;

                var versView = (e.Content as VersammlungSettingsView);
                versView.SelectConregation(vers);
                //e.Parameter = DataModel.Redner;
            }

        }

        //Vorschlagsliste aktualisieren


        private void DropDownVersammlung_ConregationChanged(object sender, System.Windows.RoutedPropertyChangedEventArgs<Datamodels.Conregation> e)
        {
            DataModel.SelectedConregation = e.NewValue;
            RednerSelect.SelectedVersammlung = e.NewValue;
        }

        private void DropDownRedner_OnSpeakerChanged(object sender, System.Windows.RoutedPropertyChangedEventArgs<Datamodels.Speaker> e)
        {
            DataModel.Redner = e.NewValue;
            calendar.Person = e.NewValue;
        }

        private void DropDownVortrag_OnSelectedItemChanged(object sender, System.Windows.RoutedPropertyChangedEventArgs<Datamodels.Talk> e)
        {
            DataModel.NeuerVortrag = e.NewValue;
        }
    }
}