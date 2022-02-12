using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Navigation;
using DevExpress.Data.Browsing;
using DevExpress.Mvvm;

namespace Vortragsmanager.MeineVerwaltung
{
    /// <summary>
    /// Interaction logic for EinstellungenView.xaml
    /// </summary>
    public partial class EinstellungenView : UserControl
    {
        public EinstellungenView()
        {
            InitializeComponent();
        }


        private void ExcelFile_ValidateExists(object sender, DevExpress.Xpf.Editors.ValidationEventArgs e)
        {
            e.IsValid = File.Exists(e.Value.ToString());
        }
    }
}