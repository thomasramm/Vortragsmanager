using System;
using System.Collections.Generic;
using System.Windows.Controls;
using Vortragsmanager.Datamodels;


namespace Vortragsmanager.UserControls
{

    /// <summary>
    /// Interaction logic for SonntagItem.xaml
    /// </summary>
    public partial class SonntagItem : UserControl
    {
        private SonntagItemViewModel _model;
        public SonntagItem()
        {
            InitializeComponent();
        }

        public SonntagItem(DateTime datum)
        {
            InitializeComponent();
            var kw = Core.Helper.CalculateWeek(datum);
            _model = new SonntagItemViewModel(kw);
            DataContext = _model;
            Datum = datum;
        }

        public DateTime Datum { get; private set; }

        public AufgabenZuordnung SelectedLeser
        {
            get => _model.SelectedLeser;
            set => _model.SelectedLeser = value;
        }

        public AufgabenZuordnung SelectedVorsitz
        {
            get => _model.SelectedVorsitz;
            set => _model.SelectedVorsitz = value;
        }

        public IEnumerable<AufgabenZuordnung> Leser => _model.Leser;

        public IEnumerable<AufgabenZuordnung> Vorsitz => _model.Vorsitz;

        public bool IsVorsitz => _model.IsVorsitz;

        public bool IsLeser => _model.IsLeser;
    }
}
