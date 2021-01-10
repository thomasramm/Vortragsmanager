using System.Windows;

namespace Vortragsmanager.Views
{
    /// <summary>
    /// Interaktionslogik für BuchungLöschenCommandDialog.xaml
    /// </summary>
    public partial class EreignisEintragenCommandDialog : Window, Datamodels.ICloseable
    {
        private EreignisEintragenCommandDialogView DataModel;

        public EreignisEintragenCommandDialog()
        {
            InitializeComponent();
            DataModel = (EreignisEintragenCommandDialogView)DataContext;
        }
    }
}