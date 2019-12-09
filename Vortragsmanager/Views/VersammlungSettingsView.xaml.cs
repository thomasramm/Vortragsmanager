using DevExpress.Xpf.LayoutControl;
using System.Windows.Controls;
using System.Windows.Input;
using GroupBox = DevExpress.Xpf.LayoutControl.GroupBox;

namespace Vortragsmanager.Views
{
    /// <summary>
    /// Interaction logic for VorlagenView.xaml
    /// </summary>
    public partial class VersammlungSettingsView : UserControl
    {
        //ToDo: Anzeigen und Einstellen meiner Versammlungsoptionen
        public VersammlungSettingsView()
        {
            InitializeComponent();
        }

        private void GroupBox_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var groupBox = (GroupBox)sender;
            if (groupBox.State == GroupBoxState.Normal)
                groupBox.State = GroupBoxState.Maximized;
        }
    }
}
