using System.Windows.Controls;
using System.Windows.Input;

namespace Vortragsmanager.Views
{
    /// <summary>
    /// Interaction logic for VersammlungRednerMaximizedControl.xaml
    /// </summary>
    public partial class VersammlungRednerMaximizedControl : UserControl
    {
        public VersammlungRednerMaximizedControl()
        {
            InitializeComponent();
        }

        private void NeueVorträgeListe_Validate(object sender, DevExpress.Xpf.Editors.ValidationEventArgs e)
        {
            if (e.Value == null)
                return;
            if (string.IsNullOrEmpty(e.Value.ToString()))
                return;
            e.IsValid = true;
            var a = (SpeakerViewModel)((DevExpress.Xpf.Editors.TextEdit)sender).DataContext;
            a.NeueVorträgeListe = e.Value.ToString();
            a.NeuenVortragSpeichern();
            e.Handled = true;
            e.IsValid = true;
        }

        private void ComboBoxEdit_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter)
                return;

            var cb = (DevExpress.Xpf.Editors.ComboBoxEdit)sender;
            if (cb.SelectedIndex == -1)
                return;

            var a = (SpeakerViewModel)(cb).DataContext;
            a.NeueVorträgeListe = a.NeuerVortrag.Vortrag.Nummer.ToString(Core.Helper.German);
            a.NeuenVortragSpeichern();
        }
    }
}