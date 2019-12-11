using System;
using DevExpress.Xpf.Core;
using Vortragsmanager.Core;

namespace Vortragsmanager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : ThemedWindow
    {
        public MainWindow()
        {
            IoExcel.ReadContainer(@"C:\Daten\Thomas\Projekte\Vortragsmanager\Rohdaten\Data.xlsx");
            Core.Templates.LoadTemplates();
            Console.WriteLine($"{Core.DataContainer.Vorträge.Count} Vorträge, {Core.DataContainer.Versammlungen.Count} Versammlungen, {Core.DataContainer.Redner.Count} Redner geladen");
            IoSqlite.SaveContainer(@"C:\Daten\Thomas\Projekte\Vortragsmanager\Rohdaten\vortragsmanager.vmf");
            
            InitializeComponent();
        }
    }
}
