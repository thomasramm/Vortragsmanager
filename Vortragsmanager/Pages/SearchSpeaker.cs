using System;
using System.Windows.Controls;
using DevExpress.Xpf.WindowsUI.Navigation;

namespace Vortragsmanager.Pages
{
    /// <summary>
    /// Interaktionslogik für ExternalQuestionEdit.xaml
    /// </summary>
    public partial class MeinPlanRednerSuchenPage : UserControl, INavigationAware
    {
        public MeinPlanRednerSuchenPage()
        {
            InitializeComponent();
        }

        public void NavigatedTo(NavigationEventArgs e)
        {
            if (e is null)
                return;

            if (e.Parameter is null)
                return;

            var datum = (DateTime)e.Parameter;

            if (datum == null)
                return;

            var data = (PageModels.MeinPlanRednerSuchenPageModel)DataContext;
            foreach (var t in data.FreieTermine)
            {
                t.IsChecked = (t.Datum == datum);
            }
        }

        /// <summary>
        /// Ich verlasse gleich die View.
        /// </summary>
        /// <param name="e">Parameter.</param>
        public void NavigatingFrom(NavigatingEventArgs e)
        {
        }

        /// <summary>
        /// Ich habe die view bereits verlassen.
        /// </summary>
        /// <param name="e">Parameter.</param>
        public void NavigatedFrom(NavigationEventArgs e)
        {
        }
    }
}