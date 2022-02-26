using System.Windows;
using Vortragsmanager.Interface;

namespace Vortragsmanager.Views
{
    /// <summary>
    /// Interaktionslogik für BuchungLöschenCommandDialog.xaml
    /// </summary>
    public partial class leerDialog : Window, ICloseable
    {
        public leerDialog()
        {
            InitializeComponent();
        }
    }
}