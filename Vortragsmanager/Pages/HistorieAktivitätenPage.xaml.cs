using Vortragsmanager.PageModels;

namespace Vortragsmanager.Pages
{
    /// <summary>
    /// Interaktionslogik für ActivityLog.xaml
    /// </summary>
    public partial class HistorieAktivitätenPage
    {
        private readonly HistorieAktivitätenPageModel _dataModel;

        public HistorieAktivitätenPage()
        {
            InitializeComponent();
            _dataModel = (HistorieAktivitätenPageModel)DataContext;
        }

        private void VersammlungenFilter_QuerySubmitted(object sender, DevExpress.Xpf.Editors.AutoSuggestEditQuerySubmittedEventArgs e)
        {
            _dataModel.SetVersammlungfilter(e.Text);
        }
    }
}
