using DevExpress.Xpf.Core;
using System;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Markup;
using Vortragsmanager.Datamodels;
using Vortragsmanager.DataModels;
using Vortragsmanager.Module;
using Vortragsmanager.PageModels;
using Vortragsmanager.Properties;

namespace Vortragsmanager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    // ReSharper disable once UnusedMember.Global
    public partial class MainWindow
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
            Helper.Helper.GlobalSettings = new MyGloabalSettings();
            DataContext = Helper.Helper.GlobalSettings;
                        
            //Bereinigungs Tasks
            Backup.CleanOldBackups();

            //Style Anpassungen
            Helper.Helper.GlobalSettings.RefreshTitle();
            EinstellungenPageModel.ThemeIsDark = Settings.Default.ThemeIsDark;
#if DEBUG
            ThemeSwitch.Visibility = Visibility.Visible; 
#endif
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
            HamburgerMenu.Margin = Helper.Helper.StyleIsDark == false 
                ? new Thickness(-10, 0, -10, -10) 
                : new Thickness(-10, -7, -10, -10);
        }
    }
}