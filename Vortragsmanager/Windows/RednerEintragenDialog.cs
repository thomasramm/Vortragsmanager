using System.Windows;
using Vortragsmanager.Interface;
using Vortragsmanager.UserControls;

namespace Vortragsmanager.Windows
{
    /// <summary>
    /// Interaktionslogik für BuchungLöschenCommandDialog.xaml
    /// </summary>
    public partial class RednerEintragenDialog : ICloseable
    {
        private readonly RednerEintragenView _dataModel;

        public RednerEintragenDialog()
        {
            InitializeComponent();
            _dataModel = (RednerEintragenView)DataContext;
        }

        public void InitializeSelectedConregation(Datamodels.Conregation versammlung)
        {
            FilterVersammlung.VersammlungenFilter.Text = versammlung.Name;
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