using Vortragsmanager.Datamodels;
using Vortragsmanager.Module;
using Vortragsmanager.Properties;

namespace Vortragsmanager.Pages
{
    /// <summary>
    /// Interaktionslogik für DashboardView.xaml
    /// </summary>
    public partial class DashboardPage
    {
        public DashboardPage()
        {
            InitializeComponent();

            //speichern
            if (!DataContainer.IsInitialized) 
                return;

            var file = IoSqlite.SaveContainer(Settings.Default.sqlite, Settings.Default.SaveBackups);
            Settings.Default.sqlite = file;
            Settings.Default.Save();
        }
    }
}