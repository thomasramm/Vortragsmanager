using System.Windows.Controls;

namespace Vortragsmanager.ActivityLog
{
    /// <summary>
    /// Interaktionslogik für ActivityLog.xaml
    /// </summary>
    public partial class ActivityLog : UserControl
    {
        private readonly Activities DataModel;

        public ActivityLog()
        {
            InitializeComponent();
            DataModel = (Activities)DataContext;
        }

        private void VersammlungenFilter_QuerySubmitted(object sender, DevExpress.Xpf.Editors.AutoSuggestEditQuerySubmittedEventArgs e)
        {
            DataModel.SetVersammlungfilter(e.Text);
        }
    }
}
