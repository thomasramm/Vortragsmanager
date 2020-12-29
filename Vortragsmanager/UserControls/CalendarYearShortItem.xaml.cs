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
    /// Interaction logic for CalendarYearShortItem.xaml
    /// </summary>
    public partial class CalendarYearShortItem : UserControl
    {
        private bool isAroundTalk;
        private bool isEvent1;
        private bool isBusy;
        private bool isHoliday;
        private bool showDetails;
        private string text;
        private DateTime _date;
        private readonly bool isMonth;
        private Busy _abwesenheit;

        public CalendarYearShortItem()
        {
            InitializeComponent();
            UpdateUi();
        }

        public CalendarYearShortItem(DateTime date)
        {
            InitializeComponent();
            Date = date;
            UpdateUi();
        }

        public CalendarYearShortItem(int month)
        {
            InitializeComponent();
            isMonth = true;
            Width = 40;
            Date = new DateTime(1900, month, 1);
        }

        public DateTime Date
        {
            get => _date; set
            {
                _date = value;
                UpdateUi();
            }
        }

        public string Text
        {
            get => text; set
            {
                text = value;
                UpdateUi();
            }
        }

        public bool ShowDetails
        {
            get => showDetails; set
            {
                showDetails = value;
                UpdateUi();
            }
        }

        public bool IsHoliday
        {
            get => isHoliday; set
            {
                isHoliday = value;
                UpdateUi();
            }
        }

        public bool IsBusy
        {
            get => isBusy;
            set
            {
                isBusy = value;
                UpdateUi();
            }
        }

        public bool IsEvent
        {
            get => isEvent1;
            set
            {
                isEvent1 = value;
                UpdateUi();
            }
        }

        public bool IsAroundTalk
        {
            get => isAroundTalk; set
            {
                isAroundTalk = value;
                UpdateUi();
            }
        }

        public void SetPerson(Speaker person)
        {
            _abwesenheit = new Busy(person, _date);
        }

        public void SetAbwesenheit(Busy abwesenheit)
        {
            _abwesenheit = abwesenheit;
            IsHoliday = true;
        }

        public void Reset()
        {
            IsHoliday = false;
            IsBusy = false;
            IsAroundTalk = false;
            Text = Date.ToShortDateString();
        }

        private void UpdateUi()
        {
            //Hintergrundfarbe
            var color = Color.FromRgb(51, 51, 51);

            if (isBusy)
                color = Colors.OrangeRed;
            else if (isHoliday)
                color = Colors.DodgerBlue;
            else if (IsEvent)
                color = Colors.SlateGray;
            else if (IsAroundTalk)
                color = Color.FromRgb(100, 60, 50);

            //Text
            Inhalt.Content = Date.Day;

            //ToolTip
            ToolTip = Text;

            if (isMonth)
            {
                var text = Date.ToString("MMMM", Core.Helper.German);
                if (showDetails)
                    Inhalt.Content = text;
                else
                    Inhalt.Content = Date.ToString("MMM", Core.Helper.German);
                ToolTip = text;
            }

            Rahmen.Background = new SolidColorBrush(color);
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            IsHoliday = !IsHoliday;
            UpdateUi();
            if (IsHoliday)
                DataContainer.Abwesenheiten.Add(_abwesenheit);
            else
                DataContainer.Abwesenheiten.Remove(_abwesenheit);
        }
    }
}
