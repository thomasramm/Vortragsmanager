﻿using System;
using System.Linq;
using System.Windows;
using Vortragsmanager.Datamodels;
using Vortragsmanager.Enums;
using Vortragsmanager.Helper;

namespace Vortragsmanager.UserControls
{
    /// <summary>
    /// Interaktionslogik für DashboardWeekItem.xaml
    /// </summary>
    public partial class DashboardWeekItem
    {
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
            get => (int)GetValue(WocheProperty);
            set => SetValue(WocheProperty, value);
        }

        static void OnWocheChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var instance = (DashboardWeekItem)d;
            instance.OnWocheChanged();
        }

        void OnWocheChanged()
        {
            Week.Content = DateCalcuation.CalculateWeek(Woche).ToShortDateString();
            LoadMeinPlan(Woche);
            LoadMeineRedner(Woche);
            LoadAufgaben(Woche);
        }

        private void LoadMeinPlan(int week)
        {
            var prog = DataContainer.MeinPlan.FirstOrDefault(x => x.Kw == week);
            if (prog == null) 
                return;
            switch (prog.Status)
            {
                case EventStatus.Ereignis:
                    MeinPlan.Text = prog.Anzeigetext;
                    PhotoViewerToolTip.Width = 0;
                    PhotoViewerToolTip.Source = null;
                    LabelToolTip.Content = null;
                    break;
                case EventStatus.Zugesagt:
                {
                    var einladung = (Invitation) prog;
                    MeinPlan.Text = einladung.Ältester.Name 
                                    + Environment.NewLine
                                    + "  in " + einladung.Ältester.Versammlung.Name 
                                    + Environment.NewLine
                                    + "  Nr " + einladung.Vortrag.Vortrag 
                                    + Environment.NewLine
                                    + "  Tel: " + (einladung.Ältester.Mobil ?? einladung.Ältester.Telefon);
                    PhotoViewerToolTip.Width = einladung.Ältester.Foto == null ? 0 : 500;
                    PhotoViewerToolTip.Source = einladung.Ältester.Foto;
                    LabelToolTip.Content = einladung.Ältester.Name;
                    break;
                }
            }
        }

        private void LoadAufgaben(int week)
        {
            var prog = DataContainer.AufgabenPersonKalender.FirstOrDefault(x => x.Kw == week);
            if (prog != null)
            {
                MeinVorsitz.Text = "Vorsitz: " + (prog.Vorsitz?.PersonName ?? "n.a.") + "\tLeser: " + (prog.Leser?.PersonName ?? "n.a.");
            }
        }

        private void LoadMeineRedner(int week)
        {
            var nextRedner = DataContainer.ExternerPlan.Where(x => x.Kw == week);
            {
                var message = string.Empty;
                int kw = DateCalcuation.CurrentWeek;
                foreach (var r in nextRedner)
                {
                    if (kw == DateCalcuation.CurrentWeek || kw == r.Kw)
                    {
                        kw = r.Kw;
                        message += r.Ältester.Name + Environment.NewLine
                            + "  in " + r.Versammlung.Name + " | " + r.Zeit + Environment.NewLine
                            + "  Nr. " + r.Vortrag?.Vortrag + Environment.NewLine;
                    }
                }
                MeineRedner.Text = message.TrimEnd('\n');
            }
        }
    }
}
