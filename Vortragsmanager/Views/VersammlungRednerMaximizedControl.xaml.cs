using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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
            a.NeueVorträgeListe = a.NeuerVortrag.Nummer.ToString(Core.DataContainer.German);
            a.NeuenVortragSpeichern();
        }
    }
}