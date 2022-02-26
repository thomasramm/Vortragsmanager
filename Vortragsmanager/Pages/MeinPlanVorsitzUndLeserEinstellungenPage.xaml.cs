﻿using System;
using System.Windows.Controls;

namespace Vortragsmanager.Pages
{
    /// <summary>
    /// Interaction logic for AufgabenView.xaml
    /// </summary>
    public partial class MeinPlanVorsitzUndLeserEinstellungenPage : UserControl
    {
        private PageModels.MeinPlanVorsitzUndLeserEinstellungenPage _model;

        public MeinPlanVorsitzUndLeserEinstellungenPage()
        {
            InitializeComponent();
            _model = (PageModels.MeinPlanVorsitzUndLeserEinstellungenPage)DataContext;
        }

        private void SpinEdit_EditValueChanged(object sender, DevExpress.Xpf.Editors.EditValueChangedEventArgs e)
        {
            var x = Convert.ToInt32(e.NewValue, Helper.Helper.German);
            _model.MonateAnzahlAnzeige = x;
        }
    }
}
