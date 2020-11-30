using System.Windows;

namespace Vortragsmanager.Views
{
    /// <summary>
    /// Interaktionslogik für BuchungLöschenCommandDialog.xaml
    /// </summary>
    public partial class RednerEintragenDialog : Window, Datamodels.ICloseable
    {
        private RednerEintragenView _dataModel;

        public RednerEintragenDialog()
        {
            InitializeComponent();
            _dataModel = (RednerEintragenView)DataContext;
        }

        private void DropDownVersammlung_ConregationChanged(object sender, RoutedPropertyChangedEventArgs<Datamodels.Conregation> e)
        {
            if (e.NewValue != e.OldValue)
            {
                _dataModel.SelectedVersammlung = e.NewValue;
            }
        }
    }
}