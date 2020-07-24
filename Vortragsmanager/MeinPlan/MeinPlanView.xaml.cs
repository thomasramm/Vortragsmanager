using System;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Vortragsmanager.MeinPlan
{
    /// <summary>
    /// Interaktionslogik für MeinPlanView.xaml
    /// </summary>
    public partial class MeinPlanView : UserControl
    {
        public MeinPlanView()
        {
            InitializeComponent();
        }

        private void Content_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var l = (sender as Label);
            var m = l.Content?.ToString();
            if (m is null)
                return;

            Clipboard.SetText(m);
            var original = l.Background;
            l.Background = new SolidColorBrush(Colors.Green);

            var timer = new Timer() { Interval = 250, Enabled = true, AutoReset = false };
            timer.Elapsed += (s, ev) => l.Dispatcher.BeginInvoke((Action)delegate () { l.Background = original; timer.Dispose(); });
        }
    }
}