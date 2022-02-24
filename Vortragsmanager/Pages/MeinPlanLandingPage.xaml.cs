using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Navigation;
using DevExpress.Mvvm;
using DevExpress.Mvvm.POCO;
using Vortragsmanager.Interface;
using Vortragsmanager.PageModels;
using Vortragsmanager.UserControls;

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
