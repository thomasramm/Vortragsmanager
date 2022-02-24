using System.Windows.Controls;

namespace Vortragsmanager.Pages
{
    /// <summary>
    /// Interaction logic for VorlagenView.xaml
    /// </summary>
    public partial class HistorieSicherungPage : UserControl
    {
        public HistorieSicherungPage()
        {
            InitializeComponent();
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