using System.Windows.Controls;
using Vortragsmanager.UserControls;

namespace Vortragsmanager.Pages
{
    /// <summary>
    /// Interaction logic for AntwortEintragenView.xaml
    /// </summary>
    public partial class MeinPlanAntwortEintragenPage : UserControl
    {
        public MeinPlanAntwortEintragenPage()
        {
            InitializeComponent();

            var data = (AntwortEintragenViewModel)AntwortEintragen.DataContext;
            data.LoadData();
        }
    }
}