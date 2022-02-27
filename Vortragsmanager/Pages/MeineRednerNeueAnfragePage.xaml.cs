using System.Windows.Controls;
using Vortragsmanager.PageModels;

namespace Vortragsmanager.Pages
{
    /// <summary>
    /// Interaktionslogik für ExternalQuestionEdit.xaml
    /// </summary>
    public partial class MeineRednerNeueAnfragePage : UserControl
    {
        private MeineRednerNeueAnfragePageModel _model;

        public MeineRednerNeueAnfragePage()
        {
            InitializeComponent();
            _model = (MeineRednerNeueAnfragePageModel)DataContext;
        }

        private void DropDownVersammlung_ConregationChanged(object sender, System.Windows.RoutedPropertyChangedEventArgs<Datamodels.Conregation> e)
        {
            _model.SelectedVersammlung = e.NewValue;
        }

        private void Redner_SelectedIndexChanged(object sender, System.Windows.RoutedEventArgs e)
        {
            DevExpress.Xpf.Editors.ComboBoxEdit cb = (sender as DevExpress.Xpf.Editors.ComboBoxEdit);
            var redner = (cb.DataContext as MeineRednerNeueAnfragePageModel).SelectedRedner;
            Calendar.Person = redner;
        }
    }
}