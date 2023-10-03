using DevExpress.Xpf.Core;
using System;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Markup;
using Vortragsmanager.DataModels;
using Vortragsmanager.Module;
using Vortragsmanager.PageModels;

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
            //Spracheinstellungen
            Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("de-DE");
            LanguageProperty.OverrideMetadata(typeof(FrameworkElement), new FrameworkPropertyMetadata(XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag)));

            //Exceptions der Module SQLite und DevExpress abfangen
            AppDomain.CurrentDomain.FirstChanceException += (sender, eventArgs) =>
            {
                if (eventArgs.Exception.Message.Contains("System.Data.SQLite.SEE.License"))
                    return;
                if (eventArgs.Exception.Message.Contains("DevExpress.Xpf.Themes.MetropolisDark"))
                    return;
                if (eventArgs.Exception.Message.Contains("DevExpress.Xpf.Themes.Office2019White"))
                    return;
                Log.Error("FirstChanceException", eventArgs.Exception.Message);
                Log.Error("FirstChanceExceptionStackTrace", eventArgs.Exception.StackTrace);
            };

            //Programmeinstellungen
            Helper.Helper.GlobalSettings = new MyGloabalSettings();
            Helper.Helper.GlobalSettings.Read();
            Log.Start();

            //Erster Start nach Update? Zeige Changelog
            if (!Helper.Helper.GlobalSettings.HideChangelog)
            {
                Update.ShowChanges();
            }

            //Datei öffnen
            OpenFileOrLoadWzard();

            //UI erstellen
            InitializeComponent();
            DataContext = Helper.Helper.GlobalSettings;
            Helper.Helper.GlobalSettings.RefreshTitle();
            EinstellungenPageModel.ThemeIsDark = Helper.Helper.GlobalSettings.ThemeIsDark;

            //Bereinigungs Tasks
            if (!Backup.CleanOldBackups())
            {
                ThemedMessageBox.Show("Fehler", "Das Programm kann nicht auf das Backup-Archiv zugreifen. Dieser Fehler sollte dringend geprüft werden. Bitte prüfe den Zugriff auf " + Backup.GetBackupFile(), MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void OpenFileOrLoadWzard()
        {
            var filename = GetFileName();

            try
            {
                IoSqlite.ReadContainer(filename);
            }
            catch
            {
                if (!Backup.RestoreLastBackup())
                    DataContainer.IsDemo = true;
            }

            if (DataContainer.IsDemo)
            {
                Initialize.LoadWizard();
            }

            if (!DataContainer.IsInitialized)
            {
                ThemedMessageBox.Show("Fehler", "Vortragsmanager ist nicht initialisiert. Das Programm wird beendet", MessageBoxButton.OK, MessageBoxImage.Error);
                Close();
            }
        }

        private string GetFileName()
        {
            //Doppelklick auf Datei im Windows Explorer
            string[] args = Environment.GetCommandLineArgs();
            if (args.Length >= 2 && File.Exists(args[1]))
            {
                Helper.Helper.GlobalSettings.sqlite = args[1];
            }
            //Datei, eingetragen in den Settings, aber ohne Pfad.
            else if (Helper.Helper.GlobalSettings.sqlite == "vortragsmanager.sqlite3" || Helper.Helper.GlobalSettings.sqlite == "demo.sqlite3")
            {
                Helper.Helper.GlobalSettings.sqlite = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\" + Helper.Helper.GlobalSettings.sqlite;
            }

            Helper.Helper.GlobalSettings.Save();
            return Helper.Helper.GlobalSettings.sqlite;
        }

        private void HelpButton_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://thomasramm.github.io/Vortragsmanager/");
        }
        private void ChangesButton_Click(object sender, RoutedEventArgs e)
        {
            Update.ShowChanges(true);
        }
    }
}