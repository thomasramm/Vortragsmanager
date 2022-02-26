using System.Windows;
using Vortragsmanager.Interface;

namespace Vortragsmanager.Views
{
    /// <summary>
    /// Interaktionslogik für BuchungLöschenCommandDialog.xaml
    /// </summary>
    public partial class EreignisEintragenCommandDialog : Window, ICloseable
    {
        private EreignisEintragenCommandDialogView DataModel;

        public EreignisEintragenCommandDialog()
        {
            InitializeComponent();
            DataModel = (EreignisEintragenCommandDialogView)DataContext;
        }
    }
}