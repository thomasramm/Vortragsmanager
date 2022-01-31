using System;
using System.Windows.Controls;

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
