using DevExpress.Xpf.LayoutControl;
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
            var groupBox = (GroupBox)sender;
            if (groupBox.State == GroupBoxState.Normal)
                groupBox.State = GroupBoxState.Maximized;
            else if (e.OriginalSource is DevExpress.Xpf.Core.DXBorder)
                groupBox.State = GroupBoxState.Normal;
        }
    }
}