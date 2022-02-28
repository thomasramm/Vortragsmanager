using System;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using DevExpress.Xpf.Core;
using Vortragsmanager.Interface;
using Vortragsmanager.PageModels;

namespace Vortragsmanager.Pages
{
    /// <summary>
    /// Interaktionslogik für MeinPlanView.xaml
    /// </summary>
    public partial class MeinPlanKalenderPage
    {
        public MeinPlanKalenderPage()
        {
            InitializeComponent();
        }

        public MeinPlanKalenderPage(INavigation parentContext)
        {
            InitializeComponent();
            DataContext = new MeinPlanKalenderPageModel(parentContext);
        }

        private void Content_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (!(sender is Label l)) 
                return;

            var m = l.Content?.ToString();
            if (m is null)
                return;

            Clipboard.SetText(m);

            var original = l.Background;
            l.Background = new SolidColorBrush(Colors.Green);

            var timer = new Timer() { Interval = 250, Enabled = true, AutoReset = false };
            timer.Elapsed += (s, ev) => l.Dispatcher.BeginInvoke((Action)delegate
                { l.Background = original; timer.Dispose(); });
        }

        private void ActionButtonEditClick(object sender, RoutedEventArgs e)
        {
            if (sender is SimpleButton btn)
            {
                if (btn.Parent is DockPanel dp && dp.Parent is Border brd)
                {
                    var contextMenu = brd.ContextMenu;
                    if (contextMenu != null)
                    {
                        contextMenu.PlacementTarget = brd;
                        contextMenu.IsOpen = true;
                    }
                }
            }

            e.Handled = true;
        }
    }
}