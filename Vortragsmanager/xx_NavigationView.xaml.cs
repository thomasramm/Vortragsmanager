using DevExpress.Xpf.WindowsUI;
using System.Windows.Controls;
using Vortragsmanager.PageModels;

namespace Vortragsmanager.Navigation
{
    /// <summary>
    /// Interaktionslogik für NavigationView.xaml
    /// </summary>
    public partial class NavigationView : UserControl
    {
        private readonly HistorieAktivitätenPageModel DataModel;

        public NavigationView()
        {
            InitializeComponent();
            Frame = frame;
            DataModel = (HistorieAktivitätenPageModel)DataContext;
        }

        public static NavigationFrame Frame { get; set; }

        private void VersammlungenFilter_QuerySubmitted(object sender, DevExpress.Xpf.Editors.AutoSuggestEditQuerySubmittedEventArgs e)
        {
            DataModel.SetVersammlungfilter(e.Text);
        }
    }
}