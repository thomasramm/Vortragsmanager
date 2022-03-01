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
            }
        }
    }
}