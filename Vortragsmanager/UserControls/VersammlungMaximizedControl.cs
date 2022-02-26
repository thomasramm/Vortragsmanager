using System.Windows.Controls;
using System.Windows.Input;
using GroupBox = DevExpress.Xpf.LayoutControl.GroupBox;

namespace Vortragsmanager.Views
{
    /// <summary>
    /// Interaction logic for VersammlungControl.xaml
    /// </summary>
    public partial class VersammlungMaximizedControl : UserControl
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