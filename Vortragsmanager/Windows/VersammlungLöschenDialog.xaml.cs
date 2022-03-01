using Vortragsmanager.Interface;

namespace Vortragsmanager.Windows
{
    /// <summary>
    /// Interaction logic for VersammlungLöschenDialog.xaml
    /// </summary>
    public partial class VersammlungLöschenDialog : ICloseable
    {
        private readonly VersammlungLöschenDialogView _dataModel;

        public VersammlungLöschenDialog()
        {
            InitializeComponent();
            _dataModel = (VersammlungLöschenDialogView)DataContext;
        }

        private void VersammlungenFilter_QuerySubmitted(object sender, DevExpress.Xpf.Editors.AutoSuggestEditQuerySubmittedEventArgs e)
        {
            _dataModel.SetVersammlungfilter(e.Text);
        }
    }
}