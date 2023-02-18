using System.IO;

namespace Vortragsmanager.Pages
{
    /// <summary>
    /// Interaction logic for EinstellungenView.xaml
    /// </summary>
    public partial class EinstellungenPage
    {
        public EinstellungenPage()
        {
            InitializeComponent();
        }


        private void ExcelFile_ValidateExists(object sender, DevExpress.Xpf.Editors.ValidationEventArgs e)
        {
            e.IsValid = (File.Exists(e.Value.ToString()) || string.IsNullOrEmpty(e.Value.ToString()));
        }
    }
}