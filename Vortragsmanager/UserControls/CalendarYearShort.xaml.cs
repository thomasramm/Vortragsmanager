using DevExpress.Xpf.LayoutControl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Vortragsmanager.Datamodels;

namespace Vortragsmanager.UserControls
{
    /// <summary>
    /// Interaction logic for CalendarYearShort.xaml
    /// </summary>
    public partial class CalendarYearShort : UserControl
    {
        private Dictionary<DateTime, CalendarYearShortItem> _calendar = new Dictionary<DateTime, CalendarYearShortItem>(53);
        private Speaker person;
        private int _year;

        public CalendarYearShort()
        {
            InitializeComponent();
            LoadYear(Core.Helper.DisplayedYear);
        }

        public Speaker Person
        {
            get => person; 
            set
            {
                person = value;
                UpdateCalendar();
            }
        }

        private void LoadYear(int year)
        {
            yearLabel.Content = year;
            _calendar.Clear();
            flowLayout.Children.Clear();
            _year = year;

            var start = Core.Helper.GetSunday(new DateTime(year, 1, 1));
            var currentMonth = 0;
            while (start.Year == year)
            {
                CalendarYearShortItem item;
                if (start.Month != currentMonth)
                {
                    item = new CalendarYearShortItem(start.Month);
                    FlowLayoutControl.SetIsFlowBreak(item, true);
                    flowLayout.Children.Add(item);
                    _calendar.Add(new DateTime(1900, start.Month, 1), item);
                    currentMonth = start.Month;
                }

                item = new CalendarYearShortItem(start);
                flowLayout.Children.Add(item);
                _calendar.Add(start, item);
                start = start.AddDays(7);
            }
            UpdateCalendar();
        }

        private void UpdateCalendar()
        {
            foreach(var item in _calendar)
            {
                item.Value.Reset();
            }

            //Events
            foreach(var myEvent in DataContainer.MeinPlan.Where(x => x.Datum.Year == _year && x.Status == EventStatus.Ereignis).Cast<SpecialEvent>())
            {
                var item = _calendar[myEvent.Datum];
                item.IsEvent = true;
                item.Text += Environment.NewLine + myEvent.Anzeigetext;
            }


            if (person == null)
                return;

            var startrange = new DateTime(_year - 1, 12, 1);
            var endrange = new DateTime(_year + 1, 2, 1);

            //Vortrag in meiner Versammlung
            foreach (var busy in DataContainer.MeinPlan.Where(x => (x.Datum >= startrange && x.Datum < endrange ) && x.Status == EventStatus.Zugesagt).Cast<Invitation>().Where(x => x.Ältester == Person))
            {
                if (busy.Datum.Year == _year)
                {
                    var item = _calendar[busy.Datum];
                    item.IsBusy = true;
                    item.Text += Environment.NewLine + $"Vortrag in {DataContainer.MeineVersammlung.Name}";
                }

                //4 Wochen um das Datum herum...
                CalculateAroundTalk(busy.Datum);
            }

            //Vortrag in anderer Versammlung
            foreach (var busy in DataContainer.ExternerPlan.Where(x => x.Ältester == Person && (x.Datum >= startrange && x.Datum < endrange)))
            {
                if (busy.Datum.Year == _year)
                {
                    var item = _calendar[busy.Datum];
                    item.IsBusy = true;
                    item.Text += Environment.NewLine + $"Vortrag in {busy.Versammlung.Name}";
                }

                //4 Wochen um das Datum herum...
                CalculateAroundTalk(busy.Datum);
            }

            //Vorsitz oder Leser
            foreach(var busy in DataContainer.AufgabenPersonKalender.Where(x => x.Datum.Year == _year))
            {
                if (busy.Leser?.VerknüpftePerson == person || busy.Vorsitz?.VerknüpftePerson == person)
                {
                    var item = _calendar[busy.Datum];
                    item.IsBusy = true;
                    item.Text += Environment.NewLine;
                    item.Text += (busy.Leser?.VerknüpftePerson == person) ? "Leser" : "Vorsitzender";
                }
            }

            //Urlaub
            //ToDo:NotImplemented....

        }

        private void CalculateAroundTalk(DateTime datum)
        {
            var start = datum.AddMonths(-1);
            var ende = datum.AddMonths(+1);
            foreach(var item in _calendar.Where(x => x.Key >= start && x.Key <= ende))
            {
                item.Value.IsAroundTalk = true;
            }
        }

        private void NextYear_Click(object sender, RoutedEventArgs e)
        {
            LoadYear(_year += 1);
        }

        private void PreviousYear_Click(object sender, RoutedEventArgs e)
        {
            LoadYear(_year -= 1);
        }

        private void ResetYear_MouseDown(object sender, MouseButtonEventArgs e)
        {
            LoadYear(DateTime.Today.Year);
        }
    }
}
