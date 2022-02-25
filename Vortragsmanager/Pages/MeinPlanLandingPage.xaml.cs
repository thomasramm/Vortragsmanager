using Vortragsmanager.Interface;

namespace Vortragsmanager.Pages
{
    /// <summary>
    /// Interaktionslogik für MeinPlan.xaml
    /// </summary>
    public partial class MeinPlanLandingPage
    {
        private readonly INavigation _currentModel;

        public MeinPlanLandingPage()
        {
            InitializeComponent();
            _currentModel = (INavigation)DataContext;
            KalenderButton.IsChecked = true;
        }
    }
}
