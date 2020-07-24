using System.Windows.Controls;

namespace Vortragsmanager.MeineVerwaltung
{
    /// <summary>
    /// Interaction logic for VorlagenView.xaml
    /// </summary>
    public partial class RednerView : UserControl
    {
        private readonly RednerViewModel DataModel;

        public RednerView()
        {
            InitializeComponent();
            DataModel = (RednerViewModel)DataContext;
        }

        //Vorschlagsliste aktualisieren
        private void Versammlung_AutoSuggestEdit_QuerySubmitted(object sender, DevExpress.Xpf.Editors.AutoSuggestEditQuerySubmittedEventArgs e)
        {
            DataModel.SetVersammlungfilter(e.Text);
        }

        private void Redner_AutoSuggestEdit_QuerySubmitted(object sender, DevExpress.Xpf.Editors.AutoSuggestEditQuerySubmittedEventArgs e)
        {
            DataModel.SetRednerfilter(e.Text);
        }
    }
}