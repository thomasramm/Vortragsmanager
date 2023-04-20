using Vortragsmanager.DataModels;
using Vortragsmanager.Module;

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
            try
            {
                var file = IoSqlite.SaveContainer(Helper.Helper.GlobalSettings.sqlite, Helper.Helper.GlobalSettings.SaveBackups);
                Helper.Helper.GlobalSettings.sqlite = file;
                Helper.Helper.GlobalSettings.Save();
            }
            catch { }
        }
    }
}