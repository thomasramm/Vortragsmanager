using System.Windows.Input;
using GroupBox = DevExpress.Xpf.LayoutControl.GroupBox;

namespace Vortragsmanager.UserControls
{
    /// <summary>
    /// Interaction logic for VersammlungControl.xaml
    /// </summary>
    public partial class VersammlungMaximizedControl
    {
        public VersammlungMaximizedControl()
        {
            InitializeComponent();
        }

        private void GroupBoxRedner_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var groupBox = (sender as GroupBox);
            var data = (groupBox?.DataContext as SpeakerViewModel);
            data?.NavigateToEditor();
            e.Handled = true;
        }
    }
}