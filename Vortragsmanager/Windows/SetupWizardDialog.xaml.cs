using System.IO;
using Vortragsmanager.Interface;

namespace Vortragsmanager.Windows
{
    /// <summary>
    /// Interaktionslogik für BuchungLöschenCommandDialog.xaml
    /// </summary>
    public partial class SetupWizardDialog : ICloseable
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