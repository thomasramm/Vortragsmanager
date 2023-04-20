using System;
using System.Windows.Input;
using System.Windows.Media;
using Vortragsmanager.DataModels;
using Vortragsmanager.Helper;
using Vortragsmanager.PageModels;

namespace Vortragsmanager.UserControls
{
    /// <summary>
    /// Interaction logic for CalendarYearShortItem.xaml
    /// </summary>
    public partial class CalendarYearShortItem
    {
        private bool _isAroundTalk;
        private bool _isEvent;
        private bool _isBusy;
        private bool _isHoliday;
        private bool _showDetails;
        private string _text;
        private DateTime _date;
        private readonly bool _isMonth;
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
            _isMonth = true;
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
            get => _text; set
            {
                _text = value;
                UpdateUi();
            }
        }

        public bool ShowDetails
        {
            get => _showDetails; set
            {
                _showDetails = value;
                UpdateUi();
            }
        }

        public bool IsHoliday
        {
            get => _isHoliday; set
            {
                _isHoliday = value;
                UpdateUi();
            }
        }

        public bool IsBusy
        {
            get => _isBusy;
            set
            {
                _isBusy = value;
                UpdateUi();
            }
        }

        public bool IsEvent
        {
            get => _isEvent;
            set
            {
                _isEvent = value;
                UpdateUi();
            }
        }

        public bool IsAroundTalk
        {
            get => _isAroundTalk; set
            {
                _isAroundTalk = value;
                UpdateUi();
            }
        }

        public void SetPerson(Speaker person)
        {
            _abwesenheit = new Busy(person, DateCalcuation.CalculateWeek(_date));
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
            Color color;

            //Hintergrundfarbe
            if (EinstellungenPageModel.ThemeIsDark)
            {
                if (_isMonth)
                    color = Color.FromRgb(51, 51, 51);
                else if (_isBusy)
                    color = Colors.OrangeRed;
                else if (_isHoliday)
                    color = Colors.DodgerBlue;
                else if (IsEvent)
                    color = Colors.SlateGray;
                else if (IsAroundTalk)
                    color = Color.FromRgb(100, 60, 50);
                else
                    color = Color.FromRgb(0, 48, 0);
            }
            else
            {
                if (_isMonth)
                    color = Color.FromRgb(255, 255, 255);
                else if (_isBusy)
                    color = Color.FromRgb(255,150,130);
                else if (_isHoliday)
                    color = Colors.DodgerBlue;
                else if (IsEvent)
                    color = Color.FromRgb(160,170,180);
                else if (IsAroundTalk)
                    color = Color.FromRgb(200, 160, 145);
                else
                    color = Color.FromRgb(160, 220, 160);
            }



            //Text
            Inhalt.Content = Date.Day;

            //ToolTip
            ToolTip = Text;

            if (_isMonth)
            {
                var text = Date.ToString("MMMM", Helper.Helper.German);
                Inhalt.Content = ShowDetails ? text : Date.ToString("MMM", Helper.Helper.German);
                ToolTip = text;
            }

            Rahmen.Background = new SolidColorBrush(color);
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            IsHoliday = !IsHoliday;
            UpdateUi();
            if (_abwesenheit == null)
                return;

            if (IsHoliday)
                DataContainer.Abwesenheiten.Add(_abwesenheit);
            else
                DataContainer.Abwesenheiten.Remove(_abwesenheit);
        }
    }
}
