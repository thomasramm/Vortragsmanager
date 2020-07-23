using DevExpress.Xpf.Core;
using System;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Markup;
using Vortragsmanager.Core;
using Vortragsmanager.Datamodels;
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

            Log.Start();
            AppDomain.CurrentDomain.FirstChanceException += (sender, eventArgs) =>
            {
                Log.Error("FirstChanceException", eventArgs.Exception.Message);
                Log.Error("FirstChanceExceptionStackTrace", eventArgs.Exception.StackTrace);
            };

            if (Settings.Default.sqlite == "vortragsmanager.sqlite3")
            {
                Settings.Default.sqlite = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\" + Settings.Default.sqlite;
            }
            var filename = Settings.Default.sqlite;

            if (File.Exists(filename))
                IoSqlite.ReadContainer(filename);
            else
                Initialize.NewDatabase();

            if (!DataContainer.IsInitialized)
            {
                ThemedMessageBox.Show("Fehler", "Vortragsmanager ist nicht initialisiert. Das Programm wird beendet", MessageBoxButton.OK, MessageBoxImage.Error);
                Close();
            }

            InitializeComponent();

            Helper.GlobalSettings = new MyGloabalSettings();
            DataContext = Helper.GlobalSettings;

            Helper.GlobalSettings.RefreshTitle();

            Updater.CheckForUpdates();
        }
    }
}