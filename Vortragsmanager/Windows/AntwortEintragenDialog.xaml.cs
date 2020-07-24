using System.Windows;

namespace Vortragsmanager.Views
{
    /// <summary>
    /// Interaction logic for AntwortEintragenDialog.xaml
    /// </summary>
    public partial class AntwortEintragenDialog : Window
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