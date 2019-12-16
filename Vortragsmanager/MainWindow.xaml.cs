using DevExpress.Xpf.Core;
using System.IO;
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