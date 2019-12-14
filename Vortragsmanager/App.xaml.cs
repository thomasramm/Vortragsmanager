using System.Windows;
using Vortragsmanager.Core;
using Vortragsmanager.Properties;

namespace Vortragsmanager
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Exit(object sender, ExitEventArgs e)
        {
            IoSqlite.SaveContainer(Settings.Default.sqlite);
        }
    }
}
