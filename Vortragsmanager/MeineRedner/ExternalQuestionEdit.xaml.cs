using System.Windows.Controls;
using Vortragsmanager.Datamodels;

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

        private void Redner_SelectedIndexChanged(object sender, System.Windows.RoutedEventArgs e)
        {
            DevExpress.Xpf.Editors.ComboBoxEdit cb = (sender as DevExpress.Xpf.Editors.ComboBoxEdit);
            var redner = (cb.DataContext as ExternalQuestionViewModel).SelectedRedner;
            calendar.Person = redner;
        }
    }
}