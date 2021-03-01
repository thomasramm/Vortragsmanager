using DevExpress.Xpf.LayoutControl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Vortragsmanager.Datamodels;

namespace Vortragsmanager.UserControls
{
    /// <summary>
    /// Interaction logic for CalendarYearShort.xaml
    /// </summary>
    public partial class CalendarYearShort : UserControl
    {
        private Dictionary<DateTime, CalendarYearShortItem> _calendar = new Dictionary<DateTime, CalendarYearShortItem>(53);
        private int _year;

        public CalendarYearShort()
        {
            InitializeComponent();
            LoadYear(Core.Helper.DisplayedYear);
        }


        public static readonly DependencyProperty PersonProperty = DependencyProperty.Register("Person", typeof(Speaker), typeof(CalendarYearShort), new PropertyMetadata(null));

        public Speaker Person
        {
            get
            {
                return (Speaker) GetValue(PersonProperty);
            }
            set
            {
                SetValue(PersonProperty, value);
                UpdateCalendar();
            }
        }

        //public Speaker Person
        //{
        //    get => person; 
        //    set
        //    {
        //        person = value;
        //        UpdateCalendar();
        //    }
        //}

        private void LoadYear(int year)
        {
            yearLabel.Content = year;
            _calendar.Clear();
            flowLayout.Children.Clear();
            _year = year;

            var start = Core.Helper.GetConregationDay(new DateTime(year, 1, 1));
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
            foreach(var myEvent in DataContainer.MeinPlan.Where(x => x.Kw/100 == _year && x.Status == EventStatus.Ereignis).Cast<SpecialEvent>())
            {
                var datum = Core.Helper.CalculateWeek(myEvent.Kw);
                var item = _calendar[datum];
                item.IsEvent = true;
                item.Text += Environment.NewLine + myEvent.Anzeigetext;
            }


            if (Person == null)
                return;



            var startrange = (_year - 1) * 100 + 50;
            var endrange = (_year + 1) * 100 + 1;

            //Vortrag in meiner Versammlung
            foreach (var busy in DataContainer.MeinPlan.Where(x => (x.Kw >= startrange && x.Kw < endrange ) && x.Status == EventStatus.Zugesagt).Cast<Invitation>().Where(x => x.Ältester == Person))
            {
                var datum = Core.Helper.CalculateWeek(busy.Kw);
                if (datum.Year == _year)
                {
                    var item = _calendar[datum];
                    item.IsBusy = true;
                    item.Text += Environment.NewLine + $"Vortrag in {DataContainer.MeineVersammlung.Name}";
                }

                //4 Wochen um das Datum herum...
                CalculateAroundTalk(datum);
            }

            //Vortrag in anderer Versammlung
            foreach (var busy in DataContainer.ExternerPlan.Where(x => x.Ältester == Person && (x.Kw >= startrange && x.Kw < endrange)))
            {
                var datum = Core.Helper.CalculateWeek(busy.Kw);
                if (datum.Year == _year)
                {
                    var item = _calendar[datum];
                    item.IsBusy = true;
                    item.Text += Environment.NewLine + $"Vortrag in {busy.Versammlung.Name}";
                }

                //4 Wochen um das Datum herum...
                CalculateAroundTalk(datum);
            }

            //Vorsitz oder Leser
            foreach(var busy in DataContainer.AufgabenPersonKalender.Where(x => x.Kw >= startrange && x.Kw < endrange))
            {
                if (busy.Leser?.VerknüpftePerson == Person || busy.Vorsitz?.VerknüpftePerson == Person)
                {
                    var datum = Core.Helper.CalculateWeek(busy.Kw);
                    var item = _calendar[datum];
                    item.IsBusy = true;
                    item.Text += Environment.NewLine;
                    item.Text += (busy.Leser?.VerknüpftePerson == Person) ? "Leser" : "Vorsitzender";
                }
            }

            //Abwesenheit, für das Ändern muß die Person zugeordnet werden
            foreach(var item in _calendar)
            {
                item.Value.SetPerson(Person);
            }

            //Vorhandene Abwesenheiten eintragen
            foreach(var busy in DataContainer.Abwesenheiten.Where(x => x.Kw/100 == _year && x.Redner == Person))
            {
                var datum = Core.Helper.CalculateWeek(busy.Kw);
                var item = _calendar[datum];
                item.SetAbwesenheit(busy);
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
