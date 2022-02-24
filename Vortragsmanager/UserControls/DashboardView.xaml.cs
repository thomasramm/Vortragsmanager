using System.Windows.Controls;
using Vortragsmanager.Core;
using Vortragsmanager.Datamodels;
using Vortragsmanager.Module;
using Vortragsmanager.Properties;

namespace Vortragsmanager.Navigation
{
    /// <summary>
    /// Interaktionslogik für DashboardView.xaml
    /// </summary>
    public partial class DashboardView : UserControl
    {
        public DashboardView()
        {
            InitializeComponent();

            //speichern
            if (DataContainer.IsInitialized)
            {
                var file = IoSqlite.SaveContainer(Settings.Default.sqlite, Settings.Default.SaveBackups);
                Settings.Default.sqlite = file;
                Settings.Default.Save();
            }
        }
    }
}