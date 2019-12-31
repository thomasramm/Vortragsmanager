using System.Windows.Controls;

namespace Vortragsmanager.Views
{
    /// <summary>
    /// Interaction logic for AntwortEintragenView.xaml
    /// </summary>
    public partial class AntwortEintragenView : UserControl
    {
        public AntwortEintragenView()
        {
            InitializeComponent();

            var data = (AntwortEintragenViewModel)AntwortEintragen.DataContext;
            data.LoadData();
        }
    }
}