using DevExpress.Xpf.Core;
using System;
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

            Log.Start();

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
                Close();

            InitializeComponent();

            var fi = new FileInfo(Settings.Default.sqlite);
            Title = "Vortragsmanager DeLuxe | " + fi.Name;

            Updater.CheckForUpdates();
        }
    }
}