﻿using DevExpress.Xpf.Core;
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
            FrameworkElement.LanguageProperty.OverrideMetadata(typeof(FrameworkElement),
             new FrameworkPropertyMetadata(XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag)));


            var wizard = new SetupWizardDialog();
            wizard.ShowDialog();

            //IoExcel.ReadContainer(@"C:\Daten\Thomas\Projekte\Vortragsmanager\Rohdaten\Data.xlsx");
            //Templates.LoadTemplates();
            var filename = Settings.Default.sqlite;
            if (File.Exists(filename))
                IoSqlite.ReadContainer(Settings.Default.sqlite);
            else
                IoSqlite.CreateEmptyDatabase(filename);
            InitializeComponent();
            Updater.CheckForUpdates();
        }
    }
}