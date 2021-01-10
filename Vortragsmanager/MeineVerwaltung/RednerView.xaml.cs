using System.Windows.Controls;

namespace Vortragsmanager.MeineVerwaltung
{
    /// <summary>
    /// Interaction logic for VorlagenView.xaml
    /// </summary>
    public partial class RednerView : UserControl
    {
        private readonly RednerViewModel DataModel;

        public RednerView()
        {
            InitializeComponent();
            DataModel = (RednerViewModel)DataContext;
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