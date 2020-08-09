using System.IO;
using System.Windows;

namespace Vortragsmanager.Views
{
    /// <summary>
    /// Interaktionslogik für BuchungLöschenCommandDialog.xaml
    /// </summary>
    public partial class SetupWizardDialog : Window, Datamodels.ICloseable
    {
        public SetupWizardDialog()
        {
            InitializeComponent();
        }

        private void ExcelFile_ValidateExists(object sender, DevExpress.Xpf.Editors.ValidationEventArgs e)
        {
            e.IsValid = File.Exists(e.Value?.ToString() ?? string.Empty);
        }
    }
}