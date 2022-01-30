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
using Vortragsmanager.Core;
using Vortragsmanager.Datamodels;

namespace Vortragsmanager.UserControls
{
    /// <summary>
    /// Interaktionslogik für DashboardWeekItem.xaml
    /// </summary>
    public partial class DashboardWeekItem : UserControl
    {
        private int woche;

        public DashboardWeekItem()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty WocheProperty = DependencyProperty.Register("Woche", 
            typeof(int), 
            typeof(DashboardWeekItem), 
            new PropertyMetadata(-1, OnWocheChanged));

        public int Woche
        {
            get
            {
                return (int)GetValue(WocheProperty);
            }
            set
            {
                SetValue(WocheProperty, value);
            }
        }

        static void OnWocheChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var instance = (DashboardWeekItem)d;
            instance.OnWocheChanged();
        }

        void OnWocheChanged()
        {
            _week.Content = Helper.CalculateWeek(Woche).ToShortDateString();
            LoadMeinPlan(Woche);
            LoadMeineRedner(Woche);
        }

        private void LoadMeinPlan(int week)
        {           
            var prog = DataContainer.MeinPlan.FirstOrDefault(x => x.Kw == week);
            if (prog.Status == EventStatus.Ereignis)
            {
                _meinPlan.Text = prog.Anzeigetext;
            }
            else if (prog.Status == EventStatus.Zugesagt)
            {
                var einladung = (Invitation)prog;
                _meinPlan.Text = einladung.Ältester.Name + Environment.NewLine
                    + "  in " + einladung.Ältester.Versammlung.Name + Environment.NewLine
                    + "  Nr "+ einladung.Vortrag.Vortrag.ToString() + Environment.NewLine
                    + "  Tel: " + einladung.Ältester.Mobil ?? einladung.Ältester.Telefon;
            }
        }

        private void LoadMeineRedner(int week)
        {
            var nextRedner = DataContainer.ExternerPlan.Where(x => x.Kw == week);
            {
                var message = string.Empty;
                int kw = Helper.CurrentWeek;
                foreach (var r in nextRedner)
                {
                    if (kw == Helper.CurrentWeek || kw == r.Kw)
                    {
                        kw = r.Kw;
                        message += r.Ältester.Name + Environment.NewLine
                            + "  in " + r.Versammlung.Name + " | " + r.Zeit.ToString() + Environment.NewLine
                            + "  Nr. " + r.Vortrag?.Vortrag.ToString() + Environment.NewLine;
                    }
                }
                _meineRedner.Text = message.TrimEnd('\n');
            }
        }
    }
}
