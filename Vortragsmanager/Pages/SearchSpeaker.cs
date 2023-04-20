namespace Vortragsmanager.Pages
{
    /// <summary>
    /// Interaktionslogik für ExternalQuestionEdit.xaml
    /// </summary>
    public partial class MeinPlanRednerSuchenPage
    {
        // ReSharper disable once UnusedMember.Global
        public MeinPlanRednerSuchenPage()
        {
            InitializeComponent();
        }

        public MeinPlanRednerSuchenPage(string datum)
        {
            InitializeComponent();

            if (datum == null)
            {
                return;
            }

            var kw = int.Parse(datum);
            var date = Helper.DateCalcuation.CalculateWeek(kw);

            var data = (PageModels.MeinPlanRednerSuchenPageModel)DataContext;
            foreach (var t in data.FreieTermine)
            {
                t.IsChecked = (t.Datum == date);
                t.IsVisible = t.IsChecked;
            }
        }

        private void Filter1MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            VersammlungenExpander.IsExpanded = !VersammlungenExpander.IsExpanded;
        }

        private void Filter2MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            RednerExpander.IsExpanded = !RednerExpander.IsExpanded;
        }

        private void Filter3MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            VortragExpander.IsExpanded = !VortragExpander.IsExpanded;
        }

        private void Filter4MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            TerminExpander.IsExpanded= !TerminExpander.IsExpanded;
        }


        private void Termin_CheckedChanged(object sender, System.Windows.RoutedEventArgs e)
        {
            var data = (PageModels.MeinPlanRednerSuchenPageModel)DataContext;
            data.FreieTermineCalculateBatch();
        }
    }
}