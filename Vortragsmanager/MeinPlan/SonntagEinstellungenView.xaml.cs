using DevExpress.XtraPrinting.Native;
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

namespace Vortragsmanager.MeinPlan
{
    /// <summary>
    /// Interaction logic for AufgabenView.xaml
    /// </summary>
    public partial class SonntagEinstellungenView : UserControl
    {
        private SonntagEinstellungenViewModel _model;

        public SonntagEinstellungenView()
        {
            InitializeComponent();
            _model = (SonntagEinstellungenViewModel)DataContext;
        }

        private void SpinEdit_EditValueChanged(object sender, DevExpress.Xpf.Editors.EditValueChangedEventArgs e)
        {
            var x = Convert.ToInt32(e.NewValue, Core.Helper.German);
            _model.MonateAnzahlAnzeige = x;
        }
    }
}
