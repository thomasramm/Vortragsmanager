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
            //Spracheinstellungen immer auf DEUTSCH
            Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("de-DE");
            LanguageProperty.OverrideMetadata(
                typeof(FrameworkElement),
                new FrameworkPropertyMetadata(XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag)));

            //Logging
            Log.Start();
            AppDomain.CurrentDomain.FirstChanceException += (sender, eventArgs) =>
            {
                Log.Error("FirstChanceException", eventArgs.Exception.Message);
                Log.Error("FirstChanceExceptionStackTrace", eventArgs.Exception.StackTrace);
            };

#if DEBUG
                Settings.Default.sqlite = "demo.sqlite3";
#endif
            //ToDo: Entfernen vor Produktiv Release
            //var path = this.GetType().Assembly.Location;
            //FileInfo fileInfo = new FileInfo(path);
            //Settings.Default.sqlite = fileInfo.DirectoryName + @"\demo.sqlite3";
            //MessageBox.Show("Dies ist eine Testversion. Es wird nicht eure Planung geöffnet, sondern die Datei" 
            //    + Environment.NewLine + Settings.Default.sqlite);

            //Erster Start nach Update?
            if (!Settings.Default.HideChangelog)
            {
                Update.ShowChanges();
            }

            //Datei öffnen
            string[] args = Environment.GetCommandLineArgs();
            if (args.Length >= 2 && File.Exists(args[1]))
            {
                Settings.Default.sqlite = args[1];
            }
            else if (Settings.Default.sqlite == "vortragsmanager.sqlite3" || Settings.Default.sqlite == "demo.sqlite3")
            {
                Settings.Default.sqlite = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\" + Settings.Default.sqlite;
            }
            Settings.Default.Save();
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

            //UI erstellen
            InitializeComponent();

            //Daten einlesen, Datenklassen bereitstellen
            Helper.GlobalSettings = new MyGloabalSettings();
            DataContext = Helper.GlobalSettings;
                        
            //Bereinigungs Tasks
            Backup.CleanOldBackups();

            //Style Anpassungen
            Helper.GlobalSettings.RefreshTitle();
            ApplicationThemeHelper.ApplicationThemeName = Settings.Default.Theme;
            ToggleSwitch_Changed(null, null);
        }

        private void HelpButton_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://thomasramm.github.io/Vortragsmanager/");
        }
        private void ChangesButton_Click(object sender, RoutedEventArgs e)
        {
            Update.ShowChanges(true);
        }

        private void ToggleSwitch_Changed(object sender, RoutedEventArgs e)
        {
            //var button = (ToggleSwitch)sender;
            //if (button == null)
            //    return;

            hamburgerMenu.Margin = Helper.StyleIsDark == false ? new Thickness(-10, 0, -10, -10) : new Thickness(-10, -7, -10, -10);
        }
    }
}