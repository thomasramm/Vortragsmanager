using System.Windows;
using Vortragsmanager.Interface;

namespace Vortragsmanager.Views
{
    /// <summary>
    /// Interaction logic for VersammlungLöschenDialog.xaml
    /// </summary>
    public partial class VersammlungLöschenDialog : Window, ICloseable
    {
        private readonly VersammlungLöschenDialogView DataModel;

        public VersammlungLöschenDialog()
        {
            InitializeComponent();
            DataModel = (VersammlungLöschenDialogView)DataContext;
        }

        private void VersammlungenFilter_QuerySubmitted(object sender, DevExpress.Xpf.Editors.AutoSuggestEditQuerySubmittedEventArgs e)
        {
            DataModel.SetVersammlungfilter(e.Text);
        }
    }
}