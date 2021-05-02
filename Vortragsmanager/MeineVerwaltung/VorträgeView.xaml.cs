using System.Windows.Controls;

namespace Vortragsmanager.MeineVerwaltung
{
    /// <summary>
    /// Interaction logic for VorlagenView.xaml
    /// </summary>
    public partial class VorträgeView : UserControl
    {
        public VorträgeView()
        {
            InitializeComponent();
        }

        private void TableView_GetIsEditorActivationAction(object sender, DevExpress.Xpf.Grid.GetIsEditorActivationActionEventArgs e)
        {
            if (e.ActivationAction == DevExpress.Xpf.Editors.ActivationAction.MouseLeftButtonDown)
                e.IsActivationAction = true;
        }

        private void UserControl_SizeChanged(object sender, System.Windows.SizeChangedEventArgs e)
        {
            if (e.WidthChanged)
            {
                if (e.NewSize.Width > 1000)
                {
                    if (MasterLayoutControl.Orientation == Orientation.Vertical)
                        MasterLayoutControl.Orientation = Orientation.Horizontal;
                        
                }
                else 
                {
                    if (MasterLayoutControl.Orientation == Orientation.Horizontal)
                        MasterLayoutControl.Orientation = Orientation.Vertical;
                }
            }
        }
    }
}