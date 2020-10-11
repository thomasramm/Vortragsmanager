using System.Windows.Controls;

namespace Vortragsmanager.MeineRedner
{
    /// <summary>
    /// Interaktionslogik für ExternalQuestionEdit.xaml
    /// </summary>
    public partial class ExternalQuestionEdit : UserControl
    {
        private ExternalQuestionViewModel _model;

        public ExternalQuestionEdit()
        {
            InitializeComponent();
            _model = (ExternalQuestionViewModel)DataContext;
        }

        private void DropDownVersammlung_ConregationChanged(object sender, System.Windows.RoutedPropertyChangedEventArgs<Datamodels.Conregation> e)
        {
            _model.SelectedVersammlung = e.NewValue;
        }
    }
}