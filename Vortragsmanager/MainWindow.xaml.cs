﻿using DevExpress.Xpf.Core;
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
            //IoExcel.ReadContainer(@"C:\Daten\Thomas\Projekte\Vortragsmanager\Rohdaten\Data.xlsx");
            //Templates.LoadTemplates();
            IoSqlite.ReadContainer(Settings.Default.sqlite);
            Anonymisieren.Start();

            InitializeComponent();
        }
    }
}