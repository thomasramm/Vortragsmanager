using DevExpress.Xpf.LayoutControl;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Vortragsmanager.Core;
using Vortragsmanager.Properties;

namespace Vortragsmanager.Navigation
{
    /// <summary>
    /// Interaktionslogik für DashboardView.xaml
    /// </summary>
    public partial class DashboardView : UserControl
    {
        public DashboardView()
        {
            InitializeComponent();

            //speichern
            if (DataContainer.IsInitialized)
            {
                var file = IoSqlite.SaveContainer(Settings.Default.sqlite, false);
                Settings.Default.sqlite = file;
                Settings.Default.Save();
            }
        }
    }

    public sealed class ScalablePaddingConverter : IValueConverter
    {
        public ScalablePaddingConverter()
        {
            MinPadding = 35;
        }

        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var controlHeight = (double)value;
            double desiredContentHeight = 3 * Tile.LargeHeight + 2 * TileLayoutControl.DefaultItemSpace + 20;
            double paddingY = Math.Floor(Math.Max(0d, controlHeight - desiredContentHeight) / 2d);
            paddingY = Math.Max(MinPadding, Math.Min(paddingY, TileLayoutControl.DefaultPadding.Top));
            double relativePadding = (paddingY - MinPadding) / (TileLayoutControl.DefaultPadding.Top - MinPadding);
            double paddingX = Math.Floor(MinPadding + relativePadding * (TileLayoutControl.DefaultPadding.Left - MinPadding));
            return new Thickness(paddingX, paddingY, paddingX, paddingY);
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public double MinPadding { get; set; }
    }
}