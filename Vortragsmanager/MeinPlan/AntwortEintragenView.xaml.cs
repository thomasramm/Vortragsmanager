using System.Windows.Controls;

namespace Vortragsmanager.MeinPlan
{
    /// <summary>
    /// Interaction logic for AntwortEintragenView.xaml
    /// </summary>
    public partial class AntwortEintragenView : UserControl
    {
        public AntwortEintragenView()
        {
            InitializeComponent();

            var data = (Views.AntwortEintragenViewModel)AntwortEintragen.DataContext;
            data.LoadData();
        }
    }
}