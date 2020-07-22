using System.Linq;
using System.Windows.Controls;
using Vortragsmanager.Models;

namespace Vortragsmanager.Views
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