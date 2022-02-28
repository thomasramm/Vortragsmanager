using System.Windows;

namespace Vortragsmanager.Windows
{
    /// <summary>
    /// Interaction logic for AntwortEintragenDialog.xaml
    /// </summary>
    public partial class AntwortEintragenDialog
    {
        public AntwortEintragenDialog()
        {
            InitializeComponent();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}