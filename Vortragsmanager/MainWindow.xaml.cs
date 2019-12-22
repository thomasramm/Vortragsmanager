using DevExpress.Xpf.Core;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Markup;
using Vortragsmanager.Core;
using Vortragsmanager.Properties;
using Vortragsmanager.Views;

namespace Vortragsmanager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : ThemedWindow
    {
        public MainWindow()
        {
            Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.GetCultureInfo("de-DE");
            LanguageProperty.OverrideMetadata(
                typeof(FrameworkElement),
                new FrameworkPropertyMetadata(XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag)));

            //IoExcel.ReadContainer(@"C:\Daten\Thomas\Projekte\Vortragsmanager\Rohdaten\Data.xlsx");
            //Templates.LoadTemplates();
            var filename = Settings.Default.sqlite;

            //ToDo: wizard nur bei leerer DB aufrufen, zum Entwickeln aber hier immer aufrufen...
            filename = @"C:\IchExistiere.Nicht";

            if (File.Exists(filename))
                IoSqlite.ReadContainer(Settings.Default.sqlite);
            else
                Initialize.NewDatabase();

            if (!DataContainer.IsInitialized)
               Close();



            InitializeComponent();
            Updater.CheckForUpdates();
        }
    }
}