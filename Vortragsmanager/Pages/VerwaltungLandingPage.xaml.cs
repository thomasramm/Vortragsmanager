using System.Windows;
using System.Windows.Controls.Primitives;

namespace Vortragsmanager.Pages
{
    /// <summary>
    /// Interaktionslogik für Verwaltung.xaml
    /// </summary>
    public partial class VerwaltungLandingPage
    {
        public VerwaltungLandingPage()
        {
            InitializeComponent();
            Redner.IsChecked = true;
        }
    }
}
