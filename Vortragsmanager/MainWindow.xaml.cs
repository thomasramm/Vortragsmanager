using DevExpress.Xpf.Core;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Markup;
using Vortragsmanager.Core;
using Vortragsmanager.Properties;

namespace Vortragsmanager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : ThemedWindow
    {
        public MainWindow()
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("de-DE");
            LanguageProperty.OverrideMetadata(
                typeof(FrameworkElement),
                new FrameworkPropertyMetadata(XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag)));

            var filename = Settings.Default.sqlite;

            //ToDo: für Debug-Zwecke: Wizard starten
            //filename = @"C:\IchExistiere.Nicht";

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