using DevExpress.Xpf.Core;

namespace Vortragsmanager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : ThemedWindow
    {
        public MainWindow()
        {
            IoSqlite.ReadContainer(Settings.Default.sqlite);

            InitializeComponent();
        }
    }
}
